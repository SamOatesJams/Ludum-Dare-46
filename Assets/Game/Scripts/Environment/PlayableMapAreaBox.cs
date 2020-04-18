using SamOatesGames.Systems;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayableMapAreaBox : MonoBehaviour
{
    public void Start()
    {
        var mapBounds = GetComponent<BoxCollider2D>().bounds;

        var eventAggregator = EventAggregator.GetInstance();
        eventAggregator.Publish(new SetCameraPlayableBoundsEvent(mapBounds));
    }
}
