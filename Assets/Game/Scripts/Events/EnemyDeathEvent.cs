using SamOatesGames.Systems;

public class EnemyDeathEvent : IEventAggregatorEvent
{
    public EnemyController Enemy { get; set; }
}
