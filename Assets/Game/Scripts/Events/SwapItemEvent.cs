using SamOatesGames.Systems;
using UnityEngine;

public class SwapItemEvent : IEventAggregatorEvent
{
    public GameObject item;
    public ItemSelection itemSelection { get; set; }
}

