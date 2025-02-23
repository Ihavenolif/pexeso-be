class CardClickedResponse : Response
{
    public int id { get; set; }
    public string name { get; set; }
    public WowClass wowClass { get; set; }
    public WowRole wowRole { get; set; }

    public CardClickedResponse(int id, string name, WowClass wowClass, WowRole wowRole)
    {
        this.id = id;
        this.name = name;
        this.wowClass = wowClass;
        this.wowRole = wowRole;
        this.messageType = MessageType.CardClicked;
    }
}