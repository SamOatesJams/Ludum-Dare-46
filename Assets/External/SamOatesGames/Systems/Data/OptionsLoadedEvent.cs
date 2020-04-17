using SamOatesGames.Systems;

public class OptionsLoadedEvent : IEventAggregatorEvent
{
    /// <summary>
    /// 
    /// </summary>
    public OptionsSystem OptionsSystem { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="optionsSystem"></param>
    public OptionsLoadedEvent(OptionsSystem optionsSystem)
    {
        OptionsSystem = optionsSystem;
    }
}
