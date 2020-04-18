using SamOatesGames.Systems;

public class EnemySpawnEvent : IEventAggregatorEvent
{
    public EnemyController Enemy { get; set; }
}
