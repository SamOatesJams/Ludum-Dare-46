using SamOatesGames.Systems.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SamOatesGames.Systems
{
    [DefaultExecutionOrder(-100)]
    public class EventAggregator : UnitySingleton<EventAggregator>
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly Dictionary<Type, Dictionary<ISubscribable, Delegate>> m_handlers =
            new Dictionary<Type, Dictionary<ISubscribable, Delegate>>();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <param name="subscriber"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public bool Subscribe<TEventType>(ISubscribable subscriber, Action<TEventType> action) where TEventType : IEventAggregatorEvent
        {
            if (!m_handlers.TryGetValue(typeof(TEventType), out var subscriberToActionMap))
            {
                subscriberToActionMap = new Dictionary<ISubscribable, Delegate>();
                m_handlers[typeof(TEventType)] = subscriberToActionMap;
            }

            // We are already subscribed to the specified event.
            if (subscriberToActionMap.ContainsKey(subscriber))
            {
                return false;
            }

            // Store the subscription
            subscriberToActionMap[subscriber] = action;
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool UnSubscribe<TEventType>(ISubscribable subscriber) where TEventType : IEventAggregatorEvent
        {
            if (!m_handlers.TryGetValue(typeof(TEventType), out var subscriberToActionMap))
            {
                // Nothing is subscribed to this event.
                return false;
            }

            if (!subscriberToActionMap.ContainsKey(subscriber))
            {
                // The subscriber isn't subscribed to this event type
                return false;
            }

            // Remove the subscription
            subscriberToActionMap.Remove(subscriber);

            // Remove empty handler
            foreach (var eventType in m_handlers.Keys.ToList())
            {
                if (m_handlers[eventType].Count == 0)
                {
                    m_handlers.Remove(eventType);
                }
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subscriber"></param>
        /// <returns></returns>
        public bool UnSubscribeAll(ISubscribable subscriber)
        {
            var anyRemoved = false;

            foreach (var handler in m_handlers.Values)
            {
                if (!handler.ContainsKey(subscriber))
                {
                    // The subscriber isn't subscribed to this event type
                    continue;
                }

                handler.Remove(subscriber);
                anyRemoved = true;
            }

            // Remove empty handlers
            foreach (var eventType in m_handlers.Keys.ToList())
            {
                if (m_handlers[eventType].Count == 0)
                {
                    m_handlers.Remove(eventType);
                }
            }
            
            return anyRemoved;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEventType"></typeparam>
        /// <param name="eventToPublish"></param>
        /// <returns></returns>
        public bool Publish<TEventType>(TEventType eventToPublish) where TEventType : IEventAggregatorEvent
        {
            if (!m_handlers.TryGetValue(typeof(TEventType), out var subscriberToActionMap))
            {
                // Nothing is subscribed to this event.
                return false;
            }

            Debug.Log($"[Event Aggregator] Published '{typeof(TEventType)}' event to '{subscriberToActionMap.Count}' subscribers.");

            // Call the delegates on all subscribers
            foreach (var subscriber in subscriberToActionMap.ToList())
            {
                if (subscriber.Value is Action<TEventType> action)
                {
                    action.Invoke(eventToPublish);
                }
            }

            return true;
        }
    }
}
