using System;
using System.Collections.Generic;

namespace GoblinFramework.Gameplay.Events
{
    public class EventorInfo : BehaviorInfo
    {
        public Dictionary<Type, List<Delegate>> eventDict = new ();
    }

    public class Eventor : Behavior<EventorInfo>
    {
        public void Hear<T>(Action<T> func) where T : Event
        {
            if (false == info.eventDict.TryGetValue(typeof(T), out var funcs))
            {
                funcs = new List<Delegate>();
                info.eventDict.Add(typeof(T), funcs);
            }
            
            funcs.Add(func);
        }

        public void Tell<T>(T content) where T : Event
        {
            if (false == info.eventDict.TryGetValue(typeof(T), out var funcs))
                return;

            foreach (var func in funcs)  (func as Action<T>).Invoke(content);
        }
    }
}