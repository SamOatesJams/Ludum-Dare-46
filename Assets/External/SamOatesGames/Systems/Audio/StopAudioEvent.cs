using SamOatesGames.Systems;

public class StopAudioEvent : IEventAggregatorEvent
{
    public int AudioId { get; }

    public StopAudioEvent(int audioId)
    {
        AudioId = audioId;
    }
}
