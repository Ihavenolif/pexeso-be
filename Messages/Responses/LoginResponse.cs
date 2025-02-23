class LoginResponse : Response
{
    public int clientId { get; private set; }
    public string username { get; private set; }


    public LoginResponse(int clientId, string username)
    {
        this.clientId = clientId;
        this.username = username;
        this.messageType = MessageType.Login;
    }

}