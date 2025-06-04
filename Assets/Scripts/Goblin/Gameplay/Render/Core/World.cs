using System;
using System.Collections.Generic;
using System.Linq;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Batches;
using Goblin.Gameplay.Render.Cameras;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Core
{
    /// <summary>
    /// 世界/渲染, 负责容纳所有的渲染层的单位
    /// </summary>
    public sealed class World : Comp
    {
        /// <summary>
        /// Stage.ActorID, Stage 的数据走的也是 Actor/Behavior/BehaviorInfo 那一套
        /// 通过包装 Actor 的形式使用
        /// 所以 Stage 也是 Actor, 但它是一个特殊的 Actor, 它的 ID 是 ulong.MaxValue
        /// </summary>
        public ulong sa => ulong.MaxValue;
        /// <summary>
        /// 座位 ID
        /// </summary>
        public ulong selfseat { get; private set; } = 0;
        /// <summary>
        /// 自我
        /// </summary>
        public ulong self {
            get
            {
                if (false == rilbucket.SeekRIL<RIL_SEAT>(sa, out var ril)) return 0;
                if (false == ril.seatdict.TryGetValue(selfseat, out var actor)) return 0;
                
                return actor;
            }
        }
        /// <summary>
        /// 事件订阅派发者
        /// </summary>
        public Eventor eventor { get; private set; }
        /// <summary>
        /// Ticker/时间驱动器
        /// </summary>
        public Ticker ticker { get; private set; }
        /// <summary>
        /// 输入系统
        /// </summary>
        public InputSystem input { get; private set; }
        /// <summary>
        /// 桶
        /// </summary>
        public RILBucket rilbucket { get; private set; }
        /// <summary>
        /// 眼睛/摄像机
        /// </summary>
        public Eyes eyes { get; private set; }
        /// <summary>
        /// Agent 集合
        /// </summary>
        private Dictionary<ulong, Dictionary<Type, Agent>> agentdict { get; set; }
        /// <summary>
        /// 快照之后产生的 Agent
        /// </summary>
        private List<Agent> snapshotagents { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
            
            ticker = AddComp<Ticker>();
            ticker.Create();
            
            input = AddComp<InputSystem>();
            input.Initialize(this).Create();
            
            rilbucket = AddComp<RILBucket>();
            rilbucket.Initialize(this).Create();
            
            eyes = AddComp<Eyes>();
            eyes.Initialize(this).Create();

            Batches();
            
            agentdict = ObjectPool.Ensure<Dictionary<ulong, Dictionary<Type, Agent>>>();
            snapshotagents = ObjectPool.Ensure<List<Agent>>();
            
            ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            ticker.eventor.UnListen<TickEvent>(OnTick);
            
            // 回收所有 Agents
            foreach (var kv in agentdict)
            {
                foreach (var agent in kv.Value)
                {
                    agent.Value.Reset();
                    ObjectPool.Set(agent.Value);
                }
                
                kv.Value.Clear();
                ObjectPool.Set(kv.Value);
            }
            agentdict.Clear();
            ObjectPool.Set(agentdict);
            
            snapshotagents.Clear();
            ObjectPool.Set(snapshotagents);
        }
        
        /// <summary>
        /// 初始化世界
        /// </summary>
        /// <param name="selfseat">我的座位号</param>
        /// <returns>世界</returns>
        public World Initialize(ulong selfseat)
        {
            this.selfseat = selfseat;
            
            return this;
        }
        
        /// <summary>
        /// 创建批处理
        /// </summary>
        private void Batches()
        {
            AddComp<SpatialBatch>().Initialize(this).Create();
        }

        /// <summary>
        /// 拍摄
        /// </summary>
        public void Snapshot()
        {
            snapshotagents.Clear();
        }

        /// <summary>
        /// 恢复
        /// </summary>
        public void Restore()
        {
            var agents = ObjectPool.Ensure<List<Agent>>();
            agents.AddRange(snapshotagents);
            foreach (var agent in agents) RmvAgent(agent);
            agents.Clear();
            ObjectPool.Set(agents);
            snapshotagents.Clear();
            foreach (var kv in agentdict)
            {
                foreach (var kv2 in kv.Value)
                {
                    kv2.Value.Flash();
                }
            }
        }

        /// <summary>
        /// 获取 Agent, 如果不存在则创建
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <typeparam name="T">Agent 类型</typeparam>
        /// <returns>Agent</returns>
        public T EnsureAgent<T>(ulong actor) where T : Agent, new()
        {
            var agent = GetAgent<T>(actor);
            if (null == agent) agent = AddAgent<T>(actor);

            return agent;
        }

        /// <summary>
        /// 获取 Agent, 如果不存在则返回默认值
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <typeparam name="T">Agent 类型</typeparam>
        /// <returns>Agent</returns>
        public T GetAgent<T>(ulong actor) where T : Agent
        {
            if (false == agentdict.TryGetValue(actor, out var agents)) return default;
            if (false == agents.TryGetValue(typeof(T), out var agent)) return default;

            return agent as T;
        }
        
        /// <summary>
        /// 获取 Agent 集合
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <returns>Agent 集合</returns>
        public Dictionary<Type, Agent> GetAgents(ulong actor)
        {
            if (false == agentdict.TryGetValue(actor, out var agents)) return default;

            return agents;
        }

        /// <summary>
        /// 移除 Agents
        /// </summary>
        /// <param name="actor">ActorID</param>
        public void RmvAgent(ulong actor)
        {
            var dict = GetAgents(actor);
            if (null == dict) return;
            var agents = ObjectPool.Ensure<List<Agent>>();
            foreach (var kv in dict) agents.Add(kv.Value);
            foreach (var agent in agents) RmvAgent(agent);
            agents.Clear();
            ObjectPool.Set(agents);
        }

        /// <summary>
        /// 移除 Agent
        /// </summary>
        /// <param name="agent">Agent</param>
        public void RmvAgent(Agent agent)
        {
            var actor = agent.actor;
            if (false == agentdict.TryGetValue(actor, out var dict)) return;

            dict.Remove(agent.GetType());
            agent.Reset();
            ObjectPool.Set(agent);
            if (0 == dict.Count)
            {
                ObjectPool.Set(dict);
                agentdict.Remove(actor);
            }
            
            snapshotagents.Remove(agent);
        }

        /// <summary>
        /// 添加 Agent
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <typeparam name="T">Agent 类型</typeparam>
        /// <returns>Agent</returns>
        /// <exception cref="Exception">Agent 已存在</exception>
        private T AddAgent<T>(ulong actor) where T : Agent, new()
        {
            if (false == agentdict.TryGetValue(actor, out var dict))
            {
                agentdict.Add(actor, dict = ObjectPool.Ensure<Dictionary<Type, Agent>>());
            }

            if (dict.TryGetValue(typeof(T), out var agent)) throw new Exception($"agent {typeof(T)} already exists");
            
            agent = ObjectPool.Ensure<T>();
            agent.Ready(actor, this);
            dict.Add(typeof(T), agent);
            snapshotagents.Add(agent);

            return agent as T;
        }

        private void OnTick(TickEvent e)
        {
            // 执行过程中, 可能会触发修改 agentdict, 导致错误
            var agents = ObjectPool.Ensure<List<(Agent agents, float timescale)>>();
            foreach (var kv in agentdict)
            {
                float timescale = 1f;
                if (rilbucket.SeekRIL<RIL_TICKER>(kv.Key, out var ril)) timescale = ril.timescale * Config.Int2Float;
                foreach (var agent in kv.Value.Values)
                {
                    if (ChaseStatus.Arrived == agent.status) continue;
                    agents.Add((agent, timescale));
                }
            }

            // 收集后再处理
            foreach ((Agent agent, float timescale) in agents) agent.Chase(e.tick, timescale);
            agents.Clear();
            ObjectPool.Set(agents);
        }
    }
}