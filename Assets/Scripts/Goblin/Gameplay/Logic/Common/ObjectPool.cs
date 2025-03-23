using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Common
{
    public class ObjectPool
    {
        private Queue<Actor> actors { get; set; } = new();
        private Dictionary<Type, Queue<Behavior>> behaviordict { get; set; } = new();
        private Dictionary<Type, Queue<BehaviorInfo>> behaviorinfodict { get; set; } = new();

        public Actor GetActor()
        {
            if (0 == actors.Count) return new();

            return actors.Dequeue();
        }

        public void SetActor(Actor actor)
        {
            actors.Enqueue(actor);
        }

        public T GetBehavior<T>() where T : Behavior, new()
        {
            if (false == behaviordict.TryGetValue(typeof(T), out var behaviors)) behaviordict.Add(typeof(T), behaviors = new());
    
            if (0 == behaviors.Count) return new();
            return behaviors.Dequeue() as T;
        }

        public void SetBehavior<T>(T behavior) where T : Behavior
        {
            if (false == behaviordict.TryGetValue(typeof(T), out var behaviors)) behaviordict.Add(typeof(T), behaviors = new());
            behaviors.Enqueue(behavior);
        }

        public T GetBehaviorInfo<T>() where T : BehaviorInfo, new()
        {
            if (false == behaviorinfodict.TryGetValue(typeof(T), out var behaviorinfos)) behaviorinfodict.Add(typeof(T), behaviorinfos = new());
       
            if (0 == behaviorinfos.Count) return new();
            return behaviorinfos.Dequeue() as T;
        }

        public void SetBehaviorInfo<T>(T behaviorInfo) where T : BehaviorInfo
        {
            if (false == behaviorinfodict.TryGetValue(typeof(T), out var behaviorinfos)) behaviorinfodict.Add(typeof(T), behaviorinfos = new());
            behaviorinfos.Enqueue(behaviorInfo);
        }
    }
}