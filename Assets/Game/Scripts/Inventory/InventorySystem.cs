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

    public void Start()
    {
        foreach (ResourceType resourceType in Enum.GetValues(typeof(ResourceType)))
        {
            m_resourceCounts[resourceType] = 0;
        }
    }

    public void OnDestroy()
    {
        if (m_eventAggregator != null)
        {
            m_eventAggregator.UnSubscribeAll(this);
        }
    }

    public void AddItem(ResourceType type, int amount)
    {
        m_resourceCounts[type] += amount;
        m_eventAggregator.Publish(new ResourcePickupEvent { ResourceType = type, Amount = amount });
    }

    public int GetItemAmount(ResourceType type)
    {
        return m_resourceCounts[type];
    }
}
