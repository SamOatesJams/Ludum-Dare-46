using System.Collections;
using SamOatesGames.Systems;
using UnityEngine;

[RequireComponent(typeof(NavMovementController))]
public class EnemyController : SubscribableMonoBehaviour
{
    public enum EnemyState
    {
        Attacking,
        RunningAway,
        Dead
    }

    public double Health { get; private set; }
    public EnemyState State => m_state;

    public double MaxHealth = 20.0;
    public GameObject ShadowBlob;
    
    private Vector3? m_target;
    private Vector3 m_spawnPoint;
    private EventAggregator m_eventAggregator;
    private NavMovementController m_movementController;
    private EnemyState m_state = EnemyState.Attacking;

    void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);
        m_eventAggregator.Subscribe<NavigationCompleteEvent>(this, OnNavigationCompleteEvent);
        m_eventAggregator.Subscribe<EnemyDeathEvent>(this, OnEnemyDeathEvent);

        m_movementController = GetComponent<NavMovementController>();

        Health = MaxHealth;
        m_spawnPoint = transform.position;

        if (m_target != null)
        {
            m_movementController.NavigateTo(m_target.Value);
        }
    }

    private void OnEnemyDeathEvent(EnemyDeathEvent args)
    {
        if (args.Enemy != this)
        {
            return;
        }

        m_state = EnemyState.Dead;
        StartCoroutine(DestroyDeadPlayer());
    }

    private IEnumerator DestroyDeadPlayer()
    {
        yield return new WaitForSecondsRealtime(10.0f);

        var spriteRenderer = GetComponent<SpriteRenderer>();
        var startColor = spriteRenderer.color;
        var endColor = Color.clear;

        var time = 0.0f;
        while (time <= 1.0f)
        {
            var value = Color.Lerp(startColor, endColor, time);
            spriteRenderer.color = value;

            time += 0.01f;
            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
    }

    private void OnNavigationCompleteEvent(NavigationCompleteEvent args)
    {
        if (m_movementController != args.NavMovementController)
        {
            return;
        }

        if (m_state == EnemyState.RunningAway)
        {
            Destroy(gameObject);
        }
    }

    private void OnRequestDaytimeEvent(RequestDaytimeEvent args)
    {
        if (m_state == EnemyState.Dead)
        {
            return;
        }

        m_state = EnemyState.RunningAway;
        SetTarget(m_spawnPoint);
    }

    public void SetTarget(Vector3 target)
    {
        m_target = target;
        if (m_movementController != null)
        {
            m_movementController.NavigateTo(target);
        }
    }

    public void DamageEnemy(double damage)
    {
        Health -= damage;

        if (Health <= 0.0)
        {
            m_eventAggregator.Publish(new EnemyDeathEvent { Enemy = this });
            m_movementController.StopNavigation();
            ShadowBlob.SetActive(false);
        }
    }
}
