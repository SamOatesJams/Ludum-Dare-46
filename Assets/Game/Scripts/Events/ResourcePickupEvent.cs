using SamOatesGames.Systems;

public class ResourcePickupEvent : IEventAggregatorEvent
{
    public ResourceType ResourceType { get; }
    public int PickupAmount { get; }
    public int TotalAmount { get; }

    public ResourcePickupEvent(ResourceType resourceType, int pickupAmount, int totalAmount)
    {
        ResourceType = resourceType;
        PickupAmount = pickupAmount;
        TotalAmount = totalAmount;
    }
}
