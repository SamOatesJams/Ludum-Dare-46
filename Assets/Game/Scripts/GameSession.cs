using SamOatesGames.Systems;
using SamOatesGames.Systems.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartNewGameEvent : IEventAggregatorEvent { }

public class GameSession : UnitySingleton<GameSession>, ISubscribable
{
    public enum GameStage
    {
        Daytime,
        Nighttime,
        GameOver
    }

    public GameStage Stage { get; private set; }

    public float StageLength { get; } = 60.0f;

    private EventAggregator m_eventAggregator;
    private InventorySystem m_inventorySystem;

    private float m_stageStartTime;

    public override void ResolveSystems()
    {
        base.ResolveSystems();
        m_eventAggregator = EventAggregator.GetInstance();
        m_inventorySystem = InventorySystem.GetInstance();
    }

    public void Start()
    {
        Stage = GameStage.Daytime;
        m_stageStartTime = Time.time;

        m_eventAggregator.Subscribe<StartNewGameEvent>(this, OnStartNewGameEvent);
        m_eventAggregator.Subscribe<StageTimeOverEvent>(this, OnStageTimeOverEvent);

        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);
        m_eventAggregator.Subscribe<RequestNighttimeEvent>(this, OnRequestNighttimeEvent);

        m_eventAggregator.Subscribe<NavigationCompleteEvent>(this, OnNavigationCompleteEvent);
    }

    public void OnDestroy()
    {
        if (m_eventAggregator != null)
        {
            m_eventAggregator.UnSubscribeAll(this);
        }
    }

    private void FixedUpdate()
    {
        if (GetStageProgress() >= 1.0f)
        {
            m_eventAggregator.Publish(new StageTimeOverEvent());
        }
    }

    public float GetStageProgress()
    {
        var currentTime = Time.time - m_stageStartTime;
        var progress = Mathf.Max(currentTime / StageLength, 0.0f);
        return progress;
    }

    private void OnStageTimeOverEvent(StageTimeOverEvent args)
    {
        switch (Stage)
        {
            case GameStage.Daytime:
                m_eventAggregator.Publish(new RequestNighttimeEvent());
                break;
            case GameStage.Nighttime:
                m_eventAggregator.Publish(new RequestDaytimeEvent());
                break;
        }
    }

    private void OnNavigationCompleteEvent(NavigationCompleteEvent args)
    {
        if (Stage == GameStage.GameOver)
        {
            return;
        }

        var enemy = args.GameObject.GetComponent<EnemyController>();
        if (enemy == null)
        {
            return;
        }

        if (enemy.State == EnemyController.EnemyState.Attacking)
        {
            Stage = GameStage.GameOver;
            m_eventAggregator.Publish(new GameOverEvent());
        }
    }

    private void OnStartNewGameEvent(StartNewGameEvent args)
    {
        Stage = GameStage.Daytime;
        m_stageStartTime = Time.time;
        SceneManager.LoadScene("Game Scene");
    }

    private void OnRequestDaytimeEvent(RequestDaytimeEvent args)
    {
        Stage = GameStage.Daytime;
        m_stageStartTime = Time.time;
    }

    private void OnRequestNighttimeEvent(RequestNighttimeEvent args)
    {
        Stage = GameStage.Nighttime;
        m_stageStartTime = Time.time + 5.0f;
    }
}
