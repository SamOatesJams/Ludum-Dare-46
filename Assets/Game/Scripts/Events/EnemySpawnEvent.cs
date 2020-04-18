using SamOatesGames.Systems;

public class EnemySpawnEvent : IEventAggregatorEvent
{
    public Enemy Enemy { get; set; }
}
