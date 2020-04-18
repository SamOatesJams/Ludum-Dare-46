using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SamOatesGames.Systems;
using System;

[RequireComponent(typeof(TMP_Text))]
public class InventoryItemLabel : SubscribableMonoBehaviour
{
    public ItemType ItemType;

    private InventorySystem m_inventorySystem;
    private EventAggregator m_eventAggregator;
    private TMP_Text m_text;

    public void Start()
    {
        m_text = GetComponent<TMP_Text>();
        m_inventorySystem = InventorySystem.GetInstance();
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<ItemPickupEvent>(this, UpdateItemLabel);
    }

    private void UpdateItemLabel(ItemPickupEvent e)
    {
        m_text.text = m_inventorySystem.GetItemAmount(ItemType).ToString();
    }
}
