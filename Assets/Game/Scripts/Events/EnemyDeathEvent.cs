using SamOatesGames.Systems;

public class EnemyDeathEvent : IEventAggregatorEvent
{
    public Enemy Enemy { get; set; }
}
