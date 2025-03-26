using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Random = Goblin.Gameplay.Logic.Behaviors.Random;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 场景状态
    /// </summary>
    public enum StageState
    {
        /// <summary>
        /// 无
        /// </summary>
        None,
        /// <summary>
        /// 初始化
        /// </summary>
        Initialized,
        /// <summary>
        /// 暂停中
        /// </summary>
        Paused,
        /// <summary>
        /// 驱动中
        /// </summary>
        Ticking,
        /// <summary>
        /// 停止了
        /// </summary>
        Stopped,
    }
    
    /// <summary>
    /// 场景
    /// </summary>
    public sealed class Stage
    {
        private ulong increment { get; set; } = 0;
        private ulong sa { get; set; } = 0;
        private List<ulong> actors { get; set; } = new();
        private List<ulong> rmvactors { get; set; } = new();
        private Dictionary<ulong, List<Type>> behaviors { get; set; } = new();
        private Dictionary<ulong, Dictionary<Type, BehaviorInfo>> behaviorinfos { get; set; } = new();
        private Dictionary<ulong, Actor> actorassembleds { get; set; } = new();
        private Dictionary<ulong, Dictionary<Type, Behavior>> behaviorassembleds { get; set; } = new();
        public StageState state { get; private set; } = StageState.None;
        public Random random => GetBehavior<Random>(sa);
        public RILSync rilsync => GetBehavior<RILSync>(sa);

        public void Initialize(int seed, byte[] data)
        {
            if (StageState.None != state) return;
            state = StageState.Initialized;

            sa = AddActor().id;
            AddBehavior<Random>(sa).Initialze(seed);
            AddBehavior<RILSync>(sa);
        }

        public void Start()
        {
            if (StageState.Initialized != state) return;
            state = StageState.Ticking;
        }

        public void Pause()
        {
            if (StageState.Ticking != state) return;
        }
        
        public void Resume()
        {
            if (StageState.Paused != state) return;
        }

        public void Stop()
        {
            if (StageState.Stopped == state) return;
        }

        public void Tick()
        {
            if (StageState.Ticking != state) return;
            
            foreach (var id in actors)
            {
                Actor actor = GetActor(id);
                actor.ticker.Tick();
            }
            
            Disassembles();
            RmvActors();
        }
        
        private void Disassembles()
        {
            foreach (var id in actors)
            {
                foreach (var type in GetBehaviors(id))
                {
                    Behavior behavior = GetBehavior(id, type);
                    behavior.Disassemble();
                    ObjectCache.Set(behavior);
                }
                
                Actor actor = GetActor(id);
                actor.Disassemble();
                ObjectCache.Set(actor);
            }
            
            actorassembleds.Clear();
            foreach (var kv in behaviorassembleds)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            behaviorassembleds.Clear();
        }
        
        private void RmvActors()
        {
            foreach (var rmvactor in rmvactors)
            {
                if (behaviors.TryGetValue(rmvactor, out var types))
                {
                    types.Clear();
                    ObjectCache.Set(types);
                }

                if (behaviorinfos.TryGetValue(rmvactor, out var infos))
                {
                    foreach (var info in infos.Values)
                    {
                        info.Reset();
                        ObjectCache.Set(info);
                    }

                    infos.Clear();
                    ObjectCache.Set(infos);
                }

                actors.Remove(rmvactor);
            }
            rmvactors.Clear();
        }
        
        public Actor GetActor(ulong id)
        {
            if (false == actors.Contains(id)) return default;
            
            if (actorassembleds.TryGetValue(id, out var a)) return a;

            var actor = ObjectCache.Get<Actor>();
            actor.Assemble(id,this);
            actorassembleds.Add(id, actor);
            
            return actor;
        }

        public void RmvActor(ulong id)
        {
            if (rmvactors.Contains(id)) return;
            rmvactors.Add(id);
        }

        public Actor AddActor()
        {
            increment++;
            actors.Add(increment);

            return GetActor(increment);
        }
        
        public List<Type> GetBehaviors(ulong id)
        {
            if (false == actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            if (false == behaviors.TryGetValue(id, out var list)) return default;

            return list;
        }
        
        public Behavior GetBehavior(ulong id, Type type)
        {
            if (typeof(Ticker) == type) return GetBehavior<Ticker>(id);
            else if (typeof(Gamepad) == type) return GetBehavior<Gamepad>(id);
            else if (typeof(Spatial) == type) return GetBehavior<Spatial>(id);

            return default;
        }

        public T GetBehavior<T>(ulong id) where T : Behavior, new()
        {
            if (false == actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            
            if (false == behaviors.TryGetValue(id, out var list)) return default;
            if (false == list.Contains(typeof(T))) return default;
            
            if (behaviorassembleds.TryGetValue(id, out var dict) && dict.TryGetValue(typeof(T), out var b))
            {
                return b as T;
            }

            var behavior = ObjectCache.Get<T>();
            behavior.Assemble(GetActor(id));
            
            if (null == dict)
            {
                dict = ObjectCache.Get<Dictionary<Type, Behavior>>();
                behaviorassembleds.Add(id, dict);
            }
            dict.Add(typeof(T), behavior);

            return behavior;
        }

        public T AddBehavior<T>(ulong id) where T : Behavior, new()
        {
            if (false == actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            if (false == behaviors.TryGetValue(id, out var list)) behaviors.Add(id, ObjectCache.Get<List<Type>>());
            if (list.Contains(typeof(T))) throw new Exception($"behavior {typeof(T)} is exist.");

            list.Add(typeof(T));

            return GetBehavior<T>(id);
        }

        public T GetBehaviorInfo<T>(ulong id) where T : BehaviorInfo
        {
            if (false == behaviorinfos.TryGetValue(id, out var dict)) return default;
            if (false == dict.TryGetValue(typeof(T), out var info)) return default;

            return info as T;
        }
        
        public T AddBehaviorInfo<T>(ulong id) where T : BehaviorInfo, new()
        {
            if (false == actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            if (false == behaviorinfos.TryGetValue(id, out var dict))
            {
                behaviorinfos.Add(id, ObjectCache.Get<Dictionary<Type, BehaviorInfo>>());
            }
            if (dict.ContainsKey(typeof(T))) throw new Exception($"behaviorinfo {typeof(T)} is exist.");

            var info = ObjectCache.Get<T>();
            dict.Add(typeof(T), info);
            info.Ready();
            
            return info;
        }
    }
}