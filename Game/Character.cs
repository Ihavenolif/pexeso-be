class Character
{
    public string name;
    public WowClass wowClass;
    public WowRole wowRole;
    public int id;

    public Character(string name, WowClass wowClass, WowRole wowRole, int id)
    {
        this.name = name;
        this.wowClass = wowClass;
        this.wowRole = wowRole;
        this.id = id;
    }
}