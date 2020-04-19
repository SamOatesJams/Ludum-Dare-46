using SamOatesGames.Systems;

public class ResourcePickupEvent : IEventAggregatorEvent
{
    public ResourceType ResourceType { get; set; }
    public int Amount { get; set; }
}
