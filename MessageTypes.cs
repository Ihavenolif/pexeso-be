enum MessageType : ushort
{
    Ping = 0,
    GetLobbies,
    CustomData,
    Error,
    JoinLobby,
    LeaveLobby,
    Login,
    ToggleReady,
    JoinAsSpectator,
    StartGame,
    CardClicked,
    TurnEvaluation,
    GamePaused
}