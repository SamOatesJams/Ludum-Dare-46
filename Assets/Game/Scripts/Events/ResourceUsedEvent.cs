using SamOatesGames.Systems;

public class ResourceUsedEvent : IEventAggregatorEvent
{
    public ResourceType ResourceType { get; }
    public int PickupAmount { get; }
    public int TotalAmount { get; }

    public ResourceUsedEvent(ResourceType resourceType, int pickupAmount, int totalAmount)
    {
        ResourceType = resourceType;
        PickupAmount = pickupAmount;
        TotalAmount = totalAmount;
    }
}
