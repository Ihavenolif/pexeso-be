class ErrorResponse : Response
{
    public string data;

    public ErrorResponse(string data)
    {
        this.data = data;
        this.messageType = MessageType.Error;
    }
}