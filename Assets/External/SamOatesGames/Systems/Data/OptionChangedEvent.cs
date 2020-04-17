namespace SamOatesGames.Systems
{
    public class OptionChangedEvent : IEventAggregatorEvent
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public OptionChangedEvent(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
