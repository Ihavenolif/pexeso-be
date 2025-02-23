class ToggleReadyResponse : Response
{
    public int clientId;
    public bool isReady;

    public ToggleReadyResponse(int clientId, bool isReady)
    {
        this.clientId = clientId;
        this.isReady = isReady;
        this.messageType = MessageType.ToggleReady;
    }
}