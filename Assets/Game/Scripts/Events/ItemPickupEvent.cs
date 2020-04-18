using SamOatesGames.Systems;

public class ItemPickupEvent : IEventAggregatorEvent
{
    public ItemType ItemType { get; set; }
    public int Amount { get; set; }
}
