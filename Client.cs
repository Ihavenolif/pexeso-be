using Fleck;
using Newtonsoft.Json;

class Client
{
    [JsonIgnore]
    public IWebSocketConnection connection { get; private set; }
    [JsonIgnore]
    public Instance instance { get; set; }
    public string name { get; set; }
    public int clientId { get; private set; }

    public bool isReady { get; set; }
    public bool isSpectator { get; set; }

    public List<Character> characters { get; private set; } = new List<Character>();

    static int clientCounter = 0;

    public Client(IWebSocketConnection connection, Instance instance)
    {
        this.connection = connection;
        this.instance = instance;
        this.name = "";
        this.clientId = clientCounter++;
    }

    public void HandleMessage(MessageReceiver messageReceiver)
    {
        Message? message;
        switch (messageReceiver.messageType)
        {
            case MessageType.GetLobbies:
                message = new GetLobbiesMessage();
                break;
            case MessageType.CustomData:
                message = JsonConvert.DeserializeObject<CustomDataMessage>(messageReceiver.data);
                break;
            case MessageType.Login:
                message = JsonConvert.DeserializeObject<LoginMessage>(messageReceiver.data);
                break;
            case MessageType.JoinLobby:
                message = JsonConvert.DeserializeObject<JoinLobbyMessage>(messageReceiver.data);
                break;
            case MessageType.ToggleReady:
                message = new ToggleReadyMessage();
                break;
            case MessageType.JoinAsSpectator:
                message = JsonConvert.DeserializeObject<JoinAsSpectatorMessage>(messageReceiver.data);
                break;
            case MessageType.StartGame:
                message = new StartGameMessage();
                break;
            case MessageType.CardClicked:
                message = JsonConvert.DeserializeObject<CardClickedMessage>(messageReceiver.data);
                break;
            default:
                Console.WriteLine($"Received invalid message from client {this.connection.ConnectionInfo.Id}.");
                return;
        }

        if (message == null)
        {
            Response response = new ErrorResponse("Invalid request");
            this.Send(response);
            return;
        }

        instance.HandleMessage(this, message);
    }

    public void Send(Response response)
    {
        MessageReceiver messageReceiver = new MessageReceiver(response.messageType, JsonConvert.SerializeObject(response));
        connection.Send(JsonConvert.SerializeObject(response));
    }

    public void SendWithDelay(Response response, int delay)
    {
        Task.Run(async () =>
        {
            await Task.Delay(delay);
            Send(response);
        });
    }
}