using System;
using System.Collections.Generic;
using System.Linq;
using SamOatesGames.Systems.Core;

namespace SamOatesGames.Systems
{
    public class OptionsSystem : UnitySingleton<OptionsSystem>, ISubscribable
    {
        [Serializable]
        public class OptionData
        {
            [Serializable]
            public class OptionKey
            {
                public string Name;
                public string ValueAsString;
            }

            public OptionData()
            {
                Options = new List<OptionKey>();
            }

            public List<OptionKey> Options;
        }

        /// <summary>
        /// 
        /// </summary>
        public string OptionsMenuFile { get; set; } = "settings.dat";

        /// <summary>
        /// 
        /// </summary>
        private EventAggregator m_eventAggregator;

        /// <summary>
        /// 
        /// </summary>
        private FileSaveSystem m_fileSaveSystem;

        /// <summary>
        /// 
        /// </summary>
        private OptionData m_optionData;

        /// <summary>
        /// 
        /// </summary>
        public override void ResolveSystems()
        {
            m_fileSaveSystem = FileSaveSystem.GetInstance();
            m_eventAggregator = EventAggregator.GetInstance();
            m_eventAggregator.Subscribe<OptionChangedEvent>(this, HandleOptionChangedEvent);

            LoadOptions();
        }

        /// <summary>
        /// 
        /// </summary>
        public void OnDestroy()
        {
            if (m_eventAggregator == null)
            {
                return;
            }

            m_eventAggregator.UnSubscribeAll(this);
            m_eventAggregator = null;
        }

        /// <summary>
        /// 
        /// </summary>
        private void LoadOptions()
        {
            if (!m_fileSaveSystem.LoadData(out m_optionData, OptionsMenuFile))
            {
                m_optionData = new OptionData();
            }

            if (m_eventAggregator != null)
            {
                m_eventAggregator.Publish(new OptionsLoadedEvent(this));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SaveOptions()
        {
            m_fileSaveSystem.SaveData(m_optionData, OptionsMenuFile);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        private void HandleOptionChangedEvent(OptionChangedEvent e)
        {
            SetValue(e.Name, e.Value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optionName"></param>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        public void SetValue<T>(string optionName, T value)
        {
            var optionKey =
                m_optionData.Options.FirstOrDefault(x => x.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));

            if (optionKey == null)
            {
                optionKey = new OptionData.OptionKey
                {
                    Name = optionName
                };
                m_optionData.Options.Add(optionKey);
            }

            optionKey.ValueAsString = value.ToString();
            SaveOptions();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="optionName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>(string optionName, T defaultValue = default(T))
        {
            var optionKey =
                m_optionData.Options.FirstOrDefault(x => x.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));

            if (optionKey == null)
            {
                return defaultValue;
            }

            if (typeof(T) == typeof(string))
            {
                return (T) (object) optionKey.ValueAsString;
            }

            if (typeof(T) == typeof(float))
            {
                float value;
                if (!float.TryParse(optionKey.ValueAsString, out value))
                {
                    return defaultValue;
                }

                return (T) (object) value;
            }

            if (typeof(T) == typeof(int))
            {
                int value;
                if (!int.TryParse(optionKey.ValueAsString, out value))
                {
                    return defaultValue;
                }

                return (T) (object) value;
            }

            if (typeof(T) == typeof(bool))
            {
                bool value;
                if (!bool.TryParse(optionKey.ValueAsString, out value))
                {
                    return defaultValue;
                }

                return (T) (object) value;
            }

            return defaultValue;
        }
    }
}
