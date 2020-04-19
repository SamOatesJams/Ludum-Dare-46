using SamOatesGames.Systems;
using UnityEngine;

public class SwapItemEvent : IEventAggregatorEvent
{
    public PlaceableItem Item { get; }
    public ItemSelection ItemSelection { get; }

    public SwapItemEvent(PlaceableItem item, ItemSelection itemSelection)
    {
        Item = item;
        ItemSelection = itemSelection;
    }
}

