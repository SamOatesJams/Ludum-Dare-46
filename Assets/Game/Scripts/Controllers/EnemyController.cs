using SamOatesGames.Systems;
using UnityEngine;

[RequireComponent(typeof(NavMovementController))]
public class EnemyController : SubscribableMonoBehaviour
{
    public enum EnemyState
    {
        Attacking,
        RunningAway
    }

    public double Health { get; private set; }

    public double MaxHealth = 20.0;
    
    private Vector3? m_target;
    private Vector3 m_spawnPoint;
    private EventAggregator m_eventAggregator;
    private NavMovementController m_movementController;
    private EnemyState m_state = EnemyState.Attacking;

    void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);

        m_movementController = GetComponent<NavMovementController>();

        Health = MaxHealth;
        m_spawnPoint = transform.position;

        if (m_target != null)
        {
            m_movementController.NavigateTo(m_target.Value);
        }
    }

    private void OnRequestDaytimeEvent(RequestDaytimeEvent args)
    {
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
            Destroy(gameObject); // TODO death animation
        }
    }
}
