class GamePausedResponse : Response
{
    public bool paused { get; set; }

    public GamePausedResponse(bool paused)
    {
        this.paused = paused;
        this.messageType = MessageType.GamePaused;
    }
}