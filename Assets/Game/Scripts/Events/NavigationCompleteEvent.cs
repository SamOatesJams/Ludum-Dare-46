using SamOatesGames.Systems;
using UnityEngine;

public class NavigationCompleteEvent : IEventAggregatorEvent
{
    public NavMovementController NavMovementController { get; }
    public GameObject GameObject { get; }

    public NavigationCompleteEvent(NavMovementController navController, GameObject gameObject)
    {
        NavMovementController = navController;
        GameObject = gameObject;
    }
}
