using SamOatesGames.Systems;

public class StartWaveEvent : IEventAggregatorEvent
{
    public int Wave { get; set; }
}
