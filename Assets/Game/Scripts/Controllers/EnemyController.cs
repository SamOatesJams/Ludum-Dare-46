﻿using System.Collections;
using SamOatesGames.Systems;
using UnityEngine;

[RequireComponent(typeof(NavMovementController))]
public class EnemyController : EntityController
{
    public enum EnemyState
    {
        Attacking,
        RunningAway,
        Dead,
        AttackingPlayer,
    }

    public EnemyState State => m_state;

    [Header("Attack settings")]
    public LayerMask PlayerLayer;
    public float EnemyVisionRange = 2.0f;
    public float EnemyAttackRange = 1.0f;
    public float EnemyAttackDelay = 1.0f;
    public double EnemyAttackDamage = 2.0f;

    [Header("Children")]
    public GameObject ShadowBlob;

    [Header("Audio")]
    public AudioClip[] AttackAudioClips;
    public AudioClip[] HurtAudioClips;

    private Vector3? m_target;
    private Vector3 m_spawnPoint;
    private EventAggregator m_eventAggregator;
    private NavMovementController m_movementController;
    private EnemyState m_state = EnemyState.Attacking;
    private AnimationController m_animationController;

    private PlayerController m_attackingPlayer;
    private float m_lastAttack;

    void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<RequestDaytimeEvent>(this, OnRequestDaytimeEvent);
        m_eventAggregator.Subscribe<NavigationCompleteEvent>(this, OnNavigationCompleteEvent);
        m_eventAggregator.Subscribe<EnemyDeathEvent>(this, OnEnemyDeathEvent);
        m_eventAggregator.Subscribe<PlayerDiedEvent>(this, OnPlayerDiedEvent);
        
        m_animationController = GetComponent<AnimationController>();
        m_movementController = GetComponent<NavMovementController>();

        Health = MaxHealth;
        m_spawnPoint = transform.position;

        if (m_target != null)
        {
            m_movementController.NavigateTo(m_target.Value);
        }
    }

    public void PlayerAttack(PlayerController player, double damage)
    {
        if (m_state != EnemyState.Attacking && m_state != EnemyState.AttackingPlayer)
        {
            return;
        }

        m_state = EnemyState.AttackingPlayer;
        m_attackingPlayer = player;

        m_movementController.StopNavigation();
        DamageEnemy(damage);
    }

    private void TryAttackingPlayer()
    {
        var distance = Vector3.Distance(transform.position, m_attackingPlayer.transform.position);
        if (distance > EnemyVisionRange)
        {
            m_attackingPlayer = null;
            m_state = m_state == EnemyState.AttackingPlayer ? EnemyState.Attacking : m_state;

            if (m_state != EnemyState.Dead)
            {
                m_movementController.ContinueNavigation();
            }
            return;
        }

        if (distance > EnemyAttackRange)
        {
            return;
        }

        if (Time.time - m_lastAttack <= EnemyAttackDelay)
        {
            return;
        }

        m_eventAggregator.Publish(new PlayShiftedAudioEvent(
            AudioIds.Villager + Random.Range(100, 110),
            AttackAudioClips[Random.Range(0, AttackAudioClips.Length)],
            new Vector2(0.75f, 1.2f),
            0.75f));

        m_animationController.SetPunchTarget(m_attackingPlayer.transform);
        m_attackingPlayer.DamagePlayer(EnemyAttackDamage);
        m_lastAttack = Time.time;
    }

    void FixedUpdate()
    {
        if (m_state == EnemyState.AttackingPlayer)
        {
            TryAttackingPlayer();
        }

        if (Time.time - m_lastAttack > EnemyAttackDelay)
        {
            m_animationController.SetPunchTarget(null);
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

    private void OnPlayerDiedEvent(PlayerDiedEvent args)
    {
        if (args.Player != m_attackingPlayer)
        {
            return;
        }

        m_attackingPlayer = null;
        m_state = m_state == EnemyState.AttackingPlayer ? EnemyState.Attacking : m_state;
        m_animationController.SetPunchTarget(null);

        if (m_state != EnemyState.Dead)
        {
            m_movementController.ContinueNavigation();
        }
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
        m_eventAggregator.Publish(new PlayShiftedAudioEvent(
            AudioIds.Villager + Random.Range(100, 110),
            HurtAudioClips[Random.Range(0, HurtAudioClips.Length)],
            new Vector2(0.75f, 1.2f),
            0.75f));

        if (Health <= 0.0)
        {
            m_eventAggregator.Publish(new EnemyDeathEvent { Enemy = this });
            m_movementController.StopNavigation();
            ShadowBlob.SetActive(false);
        }
    }
}
