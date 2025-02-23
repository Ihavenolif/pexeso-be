class Game : Instance
{
    public List<Client> clients { get; private set; }
    public List<Client> spectators { get; private set; }
    public List<Character?> characters { get; private set; }
    public int id { get; private set; }

    int playerOnTurn = 0;
    int? cardRevealed = null;

    public Game(List<Client> clients, List<Client> spectators, int id)
    {
        this.clients = clients;
        this.spectators = spectators;
        this.id = id;
        this.characters = InitializeGameChararcters()!;
        characters.AddRange(InitializeGameChararcters());
        Util.ShuffleList(characters);
    }

    public override void HandleMessage(Client fromClient, Message message)
    {
        switch (message)
        {
            case CardClickedMessage toggleReadyMessage:
                HandleCardClicked(fromClient, toggleReadyMessage);
                break;

        }
    }

    void HandleCardClicked(Client fromClient, CardClickedMessage cardClickedMessage)
    {
        if (clients[playerOnTurn] != fromClient)
        {
            return;
        }

        Character? character = characters[cardClickedMessage.id];

        if (character == null)
        {
            return;
        }

        if (cardRevealed == null)
        {
            cardRevealed = cardClickedMessage.id;
        }
        else
        {
            EvaluateTurn((int)cardRevealed, cardClickedMessage.id);
        }

        CardClickedResponse response = new CardClickedResponse(
            cardClickedMessage.id,
            character.name,
            character.wowClass,
            character.wowRole
        );

        foreach (Client client in clients)
        {
            client.Send(response);
        }

        foreach (Client client in spectators)
        {
            client.Send(response);
        }
    }

    void EvaluateTurn(int cardId1, int cardId2)
    {
        Character char1 = characters[cardId1]!;
        Character char2 = characters[cardId2]!;
        bool correctGuess = false;

        if (char1.id == char2.id)
        {
            clients[playerOnTurn].characters.Add(char1);
            characters[cardId1] = null;
            characters[cardId2] = null;
            correctGuess = true;
        }

        cardRevealed = null;
        this.playerOnTurn = (playerOnTurn + 1) % 2;

        TurnEvaluationResponse response = new TurnEvaluationResponse(correctGuess, clients);

        foreach (Client client in clients)
        {
            client.SendWithDelay(response, 2000);
        }

        foreach (Client client in spectators)
        {
            client.SendWithDelay(response, 2000);
        }
    }

    static List<Character> InitializeGameChararcters()
    {
        List<Character> list = new List<Character>();

        int characterId = 1;

        list.Add(new Character("Nolifeknight", WowClass.DeathKnight, WowRole.Tank, characterId++));
        list.Add(new Character("Zoromstitel", WowClass.Paladin, WowRole.Tank, characterId++));
        list.Add(new Character("Lanaes", WowClass.Druid, WowRole.Tank, characterId++));
        list.Add(new Character("DPH", WowClass.Warlock, WowRole.Tank, characterId++));
        list.Add(new Character("Nevrmore", WowClass.Hunter, WowRole.Tank, characterId++));
        list.Add(new Character("Mia", WowClass.Warrior, WowRole.Tank, characterId++));
        list.Add(new Character("Littlechilla", WowClass.Shaman, WowRole.Tank, characterId++));
        list.Add(new Character("Sussile", WowClass.Monk, WowRole.Tank, characterId++));
        list.Add(new Character("Dralf", WowClass.Druid, WowRole.Tank, characterId++));
        list.Add(new Character("Polichan", WowClass.Paladin, WowRole.Tank, characterId++));
        list.Add(new Character("Jeffgoldblum", WowClass.Warrior, WowRole.Tank, characterId++));
        list.Add(new Character("Rastyrose", WowClass.Paladin, WowRole.Tank, characterId++));
        list.Add(new Character("Icansheepyou", WowClass.Mage, WowRole.Tank, characterId++));
        list.Add(new Character("Shadowbtw", WowClass.Mage, WowRole.Tank, characterId++));
        list.Add(new Character("Alied", WowClass.DemonHunter, WowRole.Tank, characterId++));
        list.Add(new Character("Myrrelw", WowClass.Warlock, WowRole.Tank, characterId++));
        list.Add(new Character("Egbanek", WowClass.Evoker, WowRole.Tank, characterId++));
        list.Add(new Character("Dádulák", WowClass.DemonHunter, WowRole.Tank, characterId++));
        list.Add(new Character("Remath", WowClass.Shaman, WowRole.Tank, characterId++));
        list.Add(new Character("Olgrin", WowClass.Hunter, WowRole.Tank, characterId++));
        list.Add(new Character("Kelthrian", WowClass.DeathKnight, WowRole.Tank, characterId++));
        list.Add(new Character("Thekropicka", WowClass.DeathKnight, WowRole.Tank, characterId++));
        list.Add(new Character("Julka", WowClass.Hunter, WowRole.Tank, characterId++));
        list.Add(new Character("Toluus", WowClass.Shaman, WowRole.Tank, characterId++));

        return list;
    }
}