class GetLobbiesResponse : Response
{
    public List<Lobby> Lobbies { get; set; }

    public GetLobbiesResponse(Dictionary<int, Lobby> lobbies)
    {
        this.Lobbies = lobbies.Values.ToList();
        this.messageType = MessageType.GetLobbies;
    }

}