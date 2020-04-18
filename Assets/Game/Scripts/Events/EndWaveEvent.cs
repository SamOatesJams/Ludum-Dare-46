using SamOatesGames.Systems;

public class EndWaveEvent : IEventAggregatorEvent
{
    public int Wave { get; set; }
    public int EnemiesKilled { get; set; }
}
