using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamOatesGames.Systems;

[RequireComponent(typeof(NavMovementController))]
public class EnemyController : SubscribableMonoBehaviour
{
    public double Health { get; private set; }

    public double MaxHealth = 20.0;
    public Transform Target;

    private EventAggregator m_eventAggregator;
    private NavMovementController m_movementController;

    void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
        m_movementController = GetComponent<NavMovementController>();

        Health = MaxHealth;
        m_movementController.NavigateTo(Target.position);
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
    
    void Update()
    {
        
    }
}
