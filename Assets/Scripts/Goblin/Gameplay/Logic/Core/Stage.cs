using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.RIL.Common;
using Random = Goblin.Gameplay.Logic.Behaviors.Random;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 场景
    /// </summary>
    public sealed class Stage
    {
        private const ulong sa = 0;
        private StageInfo info { get; set; }
        public StageState state => info.state;
        public Action<ulong, IRIL> onril { get; set; }
        public Random random => GetBehavior<Random>(sa);
        public RILSync rilsync => GetBehavior<RILSync>(sa);

        public void Initialize(int seed, byte[] data)
        {
            if (null != info) throw new Exception("you cannot initialize more than once.");
            
            info = new();
            info.Reset();
            info.actors.Add(sa);
            var dict = ObjectCache.Get<Dictionary<Type, IBehaviorInfo>>();
            dict.Add(typeof(StageInfo), info);
            info.behaviorinfos.Add(sa, dict);
            
            AddBehavior<Random>(sa).Initialze(seed);
            AddBehavior<RILSync>(sa);
            
            info.state = StageState.Initialized;
            // 添加渲染指令翻译器
            info.translators.Add(typeof(AttributeInfo), new Translators.Attribute().Initialize(this));
            info.translators.Add(typeof(SpatialInfo), new Translators.Spatial().Initialize(this));
            info.translators.Add(typeof(StateMachineInfo), new Translators.StateMachine().Initialize(this));
        }

        public void Start()
        {
            if (StageState.Initialized != info.state) return;
            info.state = StageState.Ticking;
        }

        public void Pause()
        {
            if (StageState.Ticking != info.state) return;
            info.state = StageState.Paused;
        }
        
        public void Resume()
        {
            if (StageState.Paused != info.state) return;
            info.state = StageState.Ticking;
        }

        public void Stop()
        {
            if (StageState.Stopped == info.state) return;
            info.state = StageState.Stopped;
            
            Disassembles();
            RecycleActors();
            info.rmvactors.AddRange(info.actors);
            RecycleActors();
        }

        public void Tick()
        {
            if (StageState.Ticking != info.state) return;
            
            info.frame++;
            info.elapsed += info.tick;
            
            var tickers = GetBehaviors<Ticker>();
            if (null != tickers)
            {
                foreach (var ticker in tickers) ticker.Tick(info.timescale * ticker.info.tick);
                Translators();
                foreach (var ticker in tickers) ticker.TickEnd();
                
                tickers.Clear();
                ObjectCache.Set(tickers);
            }
            
            Disassembles();
            RecycleActors();
        }

        private void Translators()
        {
            foreach (var kv in info.behaviorinfos)
            {
                foreach (var kv2 in kv.Value)
                {
                    if (false == info.translators.TryGetValue(kv2.Key, out var translator)) continue;
                    translator.Translate(kv.Key, kv2.Value);
                }
            }
        }
        
        private void Disassembles()
        {
            foreach (var id in info.actors)
            {
                foreach (var type in GetBehaviorTypes(id))
                {
                    Behavior behavior = GetBehavior(id, type);
                    behavior.Disassemble();
                    ObjectCache.Set(behavior);
                }
                
                Actor actor = GetActor(id);
                actor.Disassemble();
                ObjectCache.Set(actor);
            }
            
            info.actorassembleds.Clear();
            foreach (var kv in info.behaviorassembleds)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            info.behaviorassembleds.Clear();
        }
        
        private void RecycleActors()
        {
            foreach (var rmvactor in info.rmvactors)
            {
                if (false == info.behaviorinfos.TryGetValue(rmvactor, out var infos))
                {
                    info.actors.Remove(rmvactor);
                    continue;
                }
                
                foreach (var info in infos.Values)
                {
                    info.Reset();
                    ObjectCache.Set(info);
                }

                infos.Clear();
                ObjectCache.Set(infos);
                info.behaviorinfos.Remove(rmvactor);
                
                if (false == info.behaviors.TryGetValue(rmvactor, out var types)) continue;
                
                foreach (var type in types)
                {
                    info.behaviorowners.TryGetValue(type, out var owners);
                    owners.Remove(rmvactor);
                            
                    if (owners.Count > 0) continue;
                    ObjectCache.Set(owners);
                    info.behaviorowners.Remove(type);
                }

                types.Clear();
                ObjectCache.Set(types);
                info.behaviors.Remove(rmvactor);
            }
            info.rmvactors.Clear();
        }

        public Actor GetActor(ulong id)
        {
            if (false == info.actors.Contains(id)) return default;
            
            if (info.actorassembleds.TryGetValue(id, out var a)) return a;

            var actor = ObjectCache.Get<Actor>();
            actor.Assemble(id,this);
            info.actorassembleds.Add(id, actor);
            
            return actor;
        }

        public void RmvActor(ulong id)
        {
            if (info.rmvactors.Contains(id)) return;
            info.rmvactors.Add(id);
        }

        public Actor AddActor()
        {
            info.increment++;
            info.actors.Add(info.increment);

            return GetActor(info.increment);
        }
        
        public List<Type> GetBehaviorTypes(ulong id)
        {
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            if (false == info.behaviors.TryGetValue(id, out var list)) return default;

            return list;
        }

        public List<T> GetBehaviors<T>() where T : Behavior
        {
            var type = typeof(T);
            if (false == info.behaviorowners.TryGetValue(type, out var owners) || 0 == owners.Count) return default;

            var list = ObjectCache.Get<List<T>>();
            foreach (var owner in owners) list.Add(GetBehavior(owner, type) as T);

            return list;
        }

        public Behavior GetBehavior(ulong id, Type type)
        {
            if (typeof(Gamepad) == type) return GetBehavior<Gamepad>(id);
            else if (typeof(Movement) == type) return GetBehavior<Movement>(id);
            else if (typeof(Random) == type) return GetBehavior<Random>(id);
            else if (typeof(RILSync) == type) return GetBehavior<RILSync>(id);
            else if (typeof(StateMachine) == type) return GetBehavior<StateMachine>(id);
            else if (typeof(Ticker) == type) return GetBehavior<Ticker>(id);

            return default;
        }

        public T GetBehavior<T>(ulong id) where T : Behavior, new()
        {
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            
            if (false == info.behaviors.TryGetValue(id, out var list)) return default;
            if (false == list.Contains(typeof(T))) return default;
            
            if (info.behaviorassembleds.TryGetValue(id, out var dict) && dict.TryGetValue(typeof(T), out var b))
            {
                return b as T;
            }

            var behavior = ObjectCache.Get<T>();
            behavior.Assemble(GetActor(id));
            
            if (null == dict)
            {
                dict = ObjectCache.Get<Dictionary<Type, Behavior>>();
                info.behaviorassembleds.Add(id, dict);
            }
            dict.Add(typeof(T), behavior);

            return behavior;
        }

        public T AddBehavior<T>(ulong id) where T : Behavior, new()
        {
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            if (false == info.behaviors.TryGetValue(id, out var list)) info.behaviors.Add(id, list = ObjectCache.Get<List<Type>>());
            if (list.Contains(typeof(T))) throw new Exception($"behavior {typeof(T)} is exist.");

            if (false == info.behaviorowners.TryGetValue(typeof(T), out var owners)) info.behaviorowners.Add(typeof(T), owners = ObjectCache.Get<List<ulong>>());
            owners.Add(id);
            list.Add(typeof(T));
            
            return GetBehavior<T>(id);
        }

        public T GetBehaviorInfo<T>(ulong id) where T : IBehaviorInfo
        {
            if (false == info.behaviorinfos.TryGetValue(id, out var dict)) return default;
            if (false == dict.TryGetValue(typeof(T), out var behaviorinfo)) return default;

            return (T)behaviorinfo;
        }
        
        public T AddBehaviorInfo<T>(ulong id) where T : IBehaviorInfo, new()
        {
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            if (false == info.behaviorinfos.TryGetValue(id, out var dict))
            {
                info.behaviorinfos.Add(id, dict = ObjectCache.Get<Dictionary<Type, IBehaviorInfo>>());
            }
            if (dict.ContainsKey(typeof(T))) throw new Exception($"behaviorinfo {typeof(T)} is exist.");

            var behaviorinfo = ObjectCache.Get<T>();
            dict.Add(typeof(T), behaviorinfo);
            behaviorinfo.Ready();
            
            return behaviorinfo;
        }
    }
}