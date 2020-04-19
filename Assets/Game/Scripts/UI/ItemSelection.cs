using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamOatesGames.Systems;

public class ItemSelection : SubscribableMonoBehaviour
{
    public GameObject[] items;
    public GameObject selectedItem;

    private EventAggregator m_eventAggregator;

    private void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
    }

    public void WoodClicked()
    {
        Debug.Log("Clicked wood.");
        selectedItem = items[0];
        Publish(new SwapItemEvent() { item = selectedItem });
    }

    public void StoneClicked()
    {
        Debug.Log("Clicked stone.");
        selectedItem = items[1];
        Publish(new SwapItemEvent() { item = selectedItem });
    }

    public void MetalClicked()
    {
        Debug.Log("Clicked metal.");

        selectedItem = items[2];
        Publish(new SwapItemEvent() { item = selectedItem });
    }
}
