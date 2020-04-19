using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class TrapSpikes : MonoBehaviour
{
    public double MaxHealth = 20.0f;
    public double DamagePerTick = 1.0f;
    public double SelfDamagePerTick = 1.0f;
    public int TicksPerDamageTick = 20;

    public double Health { get; private set; }

    private HashSet<EnemyController> m_enemies = new HashSet<EnemyController>();
    private int m_currentTicks = 0;
    private Animator m_animator;

    private static readonly int HealthAnimatorFloat = Animator.StringToHash("Health");

    void Start()
    {
        m_animator = GetComponent<Animator>();

        m_currentTicks = TicksPerDamageTick;
        Health = MaxHealth;
        m_animator.SetFloat(HealthAnimatorFloat, 1.0f);
    }

    private void DamageSelf(double amount)
    {
        Health -= amount;
        m_animator.SetFloat(HealthAnimatorFloat, (float) (Health / MaxHealth));

        if (Health <= 0)
        {
            // TODO animation
            Destroy(gameObject);
        }
    }

    private void DamageTick()
    {
        double selfDamage = 0;
        var enemies = new HashSet<EnemyController>(m_enemies);

        foreach (var enemy in enemies)
        {
            enemy.DamageEnemy(DamagePerTick);
            selfDamage += SelfDamagePerTick;
        }

        DamageSelf(selfDamage);
    }

    void FixedUpdate()
    {
        if (m_currentTicks-- <= 0)
        {
            m_currentTicks = TicksPerDamageTick;
            DamageTick();
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        var gameobj = collider.gameObject;
        var enemyController = gameobj.GetComponent<EnemyController>();

        if (enemyController == null)
        {
            return;
        }

        Debug.Log($"Enemy {gameobj.name} entering trap zone");
        m_enemies.Add(enemyController);
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        var gameobj = collider.gameObject;
        var enemyController = gameobj.GetComponent<EnemyController>();

        if (enemyController == null)
        {
            return;
        }

        Debug.Log($"Enemy {gameobj.name} exiting trap zone");
        m_enemies.Remove(enemyController);
    }
}
