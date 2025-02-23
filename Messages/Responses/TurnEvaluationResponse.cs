class TurnEvaluationResponse : Response
{
    public bool wasGuessCorrect { get; set; }
    public List<Client> Clients { get; set; }
    public int onTurn { get; set; }

    public TurnEvaluationResponse(bool wasGuessCorrect, List<Client> clients, int onTurn)
    {
        this.wasGuessCorrect = wasGuessCorrect;
        this.Clients = clients;
        this.onTurn = onTurn;
        this.messageType = MessageType.TurnEvaluation;
    }
}