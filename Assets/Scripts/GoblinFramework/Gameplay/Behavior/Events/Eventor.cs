using System;
using System.Collections.Generic;

namespace GoblinFramework.Gameplay.Events
{
    public class Eventor : Behavior
    {
        private Dictionary<Type, List<Delegate>> eventDict = new ();

        public void UnListen<T>(Action<T> func) where T : Event
        {
            if (false == eventDict.TryGetValue(typeof(T), out var funcs)) return;
            funcs.Remove(func);
        }

        public void Listen<T>(Action<T> func) where T : Event
        {
            if (false == eventDict.TryGetValue(typeof(T), out var funcs))
            {
                funcs = new List<Delegate>();
                eventDict.Add(typeof(T), funcs);
            }
            
            funcs.Add(func);
        }
    
        public void Tell<T>(T evt) where T : Event
        {
            if (false == eventDict.TryGetValue(typeof(T), out var funcs)) return;
            
            for(int i = funcs.Count - 1; i >= 0; i--) (funcs[i] as Action<T>).Invoke(evt);
        }
    }
}