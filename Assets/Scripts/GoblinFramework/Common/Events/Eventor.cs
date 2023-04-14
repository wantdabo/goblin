using System;
using System.Collections.Generic;
using GoblinFramework.Core;

namespace GoblinFramework.Common.Events
{
    public class Eventor : Comp
    {
        private List<Delegate> allEvents;
        private Dictionary<Type, List<Delegate>> eventDict;

        public override void Create()
        {
            base.Create();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (null != allEvents) allEvents.Clear();
            if (null != eventDict) eventDict.Clear();
        }

        public void UnListenAll(Action<IEvent> func)
        {
            if (false == allEvents.Contains(func)) return;
            allEvents.Remove(func);
        }

        public void ListenAll(Action<IEvent> func)
        {
            if (null == allEvents) allEvents = new List<Delegate>();
            allEvents.Add(func);
        }

        public void UnListen<T>(Action<T> func) where T : IEvent
        {
            if (false == eventDict.TryGetValue(typeof(T), out var funcs)) return;
            funcs.Remove(func);
        }

        public void Listen<T>(Action<T> func) where T : IEvent
        {
            if (null == eventDict) eventDict = new Dictionary<Type, List<Delegate>>();

            if (false == eventDict.TryGetValue(typeof(T), out var funcs))
            {
                funcs = new List<Delegate>();
                eventDict.Add(typeof(T), funcs);
            }

            funcs.Add(func);
        }

        public void Tell<T>(T e) where T : IEvent
        {
            if (null != allEvents && allEvents.Count > 0)
                for (int i = allEvents.Count - 1; i >= 0; i--)
                    (allEvents[i] as Action<IEvent>).Invoke(e);

            if (null == eventDict) return;
            if (false == eventDict.TryGetValue(typeof(T), out var funcs)) return;
            for (int i = funcs.Count - 1; i >= 0; i--) (funcs[i] as Action<T>).Invoke(e);
        }
    }
}