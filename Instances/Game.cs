class Game : Instance
{
    public List<Client> clients { get; private set; }
    public List<Client> spectators { get; private set; }
    public List<Character?> characters { get; private set; }
    public int id { get; private set; }

    int totalHealerCount;
    int totalDpsCount;

    bool allHealersPicked = false;
    bool allDpsPicked = false;

    bool pickingLocked = false;
    bool paused = false;

    int playerOnTurn = 0;
    int? cardRevealed = null;

    public Game(List<Client> clients, List<Client> spectators, int id)
    {
        this.clients = clients;
        this.spectators = spectators;
        this.id = id;
        this.characters = InitializeGameChararcters()!;

        this.totalHealerCount = this.characters.FindAll(character => character!.wowRole == WowRole.Healer).Count();
        this.totalDpsCount = this.characters.FindAll(character => character!.wowRole == WowRole.DPS).Count();

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
            case GamePausedMessage gamePausedMessage:
                HandleGamePaused(fromClient, gamePausedMessage);
                break;

        }
    }

    void HandleCardClicked(Client fromClient, CardClickedMessage cardClickedMessage)
    {
        if (pickingLocked || paused)
        {
            return;
        }

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
            if (cardRevealed == cardClickedMessage.id) return;
            pickingLocked = true;
            Task.Run(async () =>
            {
                await EvaluateTurn((int)cardRevealed, cardClickedMessage.id);
            });
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

    void HandleGamePaused(Client fromClient, GamePausedMessage gamePausedMessage)
    {
        if (!spectators.Contains(fromClient))
        {
            return;
        }

        paused = !paused;

        Response response = new GamePausedResponse(paused);

        foreach (Client client in clients)
        {
            client.Send(response);
        }

        foreach (Client client in spectators)
        {
            client.Send(response);
        }
    }

    async Task EvaluateTanks()
    {
        int player1TankCount = clients[0].characters.FindAll(character => character.wowRole == WowRole.Tank).Count;
        int player2TankCount = clients[1].characters.FindAll(character => character.wowRole == WowRole.Tank).Count;

        if (player1TankCount == 2 && player2TankCount == 2)
        {
            return;
        }

        List<Response> responses = new List<Response>();

        if (player1TankCount == 2)
        {
            List<Character> remainingTanks = new List<Character>();
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null && characters[i]!.wowRole == WowRole.Tank)
                {
                    remainingTanks.Add(characters[i]!);
                    responses.Add(new CardClickedResponse(i, characters[i]!.name, characters[i]!.wowClass, characters[i]!.wowRole));
                    characters[i] = null;
                }
            }

            HashSet<string> addedCharacters = new HashSet<string>();
            remainingTanks.ForEach(character =>
            {
                if (!addedCharacters.Contains(character.name))
                {
                    addedCharacters.Add(character.name);
                    clients[1].characters.Add(character);
                }
            });
        }
        else if (player2TankCount == 2)
        {
            List<Character> remainingTanks = new List<Character>();
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null && characters[i]!.wowRole == WowRole.Tank)
                {
                    remainingTanks.Add(characters[i]!);
                    responses.Add(new CardClickedResponse(i, characters[i]!.name, characters[i]!.wowClass, characters[i]!.wowRole));
                    characters[i] = null;
                }
            }

            HashSet<string> addedCharacters = new HashSet<string>();
            remainingTanks.ForEach(character =>
            {
                if (!addedCharacters.Contains(character.name))
                {
                    addedCharacters.Add(character.name);
                    clients[0].characters.Add(character);
                }
            });
        }

        if (responses.Count > 0)
        {
            await Task.Delay(1500);

            foreach (Client client in clients)
            {
                foreach (Response response in responses)
                {
                    client.Send(response);
                }
            }

            foreach (Client client in spectators)
            {
                foreach (Response response in responses)
                {
                    client.Send(response);
                }
            }
        }
    }

    async Task EvaluateHealers()
    {
        int player1HealerCount = clients[0].characters.FindAll(character => character.wowRole == WowRole.Healer).Count;
        int player2HealerCount = clients[1].characters.FindAll(character => character.wowRole == WowRole.Healer).Count;

        int healerThreshold = (int)Math.Ceiling((float)totalHealerCount / (float)2);

        if (allHealersPicked)
        {
            return;
        }

        List<Response> responses = new List<Response>();

        if (player1HealerCount == healerThreshold)
        {
            List<Character> remainingHealers = new List<Character>();
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null && characters[i]!.wowRole == WowRole.Healer)
                {
                    remainingHealers.Add(characters[i]!);
                    responses.Add(new CardClickedResponse(i, characters[i]!.name, characters[i]!.wowClass, characters[i]!.wowRole));
                    characters[i] = null;
                }
            }

            HashSet<string> addedCharacters = new HashSet<string>();
            remainingHealers.ForEach(character =>
            {
                if (!addedCharacters.Contains(character.name))
                {
                    addedCharacters.Add(character.name);
                    clients[1].characters.Add(character);
                }
            });
            allHealersPicked = true;
        }
        else if (player2HealerCount == healerThreshold)
        {
            List<Character> remainingHealers = new List<Character>();
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null && characters[i]!.wowRole == WowRole.Healer)
                {
                    remainingHealers.Add(characters[i]!);
                    responses.Add(new CardClickedResponse(i, characters[i]!.name, characters[i]!.wowClass, characters[i]!.wowRole));
                    characters[i] = null;
                }
            }

            HashSet<string> addedCharacters = new HashSet<string>();
            remainingHealers.ForEach(character =>
            {
                if (!addedCharacters.Contains(character.name))
                {
                    addedCharacters.Add(character.name);
                    clients[0].characters.Add(character);
                }
            });
            allHealersPicked = true;
        }

        if (responses.Count > 0)
        {
            await Task.Delay(1500);

            foreach (Client client in clients)
            {
                foreach (Response response in responses)
                {
                    client.Send(response);
                }
            }

            foreach (Client client in spectators)
            {
                foreach (Response response in responses)
                {
                    client.Send(response);
                }
            }
        }
    }

    async Task EvaluateDPS()
    {
        int player1DPSCount = clients[0].characters.FindAll(character => character.wowRole == WowRole.DPS).Count;
        int player2DPSCount = clients[1].characters.FindAll(character => character.wowRole == WowRole.DPS).Count;

        int DPSThreshold = (int)Math.Ceiling((float)totalDpsCount / (float)2);

        if (allDpsPicked)
        {
            return;
        }

        List<Response> responses = new List<Response>();

        if (player1DPSCount == DPSThreshold)
        {
            List<Character> remainingDPS = new List<Character>();
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null && characters[i]!.wowRole == WowRole.DPS)
                {
                    remainingDPS.Add(characters[i]!);
                    responses.Add(new CardClickedResponse(i, characters[i]!.name, characters[i]!.wowClass, characters[i]!.wowRole));
                    characters[i] = null;
                }
            }

            HashSet<string> addedCharacters = new HashSet<string>();
            remainingDPS.ForEach(character =>
            {
                if (!addedCharacters.Contains(character.name))
                {
                    addedCharacters.Add(character.name);
                    clients[1].characters.Add(character);
                }
            });
            allDpsPicked = true;
        }
        else if (player2DPSCount == DPSThreshold)
        {
            List<Character> remainingDPS = new List<Character>();
            for (int i = 0; i < characters.Count; i++)
            {
                if (characters[i] != null && characters[i]!.wowRole == WowRole.DPS)
                {
                    remainingDPS.Add(characters[i]!);
                    responses.Add(new CardClickedResponse(i, characters[i]!.name, characters[i]!.wowClass, characters[i]!.wowRole));
                    characters[i] = null;
                }
            }

            HashSet<string> addedCharacters = new HashSet<string>();
            remainingDPS.ForEach(character =>
            {
                if (!addedCharacters.Contains(character.name))
                {
                    addedCharacters.Add(character.name);
                    clients[0].characters.Add(character);
                }
            });
            allDpsPicked = true;
        }

        if (responses.Count > 0)
        {
            await Task.Delay(1500);

            foreach (Client client in clients)
            {
                foreach (Response response in responses)
                {
                    client.Send(response);
                }
            }

            foreach (Client client in spectators)
            {
                foreach (Response response in responses)
                {
                    client.Send(response);
                }
            }
        }
    }

    async Task EvaluateTurn(int cardId1, int cardId2)
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

            await EvaluateTanks();
            await EvaluateHealers();
            await EvaluateDPS();

        }

        cardRevealed = null;
        this.playerOnTurn = (playerOnTurn + 1) % 2;

        TurnEvaluationResponse response = new TurnEvaluationResponse(correctGuess, clients, playerOnTurn);

        await Task.Delay(1500);

        foreach (Client client in clients)
        {
            client.Send(response);
        }

        foreach (Client client in spectators)
        {
            client.Send(response);
        }

        pickingLocked = false;
    }

    static List<Character> InitializeGameChararcters()
    {
        List<Character> list = new List<Character>();

        int characterId = 1;

        list.Add(new Character("Mandragee", WowClass.Evoker, WowRole.DPS, characterId++));
        list.Add(new Character("Krenius", WowClass.Priest, WowRole.Healer, characterId++));
        list.Add(new Character("Ihavenolight", WowClass.Priest, WowRole.Healer, characterId++));
        list.Add(new Character("Zoromstitel", WowClass.Paladin, WowRole.DPS, characterId++));
        list.Add(new Character("Lanaes", WowClass.Druid, WowRole.Healer, characterId++));
        list.Add(new Character("Littlechilla", WowClass.Shaman, WowRole.Healer, characterId++));
        list.Add(new Character("Sussile", WowClass.Monk, WowRole.Tank, characterId++));
        list.Add(new Character("Dralf", WowClass.Druid, WowRole.DPS, characterId++));
        list.Add(new Character("Alied", WowClass.DemonHunter, WowRole.DPS, characterId++));
        list.Add(new Character("Olgrin", WowClass.Hunter, WowRole.DPS, characterId++));
        list.Add(new Character("Julka", WowClass.Hunter, WowRole.DPS, characterId++));
        list.Add(new Character("Shadowbtw", WowClass.Mage, WowRole.DPS, characterId++));
        list.Add(new Character("Icansheepyou", WowClass.Mage, WowRole.DPS, characterId++));
        list.Add(new Character("Sumoj", WowClass.Monk, WowRole.Healer, characterId++));
        list.Add(new Character("Mia", WowClass.Warrior, WowRole.DPS, characterId++));
        list.Add(new Character("Nevrmore", WowClass.Hunter, WowRole.DPS, characterId++));
        list.Add(new Character("Thekropicka", WowClass.DeathKnight, WowRole.DPS, characterId++));
        list.Add(new Character("Jeffgoldblum", WowClass.Warrior, WowRole.Tank, characterId++));
        list.Add(new Character("Rastyrose", WowClass.Paladin, WowRole.DPS, characterId++));
        list.Add(new Character("Toluus", WowClass.Shaman, WowRole.DPS, characterId++));
        list.Add(new Character("Kormios", WowClass.Evoker, WowRole.Healer, characterId++));
        list.Add(new Character("Fudko", WowClass.Monk, WowRole.Healer, characterId++));
        list.Add(new Character("Dádulák", WowClass.DemonHunter, WowRole.Tank, characterId++));
        list.Add(new Character("Kelthrian", WowClass.DeathKnight, WowRole.DPS, characterId++));
        list.Add(new Character("Nershu", WowClass.Shaman, WowRole.DPS, characterId++));
        list.Add(new Character("Remath", WowClass.Shaman, WowRole.DPS, characterId++));
        list.Add(new Character("Verifuni", WowClass.Priest, WowRole.DPS, characterId++));
        list.Add(new Character("Zořice", WowClass.Druid, WowRole.DPS, characterId++));
        list.Add(new Character("Maego", WowClass.Evoker, WowRole.DPS, characterId++));
        list.Add(new Character("Kaprdh", WowClass.DemonHunter, WowRole.Tank, characterId++));
        list.Add(new Character("Rolash", WowClass.DeathKnight, WowRole.DPS, characterId++));
        list.Add(new Character("Yungchrist", WowClass.Monk, WowRole.DPS, characterId++));


        return list;
    }
}