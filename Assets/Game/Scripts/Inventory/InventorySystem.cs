using System.Collections.Generic;
using SamOatesGames.Systems;
using SamOatesGames.Systems.Core;

public class InventorySystem : UnitySingleton<InventorySystem>, ISubscribable
{
    private List<int> m_itemCounts = new List<int>(4) { 0, 0, 0, 0 };

    private EventAggregator m_eventAggregator;

    public override void ResolveSystems()
    {
        base.ResolveSystems();
        m_eventAggregator = EventAggregator.GetInstance();
    }

    public void OnDestroy()
    {
        if (m_eventAggregator != null)
        {
            m_eventAggregator.UnSubscribeAll(this);
        }
    }

    public void AddItem(ItemType type, int amount)
    {
        m_itemCounts[(int)type] += amount;
        m_eventAggregator.Publish(new ItemPickupEvent { ItemType = type, Amount = amount });
    }

    public int GetItemAmount(ItemType type)
    {
        return m_itemCounts[(int)type];
    }
}
