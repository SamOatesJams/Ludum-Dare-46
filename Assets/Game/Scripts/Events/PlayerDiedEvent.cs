using SamOatesGames.Systems;

public class PlayerDiedEvent : IEventAggregatorEvent
{
    public PlayerController Player { get; }

    public PlayerDiedEvent(PlayerController player)
    {
        Player = player;
    }
}
