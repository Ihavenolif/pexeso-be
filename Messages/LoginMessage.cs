class LoginMessage : Message
{
    public string username { get; set; }

    public LoginMessage(string username)
    {
        this.username = username;
    }
}