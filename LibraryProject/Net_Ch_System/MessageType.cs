namespace Net_Ch_System
{
    public enum MessageType
    {
        ServerToClientHS = -4,
        ClientToServerHS = -3,
        ClientToMatchMakerHS = -2,
        MatchMakerToClientHS = -1,
        Ping = 0,
        Console = 1,
        Position = 2,
        PlayerList = 3
    }
}