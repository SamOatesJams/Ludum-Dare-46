using System;
using System.Collections.Generic;
using SamOatesGames.Systems;
using SamOatesGames.Systems.Core;

public class InventorySystem : UnitySingleton<InventorySystem>, ISubscribable
{
    private readonly Dictionary<ResourceType, int> m_resourceCounts = new Dictionary<ResourceType, int>();

    private EventAggregator m_eventAggregator;

    public override void ResolveSystems()
    {
        base.ResolveSystems();
        m_eventAggregator = EventAggregator.GetInstance();
    }

    protected override void Awake()
    {
        base.Awake();
        ResetResources();
    }

    public void OnDestroy()
    {
        if (m_eventAggregator != null)
        {
            m_eventAggregator.UnSubscribeAll(this);
        }
    }

    public void ResetResources()
    {
        m_resourceCounts[ResourceType.Wood] = 10;
        m_resourceCounts[ResourceType.Stone] = 10;
        m_resourceCounts[ResourceType.Metal] = 10;
        m_resourceCounts[ResourceType.Currency] = 0;
    }

    public void AddItem(ResourceType type, int amount)
    {
        m_resourceCounts[type] += amount;
        m_eventAggregator.Publish(new ResourcePickupEvent(type, amount, m_resourceCounts[type]));
    }

    public void UseItem(ResourceType type, int amount)
    {
        m_resourceCounts[type] -= amount;
        m_eventAggregator.Publish(new ResourceUsedEvent(type, amount, m_resourceCounts[type]));
    }

    public int GetItemAmount(ResourceType type)
    {
        return m_resourceCounts[type];
    }
}
