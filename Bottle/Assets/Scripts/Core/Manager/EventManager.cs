using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Bottle.Extensions.Singleton;
using Sirenix.OdinInspector;
namespace Bottle.Core.Manager
{
    public class EventManager : PersistentObject<EventManager>
    {
        [ShowInInspector]
        private Dictionary<string, Action<Dictionary<string, object>>> eventDictionary;

        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, Action<Dictionary<string, object>>>();
            }
        }

        public void StartListening(string eventName, Action<Dictionary<string, object>> listener)
        {
            if (Instance.eventDictionary.ContainsKey(eventName))
            {
                Instance.eventDictionary[eventName] += listener;
            }
            else
            {
                Instance.eventDictionary.Add(eventName, listener);
            }
        }

        public void StopListening(string eventName, Action<Dictionary<string, object>> listener)
        {
            if (Instance.eventDictionary.ContainsKey(eventName))
            {
                Instance.eventDictionary[eventName] -= listener;
            }
        }

        public void TriggerEvent(string eventName, Dictionary<string, object> message)
        {
            Action<Dictionary<string, object>> thisEvent = null;
            if (eventName != null)
            {
                if (Instance.eventDictionary.TryGetValue(eventName, out thisEvent))
                {
                    thisEvent.Invoke(message);
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            
        }
        protected override void Awake()
        {
            base.Awake();
            Init();
        }
    }
}
