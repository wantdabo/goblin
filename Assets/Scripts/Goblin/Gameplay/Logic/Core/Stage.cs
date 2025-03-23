using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 场景
    /// </summary>
    public sealed class Stage
    {
        private ulong increment { get; set; } = new();
        private List<ulong> actors { get; set; } = new();
        private Dictionary<ulong, Dictionary<Type, BehaviorInfo>> behaviorinfos { get; set; } = new();
        public ObjectPool pool { get; private set; }

        public Stage()
        {
            pool = new ObjectPool();
        }

        public Actor GetActor(ulong id)
        {
            if (false == actors.Contains(id)) return default;

            var actor = pool.GetActor();
            actor.Assembly(id, this);

            return actor;
        }

        public Actor AddActor()
        {
            increment++;
            var actor = pool.GetActor();
            actor.Assembly(increment, this);

            return actor;
        }

        public T GetBehavior<T>(ulong id) where T : Behavior, new()
        {
            if (false == behaviorinfos.TryGetValue(id, out var dict)) return default;

            var behavior = pool.GetBehavior<T>();
            behavior.Assembly(GetActor(id));

            return behavior;
        }

        public T AddBehavior<T>(ulong id) where T : Behavior, new()
        {
            if (false == behaviorinfos.TryGetValue(id, out var dict))
            {
                dict = new();
                behaviorinfos.Add(id, dict);
            }

            if (dict.ContainsKey(typeof(T))) throw new Exception($"behavior {typeof(T)} is exist.");

            var behavior = pool.GetBehavior<T>();
            behavior.Assembly(GetActor(id));

            return behavior;
        }

        public BehaviorInfo GetBehaviorInfo<T>(ulong id) where T : Behavior
        {
            if (false == behaviorinfos.TryGetValue(id, out var dict)) return default;
            if (false == dict.TryGetValue(typeof(T), out var info)) return default;
            
            return info;
        }

        public void AddBehaviorInfo<T>(ulong id, BehaviorInfo info) where T : Behavior
        {
            if (false == behaviorinfos.TryGetValue(id, out var dict)) behaviorinfos.Add(id, dict = new());
            if (dict.ContainsKey(typeof(T))) throw new Exception($"behavior {typeof(T)} is exist.");
            
            dict.Add(typeof(T), info);
        }
    }
}