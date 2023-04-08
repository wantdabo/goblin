using System;
using System.Collections.Generic;
using GoblinFramework.Client.Gameplay;
using GoblinFramework.Core;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Events;

namespace GoblinFramework.Gameplay
{
    public class Actor : Comp
    {
        public uint id;
        public GameStage stage;
        public Eventor eventor;

        private Dictionary<Type, Behavior> behaviorDict = new Dictionary<Type, Behavior>();

        public T GetBehavior<T>() where T : Behavior
        {
            if (behaviorDict.TryGetValue(typeof(T), out var behavior)) return behavior as T;
            
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

        public void RmvBehavior<T>() where T : Behavior
        {
            RmvBehavior(GetBehavior<T>());
        }

        public void RmvBehavior(Behavior behavior)
        {
            behaviorDict.Remove(behavior.GetType());
        }
    }
}