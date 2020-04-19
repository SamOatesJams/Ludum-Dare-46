using SamOatesGames.Systems;
using SamOatesGames.Systems.Core;
using UnityEngine.SceneManagement;

public class StartNewGameEvent : IEventAggregatorEvent { }

public class GameSession : UnitySingleton<GameSession>, ISubscribable
{
    private EventAggregator m_eventAggregator;
    private InventorySystem m_inventorySystem;

    public override void ResolveSystems()
    {
        base.ResolveSystems();
        m_eventAggregator = EventAggregator.GetInstance();
        m_inventorySystem = InventorySystem.GetInstance();
    }

    public void Start()
    {
        m_eventAggregator.Subscribe<StartNewGameEvent>(this, OnStartNewGameEvent);
    }

    public void OnDestroy()
    {
        if (m_eventAggregator != null)
        {
            m_eventAggregator.UnSubscribeAll(this);
        }
    }

    private void OnStartNewGameEvent(StartNewGameEvent args)
    {
        m_inventorySystem.ResetResources();
        SceneManager.LoadScene("Game Scene");
    }
}
