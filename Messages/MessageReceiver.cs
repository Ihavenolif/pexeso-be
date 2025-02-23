struct MessageReceiver
{
    public MessageReceiver(MessageType messageType, string data)
    {
        this.messageType = messageType;
        this.data = data;
    }

    public MessageType messageType;
    public string data;


}