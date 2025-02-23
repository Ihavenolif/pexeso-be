using System;
using Fleck;
using Newtonsoft.Json;

class NetworkManager
{
    WebSocketServer server;
    public Dictionary<Guid, Client> clients { get; private set; } = new Dictionary<Guid, Client>();
    Menu menu = new Menu();

    public void CreateLobby(string name)
    {
        menu.CreateLobby(name);
    }

    public void DeleteLobby(int id)
    {
        menu.DeleteLobby(id);
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public static NetworkManager Singleton { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

    public NetworkManager(string location)
    {
        Singleton = this;
        server = new WebSocketServer(location);
        server.Start(socket =>
        {
            socket.OnOpen = () =>
            {
                clients[socket.ConnectionInfo.Id] = new Client(socket, menu);
            };

            socket.OnClose = () =>
            {
                clients.Remove(socket.ConnectionInfo.Id);
            };

            socket.OnMessage = message =>
            {
                Client fromClient = clients[socket.ConnectionInfo.Id];
                MessageReceiver received = JsonConvert.DeserializeObject<MessageReceiver>(message);

                switch (received.messageType)
                {
                    case MessageType.Ping:
                        socket.Send("Ping");
                        break;
                    default:
                        fromClient.HandleMessage(received);
                        break;
                }


            };

        });
    }


}
