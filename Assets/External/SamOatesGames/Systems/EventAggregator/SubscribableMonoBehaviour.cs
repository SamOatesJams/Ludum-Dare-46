using System;
using UnityEngine;

namespace SamOatesGames.Systems
{
    public class SubscribableMonoBehaviour : MonoBehaviour, ISubscribable
    {
        /// <summary>
        /// Called when the object is destroyed.
        /// </summary>
        public virtual void OnDestroy()
        {
            if (!EventAggregator.Exists())
            {
                return;
            }

            var eventAggregator = EventAggregator.GetInstance();
            eventAggregator.UnSubscribeAll(this);
        }

        public void Subscribe<TEventType>(Action<TEventType> callback) where TEventType : IEventAggregatorEvent
        {
            if (!EventAggregator.Exists())
            {
                Debug.LogError($"No event aggregator instance exists, can not subscribe to '{typeof(TEventType)}' events in '{name}'");
                return;
            }

            var eventAggregator = EventAggregator.GetInstance();
            eventAggregator.Subscribe(this, callback);
        }

        public void Publish<TEventType>(TEventType eventToPublish) where TEventType : IEventAggregatorEvent
        {
            if (!EventAggregator.Exists())
            {
                Debug.LogError($"No event aggregator instance exists, can not publish '{typeof(TEventType)}' events in '{name}'");
                return;
            }

            var eventAggregator = EventAggregator.GetInstance();
            eventAggregator.Publish(eventToPublish);
        }
    }
}
