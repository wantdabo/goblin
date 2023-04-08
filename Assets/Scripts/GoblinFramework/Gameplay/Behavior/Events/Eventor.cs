using System;
using System.Collections.Generic;

namespace GoblinFramework.Gameplay.Events
{
    public class EventorInfo : BehaviorInfo
    {
        public Dictionary<Type, List<Action<Event>>> eventDict = new ();
    }

    public class Eventor : Behavior<EventorInfo>
    {
        public void Hear<T>(Action<T> func) where T : Event
        {
            if (false == info.eventDict.TryGetValue(typeof(T), out var funcs))
            {
                funcs = new List<Action<Event>>();
                info.eventDict.Add(typeof(T), funcs);
            }
            
            funcs.Add(func as Action<Event>);
        }

        public void Tell<T>(T content) where T : Event
        {
            if (false == info.eventDict.TryGetValue(typeof(T), out var funcs))
                return;

            foreach (var func in funcs) func.Invoke(content);
        }
    }
}