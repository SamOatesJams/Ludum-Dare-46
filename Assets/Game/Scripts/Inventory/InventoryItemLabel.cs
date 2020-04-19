using SamOatesGames.Systems;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class InventoryItemLabel : SubscribableMonoBehaviour
{
    public ResourceType ResourceType;

    private InventorySystem m_inventorySystem;
    private EventAggregator m_eventAggregator;
    private TMP_Text m_text;

    public void Start()
    {
        m_text = GetComponent<TMP_Text>();
        m_inventorySystem = InventorySystem.GetInstance();
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<ResourcePickupEvent>(this, UpdateItemLabel);
    }

    private void UpdateItemLabel(ResourcePickupEvent e)
    {
        m_text.text = m_inventorySystem.GetItemAmount(ResourceType).ToString();
    }
}
