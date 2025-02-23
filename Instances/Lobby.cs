class Lobby : Instance
{
    static int lobbyCounter = 0;

    public int id { get; private set; }
    public string name { get; private set; }
    public int playerCount { get; private set; } = 0;
    public List<Client> clients { get; private set; } = new List<Client>();
    public List<Client> spectators { get; private set; } = new List<Client>();

    public Lobby(string name)
    {
        this.name = name;
        this.id = lobbyCounter++;
    }

    public override void HandleMessage(Client fromClient, Message message)
    {
        switch (message)
        {
            case ToggleReadyMessage toggleReadyMessage:
                HandleToggleReady(fromClient, toggleReadyMessage);
                break;
            case StartGameMessage startGameMessage:
                HandleStartGame(fromClient, startGameMessage);
                break;

        }
    }

    public void AddClient(Client client)
    {
        this.clients.Add(client);
        this.playerCount++;
    }

    public void AddSpectator(Client client)
    {
        this.spectators.Add(client);
    }

    void HandleToggleReady(Client fromClient, ToggleReadyMessage toggleReadyMessage)
    {
        fromClient.isReady = !fromClient.isReady;
        ToggleReadyResponse response = new ToggleReadyResponse(fromClient.clientId, fromClient.isReady);

        foreach (Client client in clients)
        {
            client.Send(response);
        }
        foreach (Client client in spectators)
        {
            client.Send(response);
        }
    }

    void HandleStartGame(Client fromClient, StartGameMessage gameMessage)
    {
        bool allReady = true;

        foreach (Client client in clients)
        {
            if (!client.isReady)
            {
                allReady = false;
            }
        }

        if (!allReady) return;

        int lobbyId = ((Lobby)fromClient.instance).id;
        Game game = new Game(clients, spectators, this.id);

        foreach (Client client in clients)
        {
            client.instance = game;
        }

        foreach (Client client in spectators)
        {
            client.instance = game;
        }

        StartGameResponse response = new StartGameResponse(game.characters!.Count());

        foreach (Client client in clients)
        {
            client.Send(response);
        }

        foreach (Client client in spectators)
        {
            client.Send(response);
        }

        NetworkManager.Singleton.DeleteLobby(lobbyId);
    }
}