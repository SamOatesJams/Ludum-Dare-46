using SamOatesGames.Systems;
using UnityEngine;

public class SwapItemEvent : IEventAggregatorEvent
{
    public ItemDescription ItemDescription { get; }

    public SwapItemEvent(ItemDescription itemDescription)
    {
        ItemDescription = itemDescription;
    }
}

