using UnityEngine;
using SamOatesGames.Systems;

public class EnemySpawnPoint : SubscribableMonoBehaviour
{
    [Header("Spawn Points")]
    public Transform[] SpawnPoints;

    [Header("Spawn Settings")]
    public AnimationCurve EnemySpawnCurve;
    public int MaxWaves = 10;
    public float EnemySpawnMultiplier = 1.0f;
    public float RandomSpawnMultiplier = 5.0f;

    [Header("Spawn delay settings")]
    public int EnemySpawnDelayTicks = 10;
    public float RandomSpawnTicksMultiplier = 5.0f;

    [Header("Enemy Settings")]
    public EnemyController EnemyPrefab;
    public CollisionMask CollisionMask;
    public Transform Target;

    private int m_currentWave;
    private int m_remainingEnemies;
    private int m_spawnTicksRemaining;

    private EventAggregator m_eventAggregator;

    void Start()
    {
        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<StartWaveEvent>(this, StartWave);
    }

    public void StartWave(StartWaveEvent e)
    {
        Debug.Log($"Spawning wave {e.Wave}");
        m_currentWave = e.Wave;
        m_remainingEnemies = Mathf.FloorToInt(EnemySpawnMultiplier * EnemySpawnCurve.Evaluate(m_currentWave / MaxWaves) + Random.value * RandomSpawnMultiplier);
    }

    private void SpawnEnemy()
    {
        var spawnPoint = SpawnPoints == null || SpawnPoints.Length == 0 
            ? transform 
            : SpawnPoints[Random.Range(0, SpawnPoints.Length)];

        var enemyObj = Instantiate(EnemyPrefab, spawnPoint.position, Quaternion.identity, transform);

        var controller = enemyObj.GetComponent<NavMovementController>();
        controller.CollisionMask = CollisionMask;

        var enemy = enemyObj.GetComponent<EnemyController>();
        enemy.Target = Target;

        m_eventAggregator.Publish(new EnemySpawnEvent() { Enemy = enemy });
        InventorySystem.GetInstance().AddItem(ResourceType.Currency, 10);
    }

    public void FixedUpdate()
    {
        if (m_remainingEnemies <= 0)
        {
            return;
        }

        if (m_spawnTicksRemaining-- == 0)
        {
            SpawnEnemy();

            m_remainingEnemies--;
            m_spawnTicksRemaining = Mathf.FloorToInt(EnemySpawnDelayTicks + (Random.value * RandomSpawnTicksMultiplier));
        }
    }
}
