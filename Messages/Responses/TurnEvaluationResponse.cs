class TurnEvaluationResponse : Response
{
    public bool wasGuessCorrect { get; set; }
    public List<Client> Clients { get; set; }

    public TurnEvaluationResponse(bool wasGuessCorrect, List<Client> clients)
    {
        this.wasGuessCorrect = wasGuessCorrect;
        this.Clients = clients;
        this.messageType = MessageType.TurnEvaluation;
    }
}