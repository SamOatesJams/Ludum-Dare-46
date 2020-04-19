using SamOatesGames.Systems;

public class StartWaveEvent : IEventAggregatorEvent
{
    public int Wave { get; }

    public StartWaveEvent(int wave)
    {
        Wave = wave;
    }
}
