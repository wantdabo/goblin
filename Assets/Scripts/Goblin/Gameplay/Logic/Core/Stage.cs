using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Common.GPDatas;
using Goblin.Gameplay.Logic.Prefabs;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Logic.RIL.DIFF;
using Goblin.Gameplay.Logic.RIL.EVENT;
using Kowtow.Math;
using Config = Goblin.Gameplay.Logic.Behaviors.Config;
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
        /// 时间流逝
        /// </summary>
        public FP elapsed => info.elapsed;
        /// <summary>
        /// 时间缩放
        /// </summary>
        public FP timescale => info.timescale;
        /// <summary>
        /// Stage 缓存
        /// </summary>
        private StageCache cache { get; set; }
        /// <summary>
        /// Stage 数据
        /// </summary>
        private StageInfo info { get; set; }
        /// <summary>
        /// 快照
        /// </summary>
        private StageInfo snapshot { get; set; }
        /// <summary>
        /// 是否有快照
        /// </summary>
        public bool hassnapshot => null != snapshot;
        /// <summary>
        /// 快照帧号
        /// </summary>
        public uint snapshotframe => null != snapshot ? snapshot.frame : 0;
        /// <summary>
        /// 初始化 Stage 的游戏数据
        /// </summary>
        public GPStageData sdata { get; set; }
        /// <summary>
        /// 配置
        /// </summary>
        public Config cfg => GetBehavior<Config>(sa);
        /// <summary>
        /// 座位
        /// </summary>
        public Seat seat => GetBehavior<Seat>(sa);
        /// <summary>
        /// 随机数
        /// </summary>
        public Random random => GetBehavior<Random>(sa);
        /// <summary>
        /// 属性数值计算
        /// </summary>
        public AttributeCalc calc => GetBehavior<AttributeCalc>(sa);
        /// <summary>
        /// 碰撞检测
        /// </summary>
        public Detection detection => GetBehavior<Detection>(sa);
        /// <summary>
        /// 管线流
        /// </summary>
        public Flow flow => GetBehavior<Flow>(sa);
        /// <summary>
        /// 渲染指令同步
        /// </summary>
        public RILSync rilsync => GetBehavior<RILSync>(sa);
        /// <summary>
        /// 预制创建器集合
        /// </summary>
        public Dictionary<Type, Prefab> prefabs { get; set; }
        /// <summary>
        /// 对外暴露抛出 RIL 的事件
        /// </summary>
        public Action<IRIL> onril { get; set; }
        /// <summary>
        /// 对外暴露抛出 RIL_DIFF 的事件
        /// </summary>
        public Action<IRIL_DIFF> ondiff { get; set; }
        /// <summary>
        /// 对外暴露抛出 RIL_EVENT 的事件
        /// </summary>
        public Action<IRIL_EVENT> onevent { get; set; }

        /// <summary>
        /// 初始化 Stage
        /// </summary>
        /// <param name="data">Stage 初始化的游戏数据</param>
        /// <returns>Stage</returns>
        /// <exception cref="Exception">初始化数据为空 || 重复初始化</exception>
        public Stage Initialize(GPStageData data)
        {
            if (null != info) throw new Exception("you cannot initialize more than once.");

            // StageCache 初始化
            cache = new StageCache().Initialize();
            
            // 构建 StageInfo, 因为 Stage 的数据也是走 BehaviorInfo, 无法通过自举的方式走 API 添加
            // 悖论存在, 此处手动添加
            info = ObjectCache.Ensure<StageInfo>();
            info.Ready(sa);
            info.state = StageState.Initialized;
            
            // 操作 Sa Behavior
            AddActor(sa);
            GetBehavior<Tag>(sa).Set(TAG_DEFINE.ACTOR_TYPE, ACTOR_DEFINE.STAGE);
            AddBehavior<Config>(sa);
            AddBehavior<Seat>(sa);
            AddBehavior<Random>(sa).Initialze(data.seed);
            AddBehavior<AttributeCalc>(sa);
            AddBehavior<Detection>(sa);
            AddBehavior<SkillBinding>(sa);
            AddBehavior<Flow>(sa);
            AddBehavior<Bullet>(sa);
            AddBehavior<RILSync>(sa);
            
            // 添加预制创建器
            Prefabs();
            // 构建初始化 Stage
            Building(data);

            return this;
        }
        
        /// <summary>
        /// 销毁 Stage
        /// </summary>
        public void Dispose()
        {
            if (StageState.Stopped != info.state) return;
            info.state = StageState.Disposed;
            
            // 回收 Actor
            cache.rmvactors.Clear();
            cache.rmvactors.AddRange(info.actors);
            RecycleActors();
            
            // 卸载 Prefabs
            foreach (var prefab in prefabs.Values) prefab.Unload();
            prefabs.Clear();
            ObjectCache.Set(prefabs);
            
            // StageInfo 回收
            info.Reset();
            ObjectCache.Set(info);

            // StageCache 回收
            cache.Dispose();
        }

        /// <summary>
        /// 快照
        /// </summary>
        public void Snapshot()
        {
            if (null != snapshot)
            {
                snapshot.Reset();
                ObjectCache.Set(snapshot);
            }
            
            snapshot = info.Clone() as StageInfo;
        }
        
        /// <summary>
        /// 恢复
        /// </summary>
        public void Restore()
        {
            if (null == snapshot) return;
            if (snapshot.frame == info.frame) return;
            cache.rmvactors.Clear();
            cache.rmvactors.AddRange(info.actors);
            RecycleActors();
            
            info.Reset();
            ObjectCache.Set(info);
            info = snapshot.Clone() as StageInfo;
            
            foreach (var behaviorinfos in info.behaviorinfos.Values)
            {
                foreach (var behaviorinfo in behaviorinfos)
                {
                    if (false == cache.behaviorinfodict.TryGetValue(behaviorinfo.id, out var dict)) cache.behaviorinfodict.Add(behaviorinfo.id, dict = ObjectCache.Ensure<Dictionary<Type, BehaviorInfo>>());
                    dict.Add(behaviorinfo.GetType(), behaviorinfo);
                }
            }
            
            foreach (var id in info.actors)
            {
                AddActor(id);
                if (false == info.behaviortypes.TryGetValue(id, out var types)) continue;
                foreach (var type in types)
                {
                    if (SeekBehavior(id, type, out _)) continue;
                    AddBehavior(id, type);
                }
            }

            rilsync.Execute();
        }

        /// <summary>
        /// 添加预制创建器
        /// </summary>
        /// <exception cref="Exception">不能重复添加</exception>
        private void Prefabs()
        {
            void Prefab<T, E>() where T : Prefab, new() where E : IPrefabInfo, new()
            {
                prefabs.Add(typeof(E), ObjectCache.Ensure<T>().Load(this));
            }
            prefabs = ObjectCache.Ensure<Dictionary<Type, Prefab>>();
            Prefab<FlowPrefab, FlowPrefabInfo>();
            Prefab<HeroPrefab, HeroPrefabInfo>();
            Prefab<BulletPrefab, BulletPrefabInfo>();
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
        /// 输入
        /// </summary>
        /// <param name="id">座位 ID</param>
        /// <param name="inputType">按键类型</param>
        /// <param name="press">摁下之后 -> TRUE</param>
        /// <param name="dire">按键的方向</param>
        public void SetInput(ulong id, ushort inputType, bool press, GPVector2 dire)
        {
            var actor = seat.GetActor(id);
            if (false == info.actors.Contains(actor)) return;
            
            var gamepad = GetBehavior<Gamepad>(actor);
            if (null == gamepad) return;
            
            gamepad.SetInput(inputType, press, dire.ToFPVector2());
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
            foreach (var type in TICK_DEFINE.TICK_TYPE_LIST)
            {
                if (false == SeekBehaviors(type, out var behaviors)) continue;
                foreach (var behavior in behaviors)
                {
                    if (SeekBehaviorInfo(behavior.actor.id, out TickerInfo ticker))
                    {
                        behavior.Tick(ticker.timescale * info.tick);
                        continue;
                    }

                    behavior.Tick(info.tick);
                }
                behaviors.Clear();
                ObjectCache.Set(behaviors);
            }

            // 帧末调度
            foreach (var type in TICK_DEFINE.TICK_TYPE_LIST)
            {
                if (false == SeekBehaviors(type, out var behaviors)) continue;
                foreach (var behavior in behaviors)
                {
                    behavior.EndTick();
                }
                behaviors.Clear();
                ObjectCache.Set(behaviors);
            }
            
            RecycleActors();
            
            // TODO 测试跳字, 请记得删除
            var e = ObjectCache.Ensure<RIL_EVENT_DAMAGE>();
            e.from = 1;
            e.to = 1;
            e.crit = true;
            e.damage = 10086;
            rilsync.Send(e);
        }

        /// <summary>
        /// 回收/销毁 RmvActors
        /// </summary>
        private void RecycleActors()
        {
            // 拆解 Behavior
            foreach (var rmvactor in cache.rmvactors)
            {
                if (false == SeekBehaviors(rmvactor, out var behaviors)) continue;
                foreach (var behavior in behaviors) behavior.Disassemble();
            }

            // 重置 BehaviorInfo
            foreach (var rmvactor in cache.rmvactors)
            {
                if (false == SeekBehaviorInfos(rmvactor, out var behaviorinfos)) continue;
                foreach (var behaviorinfo in behaviorinfos) behaviorinfo.Reset();
            }
            
            // 拆解 Actor
            foreach (var rmvactor in cache.rmvactors)
            {
                var actor = GetActor(rmvactor);
                if (null == actor) continue;
                actor.Disassemble();
            }
            
            // 回收清理
            foreach (var rmvactor in cache.rmvactors)
            {
                // 回收清理 Behavior
                if (cache.behaviordict.TryGetValue(rmvactor, out var dict))
                {
                    foreach (var behavior in dict.Values)
                    {
                        if (cache.behaviors.TryGetValue(behavior.GetType(), out var list))
                        {
                            list.Remove(behavior);
                            if (0 == list.Count)
                            {
                                ObjectCache.Set(list);
                                cache.behaviors.Remove(behavior.GetType());
                            }
                        }
                        ObjectCache.Set(behavior);  
                    }
                    dict.Clear();
                    ObjectCache.Set(dict);
                    cache.behaviordict.Remove(rmvactor);
                }
                if (info.behaviortypes.TryGetValue(rmvactor, out var types))
                {
                    types.Clear();
                    ObjectCache.Set(types);
                    
                    info.behaviortypes.Remove(rmvactor);
                }
                
                // 回收清理 BehaviorInfo
                if (cache.behaviorinfodict.TryGetValue(rmvactor, out var infodict))
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
                        ObjectCache.Set(behaviorinfo);
                    }
                    infodict.Clear();
                    ObjectCache.Set(infodict);
                    cache.behaviorinfodict.Remove(rmvactor);
                }
            
                // 回收清理 Actor
                if (cache.actordict.TryGetValue(rmvactor, out var actor))
                {
                    info.actors.Remove(rmvactor);
                    cache.actordict.Remove(rmvactor);
                }
            }
            
            cache.rmvactors.Clear();
        }

        /// <summary>
        /// 根据预制创建器构建一个 Actor
        /// </summary>
        /// <param name="prefabinfo">预制构建器数据</param>
        /// <typeparam name="T">预制构建器类型</typeparam>
        /// <returns>Actor</returns>
        /// <exception cref="Exception">预制构建器类型不存在</exception>
        public Actor Spawn<T>(T prefabinfo) where T : IPrefabInfo
        {
            if (false == prefabs.TryGetValue(typeof(T), out var prefab)) throw new Exception($"prefab {typeof(T)} is not exist.");

            var state = ObjectCache.Ensure<PrefabInfoState<T>>();
            state.info = prefabinfo;
            var actor = prefab.Processing(GenActor(), state);
            state.Reset();
            ObjectCache.Set(state);

            return actor;
        }

        /// <summary>
        /// 获取 Actor
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>Actor</returns>
        public Actor GetActor(ulong id)
        {
            if (false == cache.actordict.TryGetValue(id, out var actor)) return default;
            
            return actor;
        }
        
        /// <summary>
        /// 移除 Actor
        /// 不是立即执行, 而是添加入移除列表, 等待帧末执行回收 (Stage.RmvActors)
        /// </summary>
        /// <param name="id">ActorID</param>
        public void RmvActor(ulong id)
        {
            if (cache.rmvactors.Contains(id)) return;
            cache.rmvactors.Add(id);
            
            DiffActor(id, RIL_DEFINE.DIFF_DEL);
            
            seat.Standup(id);
        }

        /// <summary>
        /// 生成 Actor
        /// </summary>
        /// <returns>Actor</returns>
        public Actor GenActor()
        {
            // 生成一个 Actor
            var actor = AddActor(++info.increment);
            
            DiffActor(actor.id, RIL_DEFINE.DIFF_NEW);
            
            return actor;
        }

        /// <summary>
        /// 添加 Actor
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>Actor</returns>
        /// <exception cref="Exception">Actor 已存在</exception>
        private Actor AddActor(ulong id)
        {
            var actor = ObjectCache.Ensure<Actor>();
            if (false == info.actors.Contains(id)) info.actors.Add(id);
            if (cache.actordict.ContainsKey(id)) throw new Exception($"actor {id} already exists.");
            cache.actordict.Add(id, actor);
            
            actor.Assemble(id, this);
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
            if (false == cache.behaviors.TryGetValue(type, out var list) || 0 == list.Count) return default;

            // 根据类型去获取所有 Behavior
            var result = ObjectCache.Ensure<List<T>>();
            foreach (var behavior in list) result.Add(behavior as T);

            return result;
        }
        
        /// <summary>
        /// 获取指定类型的所有 Behavior 列表
        /// </summary>
        /// <param name="type">Behavior 类型</param>
        /// <param name="behaviors">所有 Behavior 列表</param>
        /// <returns>YES/NO</returns>
        public bool SeekBehaviors(Type type, out List<Behavior> behaviors)
        {
            behaviors = GetBehaviors(type);
        
            return null != behaviors;
        }

        /// <summary>
        /// 获取指定类型的所有 Behavior 列表
        /// <param name="type">Behavior 类型</param>
        /// </summary>
        /// <returns>所有 Behavior 列表</returns>
        public List<Behavior> GetBehaviors(Type type)
        {
            if (false == cache.behaviors.TryGetValue(type, out var list) || 0 == list.Count) return default;
            var result = ObjectCache.Ensure<List<Behavior>>();
            foreach (var behavior in list) result.Add(behavior);

            return result;
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
            if (false == info.behaviortypes.TryGetValue(id, out var types)) return default;

            // 根据 ActorID 获取所有 Behavior
            var result = ObjectCache.Ensure<List<Behavior>>();
            foreach (var type in types)
            {
                if (false == SeekBehavior(id, type, out var behavior)) continue;
                result.Add(behavior);
            }

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
            if (false == cache.behaviordict.TryGetValue(id, out var dict)) return default;
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
            return AddBehavior(id, typeof(T)) as T;
        }

        /// <summary>
        /// 添加 Behavior
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="type">Behavior 类型</param>
        /// <returns>Behavior</returns>
        /// <exception cref="Exception">Actor 不存在 | Behavior 已存在</exception>
        private Behavior AddBehavior(ulong id, Type type)
        {
            // 检查 Behavior 容器是否存在
            if (false == cache.behaviors.TryGetValue(type, out var list)) cache.behaviors.Add(type, list = ObjectCache.Ensure<List<Behavior>>());
            if (false == cache.behaviordict.TryGetValue(id, out var dict)) cache.behaviordict.Add(id, dict = ObjectCache.Ensure<Dictionary<Type, Behavior>>());
            // 检查 Behavior 是否已经存在容器中
            if (dict.ContainsKey(type)) throw new Exception($"behavior {type} is exist.");
            if (false == info.behaviortypes.TryGetValue(id, out var types)) info.behaviortypes.Add(id, types = ObjectCache.Ensure<List<Type>>());
            
            var behavior = ObjectCache.Ensure(type) as Behavior;
            if (false == types.Contains(type)) types.Add(type);
            dict.Add(type, behavior);
            list.Add(behavior);
            
            behavior.Initialize(this, id);
            // 排序
            list.Sort((a, b) =>
            {
                return a.id.CompareTo(b.id);
            });
            behavior.Assemble();

            return behavior;
        }

        /// <summary>
        /// 获取指定类型的所有 BehaviorInfo 列表
        /// </summary>
        /// <param name="infos">BehaviorInfo 列表</param>
        /// <typeparam name="T">BehaviorInfo 类型</typeparam>
        /// <returns>YES/NO</returns>
        public bool SeekBehaviorInfos<T>(out List<T> infos) where T : BehaviorInfo
        {
            infos = GetBehaviorInfos<T>();
        
            return null != infos;
        }

        /// <summary>
        /// 获取指定类型的所有 BehaviorInfo 列表
        /// </summary>
        /// <typeparam name="T">BehaviorInfo 类型</typeparam>
        /// <returns>BehaviorInfo 列表</returns>
        /// 
        public List<T> GetBehaviorInfos<T>() where T : BehaviorInfo
        {
            if (false == info.behaviorinfos.TryGetValue(typeof(T), out var infos)) return default;
            var result = ObjectCache.Ensure<List<T>>();
            foreach (var i in infos) result.Add(i as T);
            
            return result;
        }

        /// <summary>
        /// 获取 Actor 所有 BehaviorInfo 列表
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="infos">BehaviorInfo 列表</param>
        /// <returns>YES/NO</returns>
        public bool SeekBehaviorInfos(ulong id, out List<BehaviorInfo> infos)
        {
            infos = GetBehaviorInfos(id);
        
            return null != infos;
        }

        /// <summary>
        /// 获取 Actor 所有 BehaviorInfo 列表
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>BehaviorInfo 列表</returns>
        public List<BehaviorInfo> GetBehaviorInfos(ulong id)
        {
            if (false == cache.behaviorinfodict.TryGetValue(id, out var dict)) return default;
            var result = ObjectCache.Ensure<List<BehaviorInfo>>();
            foreach (var i in dict.Values) result.Add(i);
            
            return result;
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
            if (id == sa && typeof(T) == typeof(StageInfo)) return info as T;
            if (false == cache.behaviorinfodict.TryGetValue(id, out var dict)) return default;
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
            // 检查 BehaviorInfo 容器是否存在
            if (false == cache.behaviorinfodict.TryGetValue(id, out var dict)) cache.behaviorinfodict.Add(id, dict = ObjectCache.Ensure<Dictionary<Type, BehaviorInfo>>());
            // 检查 BehaviorInfo 是否已经存在容器中
            if (dict.ContainsKey(typeof(T))) throw new Exception($"behaviorinfo {typeof(T)} is exist.");
            // 初始化 BehaviorInfos
            if (false == info.behaviorinfos.TryGetValue(typeof(T), out var list)) info.behaviorinfos.Add(typeof(T), list = ObjectCache.Ensure<List<BehaviorInfo>>());

            var behaviorinfo = ObjectCache.Ensure<T>();
            dict.Add(typeof(T), behaviorinfo);
            list.Add(behaviorinfo);
            behaviorinfo.Ready(id);
            
            return behaviorinfo;
        }
        
        /// <summary>
        /// 构建初始化 Stage
        /// </summary>
        /// <param name="data">Stage 初始化的游戏数据</param>
        public void Building(GPStageData data)
        {
            this.sdata = data;
            foreach (var player in data.players)
            {
                var hero = Spawn(new HeroPrefabInfo
                {
                    hero = player.hero,
                    spatial = new()
                    {
                        position = player.position.ToFPVector3(),
                        euler = player.euler.ToFPVector3(),
                        scale = player.scale * cfg.int2fp,
                    }
                });
                hero.AddBehavior<Gamepad>();
                // 入座
                seat.Sitdown(player.seat, hero.id);
            }
        }
        
        /// <summary>
        /// Actor 差异
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="token">RIL 差异标记</param>
        private void DiffActor(ulong id, byte token)
        {
            var diff = ObjectCache.Ensure<RIL_DIFF_ACTOR>();
            diff.Ready(sa, token);
            diff.target = id;
            rilsync.Send(diff);
        }
        
        /// <summary>
        /// Stage 缓存
        /// </summary>
        private sealed class StageCache
        {
            /// <summary>
            /// Actor 列表, 键为 ActorID, 值为 Actor 实例
            /// </summary>
            public Dictionary<ulong, Actor> actordict { get; set; }
            /// <summary>
            /// 行为列表, 键为 ActorID, 值为该 Actor 上的所有行为
            /// </summary>
            public Dictionary<ulong, Dictionary<Type, Behavior>> behaviordict { get; set; }
            /// <summary>
            /// 行为列表, 键为行为类型, 值为该行为类型的所有 Behavior 列表
            /// </summary>
            public Dictionary<Type, List<Behavior>> behaviors { get; set; }
            /// <summary>
            /// 行为信息列表, 键为 ActorID, 值为该 Actor 上的所有行为信息
            /// </summary>
            public Dictionary<ulong, Dictionary<Type, BehaviorInfo>> behaviorinfodict { get; set; }
            /// <summary>
            /// RmvActor 列表
            /// </summary>
            public List<ulong> rmvactors { get; set; }
        
            /// <summary>
            /// 初始化 StageCache
            /// </summary>
            /// <returns>StageCache</returns>
            public StageCache Initialize()
            {
                actordict = ObjectCache.Ensure<Dictionary<ulong, Actor>>();
                behaviordict = ObjectCache.Ensure<Dictionary<ulong, Dictionary<Type, Behavior>>>();
                behaviors = ObjectCache.Ensure<Dictionary<Type, List<Behavior>>>();
                behaviorinfodict = ObjectCache.Ensure<Dictionary<ulong, Dictionary<Type, BehaviorInfo>>>();
                rmvactors = ObjectCache.Ensure<List<ulong>>();

                return this;
            }
        
            /// <summary>
            /// 销毁 StageCache
            /// </summary>
            public void Dispose()
            {
                actordict.Clear();
                ObjectCache.Set(actordict);
            
                behaviordict.Clear();
                ObjectCache.Set(behaviordict);
            
                behaviors.Clear();
                ObjectCache.Set(behaviors);
            
                behaviorinfodict.Clear();
                ObjectCache.Set(behaviorinfodict);
            
                rmvactors.Clear();
                ObjectCache.Set(rmvactors);
            }
        }
    }
}