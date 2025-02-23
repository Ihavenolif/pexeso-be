class StartGameResponse : Response
{
    public int cardCount;

    public StartGameResponse(int cardCount)
    {
        this.cardCount = cardCount;
        this.messageType = MessageType.StartGame;
    }
}