﻿using SamOatesGames.Systems;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class InventoryItemLabel : SubscribableMonoBehaviour
{
    public ResourceType ResourceType;

    private EventAggregator m_eventAggregator;
    private TMP_Text m_text;

    public void Start()
    {
        m_text = GetComponent<TMP_Text>();
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<ResourcePickupEvent>(this, OnResourcePickupEvent);
        UpdateItemLabel(InventorySystem.GetInstance().GetItemAmount(ResourceType));
    }

    private void OnResourcePickupEvent(ResourcePickupEvent e)
    {
        UpdateItemLabel(e.TotalAmount);
    }

    private void UpdateItemLabel(int amount)
    {
        m_text.text = amount.ToString();
    }
}
