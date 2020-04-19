using SamOatesGames.Systems;
using UnityEngine;

public class ResourceSpawner : SubscribableMonoBehaviour
{
    public ResourceDescription[] ResourceDescriptions;

    private EventAggregator m_eventAggregator;
    private EnemyController m_owner;

    public void Start()
    {
        m_owner = GetComponentInParent<EnemyController>();

        m_eventAggregator = EventAggregator.GetInstance();
        m_eventAggregator.Subscribe<EnemyDeathEvent>(this, OnEnemyDeathEvent);
    }

    private void OnEnemyDeathEvent(EnemyDeathEvent args)
    {
        if (args.Enemy != m_owner)
        {
            return;
        }

        var resourceDescription = ResourceDescriptions[Random.Range(0, ResourceDescriptions.Length)];

        var amountToSpawn = Random.Range(resourceDescription.SpawnableAmount.x, resourceDescription.SpawnableAmount.y);
        for (var index = 0; index < amountToSpawn; ++index)
        {
            var offset = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), 0);
            var instance = Instantiate(resourceDescription.Prefab, transform.position + offset, Quaternion.identity);
            var rigidBody = instance.GetComponent<Rigidbody2D>();
            rigidBody.AddForce(new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)) * 100.0f);
        }
    }
}
