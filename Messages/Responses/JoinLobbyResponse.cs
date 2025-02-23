class JoinLobbyResponse : Response
{
    public List<Client> Clients;
    public List<Client> Spectators;
    public int id;
    public string name;

    public JoinLobbyResponse(Lobby lobby)
    {
        this.Clients = lobby.clients;
        this.Spectators = lobby.spectators;
        this.id = lobby.id;
        this.name = lobby.name;
        this.messageType = MessageType.JoinLobby;
    }

}