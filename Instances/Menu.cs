class Menu : Instance
{
    //List<Lobby> lobbies = new List<Lobby>();
    Dictionary<int, Lobby> lobbies = new Dictionary<int, Lobby>();

    public void CreateLobby(string name)
    {
        Lobby lobby = new Lobby(name);
        lobbies.Add(lobby.id, lobby);
    }

    public override void HandleMessage(Client fromClient, Message message)
    {
        switch (message)
        {
            case GetLobbiesMessage getLobbiesMessage:
                GetLobbiesResponse response = new GetLobbiesResponse(lobbies);
                fromClient.Send(response);
                break;
            case CustomDataMessage customDataMessage:
                Console.WriteLine(customDataMessage.data);
                break;
            case JoinLobbyMessage joinLobbyMessage:
                HandleJoinLobby(fromClient, joinLobbyMessage);
                break;
            case LoginMessage loginMessage:
                HandleLogin(fromClient, loginMessage);
                break;
            case JoinAsSpectatorMessage joinAsSpectatorMessage:
                HandleJoinAsSpectator(fromClient, joinAsSpectatorMessage);
                break;

        }
        Console.WriteLine($"Received: {message}");
    }

    void HandleLogin(Client fromClient, LoginMessage loginMessage)
    {
        fromClient.name = loginMessage.username;

        LoginResponse loginResponse = new LoginResponse(fromClient.clientId, fromClient.name);
        fromClient.Send(loginResponse);
    }

    void HandleJoinLobby(Client fromClient, JoinLobbyMessage joinLobbyMessage)
    {

        if (!lobbies.ContainsKey(joinLobbyMessage.lobbyId))
        {
            fromClient.Send(new ErrorResponse("Lobby not found"));
            return;
        }

        Lobby lobby = lobbies[joinLobbyMessage.lobbyId];

        if (lobby.playerCount >= 2)
        {
            fromClient.Send(new ErrorResponse("Lobby is full"));
            return;
        }

        lobby.AddClient(fromClient);
        fromClient.instance = lobby;

        JoinLobbyResponse response = new JoinLobbyResponse(lobby);
        foreach (Client client in lobby.clients)
        {
            client.Send(response);
        }

        foreach (Client client in lobby.spectators)
        {
            client.Send(response);
        }
    }

    void HandleJoinAsSpectator(Client fromClient, JoinAsSpectatorMessage joinAsSpectatorMessage)
    {
        if (!lobbies.ContainsKey(joinAsSpectatorMessage.lobbyId))
        {
            fromClient.Send(new ErrorResponse("Lobby not found"));
            return;
        }

        Lobby lobby = lobbies[joinAsSpectatorMessage.lobbyId];

        lobby.AddSpectator(fromClient);
        fromClient.instance = lobby;

        JoinLobbyResponse response = new JoinLobbyResponse(lobby);
        foreach (Client client in lobby.clients)
        {
            client.Send(response);
        }

        foreach (Client client in lobby.spectators)
        {
            client.Send(response);
        }
    }
}