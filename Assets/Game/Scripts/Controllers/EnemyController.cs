using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SamOatesGames.Systems;

public class EnemyController : SubscribableMonoBehaviour
{
    public double Health { get; private set; }

    public double MaxHealth = 20.0;

    private EventAggregator m_eventAggregator;

    void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
        Health = MaxHealth;
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
