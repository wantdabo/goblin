using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.GameplayDatas;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Logic.Translators;
using Goblin.Gameplay.Logic.Translators.Common;
using Goblin.Gameplay.Prefabs;
using Goblin.Gameplay.Prefabs.Common;
using Kowtow.Math;
using Attribute = Goblin.Gameplay.Logic.Translators.Attribute;
using Random = Goblin.Gameplay.Logic.Behaviors.Random;
using StateMachine = Goblin.Gameplay.Logic.Behaviors.StateMachine;

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
        private const ulong sa = ulong.MaxValue;
        /// <summary>
        /// Stage 数据
        /// </summary>
        private StageInfo info { get; set; }
        /// <summary>
        /// Stage 当前的运行状态
        /// </summary>
        public StageState state => info.state;
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
        /// 行为集合
        /// </summary>
        private Dictionary<uint, List<Behavior>> behaviors { get; set; }
        /// <summary>
        /// RIL 翻译器集合
        /// </summary>
        private Dictionary<Type, Translator> translators { get; set; }
        /// <summary>
        /// 预制创建器集合
        /// </summary>
        private Dictionary<Type, Prefab> prefabs { get; set; }
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
            
            // 添加 Stage 行为
            AddBehavior<Random>(sa).Initialze(data.seed);
            AddBehavior<RILSync>(sa);
            AddBehavior<Tag>(sa).Set(TAG_DEFINE.ACTOR_TYPE, ACTOR_DEFINE.STAGE);
            // 添加渲染指令翻译器
            Translators();
            // 添加预制创建器
            Prefabs();

            this.gpdata = data;
            // TODO 临时代码, 后续挪到一个单独的地方, 构建初始化世界
            foreach (var player in data.players)
            {
                var actor = Spawn<Hero>(new HeroInfo
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
                actor.AddBehavior<Gamepad>();
            }
        }
        
        /// <summary>
        /// 销毁 Stage
        /// </summary>
        public void Dispose()
        {
            if (StageState.Stopped != info.state) return;
            info.state = StageState.Disposed;
            
            // 拆解所有
            Disassembles();
            
            // 回收 Actor
            info.rmvactors.Clear();
            info.rmvactors.AddRange(info.actors);
            RecycleActors();
            
            // StageInfo 回收
            info.Reset();
            ObjectCache.Set(info);
            
            // 渲染指令翻译器回收
            translators.Clear();
            ObjectCache.Set(translators);
            
            // 预制创建器回收
            prefabs.Clear();
            ObjectCache.Set(prefabs);
        }

        /// <summary>
        /// 添加渲染指令翻译器
        /// </summary>
        /// <exception cref="Exception">不能重复添加</exception>
        private void Translators()
        {
            if (null != translators) throw new Exception("you cannot initialize more than once.");
            
            translators = ObjectCache.Get<Dictionary<Type, Translator>>();
            translators.Add(typeof(AttributeInfo), new Attribute().Initialize(this));
            translators.Add(typeof(SpatialInfo), new Spatial().Initialize(this));
            translators.Add(typeof(StateMachineInfo), new Translators.StateMachine().Initialize(this));
        }

        /// <summary>
        /// 添加预制创建器
        /// </summary>
        /// <exception cref="Exception">不能重复添加</exception>
        private void Prefabs()
        {
            if (null != prefabs) throw new Exception("you cannot initialize more than once.");
            
            prefabs = ObjectCache.Get<Dictionary<Type, Prefab>>();
            prefabs.Add(typeof(Hero), new Hero());
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
        /// Tick 驱动
        /// </summary>
        public void Tick()
        {
            if (StageState.Ticking != info.state) return;
            
            // 帧号递增 & 时间流逝
            info.frame++;
            info.elapsed += info.tick;
            
            // 获取所有 Ticker, 进行 Tick 驱动
            // 翻译 BehaviorInfo 成为 RIL 渲染指令
            var tickers = GetBehaviors<Ticker>();
            if (null != tickers)
            {
                // Tick 驱动
                foreach (var ticker in tickers) ticker.Tick(info.tick);
                // 翻译 RIL 渲染指令
                Translate();
                // 帧末调度
                foreach (var ticker in tickers) ticker.TickEnd();
                
                tickers.Clear();
                ObjectCache.Set(tickers);
            }
            
            // 拆解回收 Behavior & Actor
            Disassembles();
            RecycleActors();
        }

        /// <summary>
        /// 翻译 RIL 渲染指令, 遍历整个 Stage BehaviorInfo 来生成渲染指令
        /// </summary>
        private void Translate()
        {
            foreach (var kv in info.behaviorinfodict)
            {
                foreach (var kv2 in kv.Value)
                {
                    if (false == translators.TryGetValue(kv2.Key, out var translator)) continue;
                    translator.Translate(kv2.Value);
                }
            }
        }
        
        /// <summary>
        /// 拆解 Behavior & Actor
        /// </summary>
        private void Disassembles()
        {
            foreach (var id in info.actors)
            {
                // 拆解 Behavior 返回对象池
                foreach (var type in GetBehaviorTypes(id))
                {
                    Behavior behavior = GetBehavior(id, type);
                    behavior.Disassemble();
                    ObjectCache.Set(behavior);
                }
                
                // 拆解 Actor 返回对象池
                Actor actor = GetActor(id);
                actor.Disassemble();
                ObjectCache.Set(actor);
            }
            
            // 回收 Behavior & Actor 容器
            foreach (var kv in info.behaviorassembleds)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            
            // 不回收到对象池, 下一帧还要用
            info.behaviorassembleds.Clear();
            info.actorassembleds.Clear();
        }
        
        /// <summary>
        /// 回收/销毁 RmvActors
        /// </summary>
        private void RecycleActors()
        {
            foreach (var rmvactor in info.rmvactors)
            {
                // 回收 Actor 的 BehaviorInfos
                if (info.behaviorinfodict.TryGetValue(rmvactor, out var infos))
                {
                    foreach (var behaviorinfo in infos.Values)
                    {
                        behaviorinfo.Reset();
                        ObjectCache.Set(behaviorinfo);
                    }
                    infos.Clear();
                    ObjectCache.Set(infos);
                    info.behaviorinfodict.Remove(rmvactor);
                }

                // 回收 Actor 的 Behaviors 容器
                if (info.behaviors.TryGetValue(rmvactor, out var types))
                {
                    foreach (var type in types)
                    {
                        // 移除 Behavior Owner
                        if (false == info.behaviorowners.TryGetValue(type, out var owners)) continue;
                        owners.Remove(rmvactor);

                        // 如果容器为空, 回收到对象池
                        if (0 == owners.Count)
                        {
                            ObjectCache.Set(owners);
                            info.behaviorowners.Remove(type);
                        }
                    }
                    
                    // 回收 Actor 的 BehaviorType 容器
                    types.Clear();
                    ObjectCache.Set(types);
                    info.behaviors.Remove(rmvactor);
                }

                // 从 Actors 中移除 Actor
                info.actors.Remove(rmvactor);
            }
            
            // 清空 RmvActors
            info.rmvactors.Clear();
        }
        
        /// <summary>
        /// 根据预制创建器构建一个 Actor
        /// </summary>
        /// <param name="info">预制构建器数据</param>
        /// <typeparam name="T">预制构建器类型</typeparam>
        /// <returns>Actor</returns>
        /// <exception cref="Exception">预制构建器类型不存在</exception>
        public Actor Spawn<T>(IPrefabInfo info) where T : Prefab
        {
            if (false == prefabs.TryGetValue(typeof(T), out var prefab)) throw new Exception($"prefab {typeof(T)} is not exist.");
            
            return prefab.Processing(GenActor(), info);
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
            
            // 返回当前帧已经组装好的 Actor
            if (info.actorassembleds.TryGetValue(id, out var actor)) return actor;

            // 组装 Actor, 并存入组装列表
            actor = ObjectCache.Get<Actor>();
            actor.Assemble(id,this);
            info.actorassembleds.Add(id, actor);
            
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
        public Actor GenActor()
        {
            // 生成一个 Actor
            info.increment++;
            info.actors.Add(info.increment);
            var actor = GetActor(info.increment);
            // 默认携带 Tag 行为. 写入 ActorType 为 NONE
            actor.AddBehavior<Tag>().Set(TAG_DEFINE.ACTOR_TYPE, ACTOR_DEFINE.NONE);
            
            return actor;
        }
        
        /// <summary>
        /// 根据 ActorID 获取 Behavior 类型列表
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="types">Behavior 类型列表</param>
        /// <returns>YES/NO</returns>
        public bool SeekBehaviorTypes(ulong id, out List<Type> types)
        {
            types = GetBehaviorTypes(id);

            return null != types;
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
        /// 根据 ActorID 获取 Behavior 类型列表
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <returns>Behavior 类型列表</returns>
        /// <exception cref="Exception">Actor 不存在</exception>
        private List<Type> GetBehaviorTypes(ulong id)
        {
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            if (false == info.behaviors.TryGetValue(id, out var list)) return default;

            return list;
        }

        /// <summary>
        /// 获取指定类型的所有 Behavior 列表
        /// </summary>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>所有 Behavior 列表</returns>
        private List<T> GetBehaviors<T>() where T : Behavior
        {
            var type = typeof(T);
            if (false == info.behaviorowners.TryGetValue(type, out var owners) || 0 == owners.Count) return default;

            // 根据类型去获取 ActorID 获取
            var list = ObjectCache.Get<List<T>>();
            foreach (var owner in owners) list.Add(GetBehavior(owner, type) as T);

            return list;
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
        /// <param name="type">Behavior 类型</param>
        /// <returns>Behavior</returns>
        private Behavior GetBehavior(ulong id, Type type)
        {
            if (typeof(Gamepad) == type) return GetBehavior<Gamepad>(id);
            else if (typeof(Movement) == type) return GetBehavior<Movement>(id);
            else if (typeof(Random) == type) return GetBehavior<Random>(id);
            else if (typeof(RILSync) == type) return GetBehavior<RILSync>(id);
            else if (typeof(StateMachine) == type) return GetBehavior<StateMachine>(id);
            else if (typeof(Tag) == type) return GetBehavior<Tag>(id);
            else if (typeof(Ticker) == type) return GetBehavior<Ticker>(id);

            return default;
        }

        /// <summary>
        /// 获取 Behavior
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <typeparam name="T">Behavior 类型</typeparam>
        /// <returns>Behavior</returns>
        /// <exception cref="Exception">Actor 不存在</exception>
        private T GetBehavior<T>(ulong id) where T : Behavior, new()
        {
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            
            // 没在 Behavior 列表中找到
            if (false == info.behaviors.TryGetValue(id, out var list) || false == list.Contains(typeof(T))) return default;

            // 返回当前帧已经组装好的 Behavior
            if (info.behaviorassembleds.TryGetValue(id, out var dict) && dict.TryGetValue(typeof(T), out var b)) return b as T;

            var behavior = ObjectCache.Get<T>();
            behavior.Assemble(this, id);
            
            // 当前帧组装集合 Behavior 集合不存在
            // 构建集合
            if (null == dict)
            {
                dict = ObjectCache.Get<Dictionary<Type, Behavior>>();
                info.behaviorassembleds.Add(id, dict);
            }
            
            // 存入当前帧组装集合
            dict.Add(typeof(T), behavior);

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
            // 检查 Actor 是否存在
            if (false == info.actors.Contains(id)) throw new Exception($"actor {id} is not exist.");
            // 检查 Behavior 容器是否存在
            if (false == info.behaviors.TryGetValue(id, out var list)) info.behaviors.Add(id, list = ObjectCache.Get<List<Type>>());
            // 检查 Behavior 是否已经存在容器中
            if (list.Contains(typeof(T))) throw new Exception($"behavior {typeof(T)} is exist.");

            if (false == info.behaviorowners.TryGetValue(typeof(T), out var owners)) info.behaviorowners.Add(typeof(T), owners = ObjectCache.Get<List<ulong>>());
            owners.Add(id);
            list.Add(typeof(T));
            
            return GetBehavior<T>(id);
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
        private T GetBehaviorInfo<T>(ulong id) where T : BehaviorInfo
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