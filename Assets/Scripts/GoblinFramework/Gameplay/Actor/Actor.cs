using System;
using System.Collections.Generic;
using GoblinFramework.Gameplay.Common;

namespace GoblinFramework.Gameplay
{
    public class Actor : PComp
    {
        public uint id;

        public Dictionary<Type, Behavior> behaviorDict = new Dictionary<Type, Behavior>();

        public T GetBehavior<T>() where T : Behavior
        {
            var behavior = GetBehavior(typeof(T));
            if (null == behavior) return null;

            return behavior as T;
        }

        public Behavior GetBehavior(Type type)
        {
            if (behaviorDict.TryGetValue(type, out var behavior)) return behavior;

            return null;
        }

        public T AddBehavior<T>() where T : Behavior, new()
        {
            if (behaviorDict.ContainsKey(typeof(T))) throw new Exception($"can't add same behavior -> {typeof(T)}");
            
            var behavior = AddComp<T>();
            behavior.actor = this;
            behaviorDict.Add(typeof(T), behavior);

            return behavior;
        }

        public void RmvBehavior<T>()
        {
            RmvBehavior(GetBehavior(typeof(T)));
        }

        public void RmvBehavior(Behavior behavior)
        {
            behaviorDict.Remove(behavior.GetType());
        }
    }
}