using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Batches;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.GameplayDatas;
using Goblin.Gameplay.Logic.Prefabs;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Goblin.Gameplay.Logic.RIL.Common;
using Kowtow.Math;
using Random = Goblin.Gameplay.Logic.Behaviors.Random;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 场景, 逻辑层的容器, 负责容纳所有的 Actor, 以及 Actor 的行为 & 行为数据
    /// </summary>
    public sealed class Stage
    {
        /// <summary>
        /// Stage.ActorID, Stage 的数据走的也是 Actor/Behavior/BehaviorInfo 那一套
        /// 通过包装 Actor 的形式使用
        /// 所以 Stage 也是 Actor, 但它是一个特殊的 Actor, 它的 ID 是 ulong.MaxValue
        /// </summary>
        public ulong sa { get; private set; } = ulong.MaxValue;
        /// <summary>
        /// Stage 当前的运行状态
        /// </summary>
        public StageState state => info.state;
        /// <summary>
        /// Stage 当前的帧号
        /// </summary>
        public uint frame => info.frame;
        /// <summary>
        /// Stage 数据
        /// </summary>
        public StageInfo info { get; private set; }
        /// <summary>
        /// 初始化 Stage 的游戏数据
        /// </summary>
        public GameplayData gpdata { get; set; }
        /// <summary>
        /// 随机数
        /// </summary>
        public Random random => GetBehavior<Random>(sa);
        /// <summary>
        /// 渲染指令同步
        /// </summary>
        public RILSync rilsync => GetBehavior<RILSync>(sa);
        /// <summary>
        /// 预制创建器集合
        /// </summary>
        public Dictionary<Type, Prefab> prefabs { get; set; }
        /// <summary>
        /// 批处理集合
        /// </summary>
        public Dictionary<Type, Batch> batches { get; set; }
        /// <summary>
        /// 对外暴露抛出 RIL 的事件
        /// </summary>
        public Action<ulong, IRIL> onril { get; set; }

        /// <summary>
        /// 初始化 Stage
        /// </summary>
        /// <param name="data">初始化的游戏数据</param>
        /// <exception cref="Exception">初始化数据为空 || 重复初始化</exception>
        public void Initialize(GameplayData data)
        {
            if (null == data) throw new Exception("data is null.");
            if (null != info) throw new Exception("you cannot initialize more than once.");
            
            // 构建 StageInfo, 因为 Stage 的数据也是走 BehaviorInfo, 无法通过自举的方式走 API 添加
            // 悖论存在, 此处手动添加
            info = ObjectCache.Get<StageInfo>();
            info.Ready(sa);
            info.state = StageState.Initialized;
            info.actors.Add(sa);
            info.actordict.Add(sa, ObjectCache.Get<Actor>());
            GetActor(sa).Assemble(sa, this);
            
            // 添加 Stage 行为
            AddBehavior<Random>(sa).Initialze(data.seed);
            AddBehavior<RILSync>(sa);
            AddBehavior<Tag>(sa).Set(TAG_DEFINE.ACTOR_TYPE, ACTOR_DEFINE.STAGE);
            // 添加预制创建器
            Prefabs();
            // 添加批处理
            Batches();

            this.gpdata = data;
            // TODO 临时代码, 后续挪到一个单独的地方, 构建初始化世界
            foreach (var player in data.players)
            {
                var hero = Spawn<Hero>(new HeroInfo
                {
                    hero = player.hero,
                    attribute = new()
                    {
                        hp = 100,
                        maxhp = 100,
                        moveseed = 10,
                        attack = 5,
                    },
                    spatial = new()
                    {
                        position = FPVector3.zero,
                        euler = FPVector3.zero,
                        scale = FPVector3.one
                    }
                });
                hero.AddBehavior<Gamepad>();
            }
        }
        
        /// <summary>
        /// 销毁 Stage
        /// </summary>
        public void Dispose()
        {
            if (StageState.Stopped != info.state) return;
            info.state = StageState.Disposed;
            
            // 回收 Actor
            info.rmvactors.Clear();
            info.rmvactors.AddRange(info.actors);
            RecycleActors();
            
            // 卸载 Prefabs
            foreach (var prefab in prefabs.Values) prefab.Unload();
            prefabs.Clear();
            ObjectCache.Set(prefabs);

            // 卸载批处理
            foreach (var batch in batches.Values)
            {
                batch.Unload();
            }
            batches.Clear();
            ObjectCache.Set(batches);
            
            // StageInfo 回收
            info.Reset();
            ObjectCache.Set(info);
        }
        
        /// <summary>
        /// 添加预制创建器
        /// </summary>
        /// <exception cref="Exception">不能重复添加</exception>
        private void Prefabs()
        {
            prefabs = ObjectCache.Get<Dictionary<Type, Prefab>>();
            prefabs.Add(typeof(Hero), ObjectCache.Get<Hero>().Load(this));
        }
        
        /// <summary>
        /// 添加批处理
        /// </summary>
        private void Batches()
        {
            batches = ObjectCache.Get<Dictionary<Type, Batch>>();
            batches.Add(typeof(Translator), ObjectCache.Get<Translator>().Load(this));
        }

        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            if (StageState.Initialized != info.state) return;
            info.state = StageState.Ticking;
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public void Pause()
        {
            if (StageState.Ticking != info.state) return;
            info.state = StageState.Paused;
        }
        
        /// <summary>
        /// 恢复
        /// </summary>
        public void Resume()
        {
            if (StageState.Paused != info.state) return;
            info.state = StageState.Ticking;
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            if (StageState.Stopped == info.state) return;
            info.state = StageState.Stopped;
        }
        
        /// <summary>
        /// 驱动
        /// </summary>
        public void Step()
        {
            if (StageState.Ticking != info.state) return;
            
            // 帧号递增 & 时间流逝
            info.frame++;
            info.elapsed += info.tick;

            // Tick 驱动
            foreach (var tt in TICK_DEFINE.TICK_TYPE_LIST)
            {
                switch (tt.ticktype)
                {
                    case TICK_DEFINE.BEHAVIOR:
                        var behaviors = GetBehaviors(tt.type);
                        if (null == behaviors) continue;
                        foreach (var behavior in behaviors)
                        {
                            var ticker = GetBehaviorInfo<TickerInfo>(behavior.actor.id);
                            behavior.Tick(ticker.timescale * info.tick);
                        }
                        break;
                    case TICK_DEFINE.BATCH:
                        if (false == batches.TryGetValue(tt.type, out var batch)) continue;
                        batch.Tick(info.tick);
                        break;
                }
            }

            // 帧末调度
            foreach (var tt in TICK_DEFINE.TICK_TYPE_LIST)
            {
                switch (tt.ticktype)
                {
                    case TICK_DEFINE.BEHAVIOR:
                        var behaviors = GetBehaviors(tt.type);
                        if (null == behaviors) continue;
                        foreach (var behavior in behaviors)
                        {
                            behavior.EndTick();
                        }
                        break;
                    case TICK_DEFINE.BATCH:
                        if (false == batches.TryGetValue(tt.type, out var batch)) continue;
                        batch.EndTick();
                        break;
                }
            }
            
            RecycleActors();
        }

        /// <summary>
        /// 回收/销毁 RmvActors
        /// </summary>
        private void RecycleActors()
        {
            foreach (var rmvactor in info.rmvactors)
            {
                if (info.behaviorinfodict.TryGetValue(rmvactor, out var infodict))
                {
                    foreach (var behaviorinfo in infodict.Values)
                    {
                        if (info.behaviorinfos.TryGetValue(behaviorinfo.GetType(), out var infos))
                        {
                            infos.Remove(behaviorinfo);
                            if (0 == infos.Count)
                            {
                                ObjectCache.Set(infos);
                                info.behaviorinfos.Remove(behaviorinfo.GetType());
                            }
                        }
                        behaviorinfo.Reset();
                        ObjectCache.Set(behaviorinfo);
                    }
                    infodict.Clear();
                    ObjectCache.Set(infodict);
                    info.behaviorinfodict.Remove(rmvactor);
                }

                if (info.behaviordict.TryGetValue(rmvactor, out var behaviordict))
                {
                    foreach (var behavior in behaviordict.Values)
                    {
                        if (info.behaviors.TryGetValue(behavior.GetType(), out var behaviors))
                        {
                            behaviors.Remove(behavior);
                            if (0 == behaviors.Count)
                            {
                                ObjectCache.Set(behaviors);
                                info.behaviors.Remove(behavior.GetType());
                            }
                        }
                        behavior.Disassemble();
                        ObjectCache.Set(behavior);
                    }
                }

                if (info.actordict.TryGetValue(rmvactor, out var actor))
                {
                    info.actors.Remove(rmvactor);
                    info.actordict.Remove(rmvactor);
                    actor.Disassemble();
                }
            }
            info.rmvactors.Clear();
        }

        /// <summary>
        /// 根据预制创建器构建一个 Actor
        /// </summary>
        /// <param name="prefabinfo">预制构建器数据</param>
        /// <typeparam name="T">预制构建器类型</typeparam>
        /// <returns>Actor</returns>
        /// <exception cref="Exception">预制构建器类型不存在</exception>
        public Actor Spawn<T>(IPrefabInfo prefabinfo) where T : Prefab
        {
            if (false == prefabs.TryGetValue(typeof(T), out var prefab)) throw new Exception($"prefab {typeof(T)} is not exist.");
            
            return prefab.Processing(AddActor(), prefabinfo);
        }

        /// <summary>
        /// 获取 Actor
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>Actor</returns>
        public Actor GetActor(ulong id)
        {
            // Actor 不存在
            if (false == info.actors.Contains(id)) return default;
            if (false == info.actordict.TryGetValue(id, out var actor)) return default;
            
            return actor;
        }
        
        /// <summary>
        /// 移除 Actor
        /// 不是立即执行, 而是添加入移除列表, 等待帧末执行回收 (Stage.RmvActors)
        /// </summary>
        /// <param name="id">ActorID</param>
        public void RmvActor(ulong id)
        {
            if (info.rmvactors.Contains(id)) return;
            info.rmvactors.Add(id);
        }

        /// <summary>
        /// 添加 Actor
        /// </summary>
        /// <returns>Actor</returns>
        public Actor AddActor()
        {
            // 生成一个 Actor
            info.increment++;
            var actor = ObjectCache.Get<Actor>();
            info.actors.Add(info.increment);
            info.actordict.Add(info.increment, actor);
            
            actor.Assemble(info.increment, this);
            // 默认携带 Tag 行为. 写入 ActorType 为 NONE
            actor.AddBehavior<Tag>().Set(TAG_DEFINE.ACTOR_TYPE, ACTOR_DEFINE.NONE);
            
            return actor;
        }

        /// <summary>
        /// 获取指定类型的所有 Behavior 列表
        /// </summary>
        /// <param name="behaviors">所有 Behavior 列表</param>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekBehaviors<T>(out List<T> behaviors) where T : Behavior
        {
            behaviors = GetBehaviors<T>();
        
            return null != behaviors;
        }
        
        /// <summary>
        /// 获取指定类型的所有 Behavior 列表
        /// </summary>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>所有 Behavior 列表</returns>
        public List<T> GetBehaviors<T>() where T : Behavior
        {
            var type = typeof(T);
            if (false == info.behaviors.TryGetValue(type, out var list) || 0 == list.Count) return default;

            // 根据类型去获取所有 Behavior
            var result = ObjectCache.Get<List<T>>();
            foreach (var behavior in list) result.Add(behavior as T);

            return result;
        }

        /// <summary>
        /// 获取指定类型的所有 Behavior 列表
        /// <param name="type">类型</param>
        /// </summary>
        /// <returns>所有 Behavior 列表</returns>
        public List<Behavior> GetBehaviors(Type type)
        {
            if (false == info.behaviors.TryGetValue(type, out var list) || 0 == list.Count) return default;

            // 根据类型去获取所有 Behavior
            return list;
        }

        /// <summary>
        /// 获取 Actor 所有 Behavior 列表
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="behaviors">Behavior 列表</param>
        /// <returns>YES/NO</returns>
        public bool SeekBehaviors(ulong id, out List<Behavior> behaviors)
        {
            behaviors = GetBehaviors(id);
        
            return null != behaviors;
        }

        /// <summary>
        /// 获取 Actor 所有 Behavior 列表
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>Behavior 列表</returns>
        public List<Behavior> GetBehaviors(ulong id)
        {
            if (false == info.behaviordict.TryGetValue(id, out var dict) || dict.Count == 0) return default;

            // 根据 ActorID 获取所有 Behavior
            var result = ObjectCache.Get<List<Behavior>>();
            foreach (var behavior in dict.Values) result.Add(behavior);

            return result;
        }

        /// <summary>
        /// 获取 Behavior
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="behavior">Behavior</param>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekBehavior<T>(ulong id, out T behavior) where T : Behavior, new()
        {
            behavior = GetBehavior<T>(id);
        
            return null != behavior;
        }

        /// <summary>
        /// 获取 Behavior
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="type">Behavior 类型</param>
        /// <param name="behavior">Behavior</param>
        /// <returns>YES/NO</returns>
        public bool SeekBehavior(ulong id, Type type, out Behavior behavior)
        {
            behavior = GetBehavior(id, type);
        
            return null != behavior;
        }

        /// <summary>
        /// 获取 Behavior
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>Behavior</returns>
        /// <exception cref="Exception">Actor 不存在</exception>
        public T GetBehavior<T>(ulong id) where T : Behavior
        {
            var behavior = GetBehavior(id, typeof(T));
            if (null == behavior) return default;

            return behavior as T;
        }
        
        /// <summary>
        /// 获取 Behavior
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="type">Behavior 类型</param>
        /// <returns>Behavior</returns>
        public Behavior GetBehavior(ulong id, Type type)
        {
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            if (false == info.behaviordict.TryGetValue(id, out var dict)) return default;
            if (false == dict.TryGetValue(type, out var behavior)) return default;
            
            return behavior;
        }

        /// <summary>
        /// 添加 Behavior
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>Behavior</returns>
        /// <exception cref="Exception">Actor 不存在 | Behavior 已存在</exception>
        public T AddBehavior<T>(ulong id) where T : Behavior, new()
        {
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            // 检查 Behavior 容器是否存在
            if (false == info.behaviors.TryGetValue(typeof(T), out var list)) info.behaviors.Add(typeof(T), list = ObjectCache.Get<List<Behavior>>());
            if (false == info.behaviordict.TryGetValue(id, out var dict)) info.behaviordict.Add(id, dict = ObjectCache.Get<Dictionary<Type, Behavior>>());
            // 检查 Behavior 是否已经存在容器中
            if (dict.ContainsKey(typeof(T))) throw new Exception($"behavior {typeof(T)} is exist.");
            
            var behavior = ObjectCache.Get<T>();
            list.Add(behavior);
            dict.Add(typeof(T), behavior);
            behavior.Assemble(this, GetActor(id));

            return behavior;
        }

        /// <summary>
        /// 获取 BehaviorInfo
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="info">BehaviorInfo</param>
        /// <typeparam name="T">BehaviorInfo 类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekBehaviorInfo<T>(ulong id, out T info) where T : BehaviorInfo
        {
            info = GetBehaviorInfo<T>(id);
            
            return null != info;
        }

        /// <summary>
        /// 获取 BehaviorInfo
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <typeparam name="T">BehaviorInfo 类型</typeparam>
        /// <returns>BehaviorInfo</returns>
        public T GetBehaviorInfo<T>(ulong id) where T : BehaviorInfo
        {
            if (false == info.behaviorinfodict.TryGetValue(id, out var dict)) return default;
            if (false == dict.TryGetValue(typeof(T), out var behaviorinfo)) return default;

            return (T)behaviorinfo;
        }
        
        /// <summary>
        /// 添加 BehaviorInfo
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <typeparam name="T">BehaviorInfo 类型</typeparam>
        /// <returns>BehaviorInfo</returns>
        /// <exception cref="Exception">Actor 不存在 | BehaviorInfo 已存在</exception>
        public T AddBehaviorInfo<T>(ulong id) where T : BehaviorInfo, new()
        {
            // 检查 Actor 是否存在
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            // 检查 BehaviorInfo 容器是否存在
            if (false == info.behaviorinfodict.TryGetValue(id, out var dict)) info.behaviorinfodict.Add(id, dict = ObjectCache.Get<Dictionary<Type, BehaviorInfo>>());
            // 检查 BehaviorInfo 是否已经存在容器中
            if (dict.ContainsKey(typeof(T))) throw new Exception($"behaviorinfo {typeof(T)} is exist.");

            var behaviorinfo = ObjectCache.Get<T>();
            dict.Add(typeof(T), behaviorinfo);
            behaviorinfo.Ready(id);
            
            return behaviorinfo;
        }
    }
}