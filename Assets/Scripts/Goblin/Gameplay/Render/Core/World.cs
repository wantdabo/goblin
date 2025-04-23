using System;
using System.Collections.Generic;
using System.Linq;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Directors.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Batches;
using Goblin.Gameplay.Render.Cameras;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Core
{
    /// <summary>
    /// RIL 指令事件
    /// </summary>
    public struct RILEvent : IEvent
    {
        /// <summary>
        /// 渲染状态
        /// </summary>
        public RILState rilstate { get; set; }
    }

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
                if (false == statebucket.GetState<SeatState>(sa, out var state)) return 0;
                if (false == state.seatdict.TryGetValue(selfseat, out var actor)) return 0;
                
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
        /// RIL 状态桶
        /// </summary>
        public StateBucket statebucket { get; private set; }
        /// <summary>
        /// 眼睛/摄像机
        /// </summary>
        public Eyes eyes { get; private set; }
        /// <summary>
        /// Agent 集合
        /// </summary>
        private Dictionary<ulong, Dictionary<Type, Agent>> agentdict { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
            
            ticker = AddComp<Ticker>();
            ticker.Create();
            
            input = AddComp<InputSystem>();
            input.Initialize(this).Create();
            
            statebucket = AddComp<StateBucket>();
            statebucket.Initialize(this).Create();
            
            eyes = AddComp<Eyes>();
            eyes.Initialize(this).Create();

            Batches();
            
            agentdict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, Agent>>>();
            
            ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // 回收所有 Agents
            foreach (var kv in agentdict)
            {
                foreach (var agent in kv.Value)
                {
                    agent.Value.Reset();
                    ObjectCache.Set(agent.Value);
                }
                
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            agentdict.Clear();
            ObjectCache.Set(agentdict);
            
            ticker.eventor.UnListen<TickEvent>(OnTick);
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
            AddComp<StageBatch>().Initialize(this).Create();
            AddComp<TagBatch>().Initialize(this).Create();
            AddComp<SpatialBatch>().Initialize(this).Create();
            AddComp<StateMachineBatch>().Initialize(this).Create();
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
        /// 移除 Agent
        /// </summary>
        /// <param name="agent">Agent</param>
        private void RmvAgent(Agent agent)
        {
            if (false == agentdict.TryGetValue(agent.actor, out var agents)) return;

            agents.Remove(agent.GetType());
            agent.Reset();
            ObjectCache.Set(agent);
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
            if (false == agentdict.TryGetValue(actor, out var agents))
            {
                agentdict.Add(actor, agents = ObjectCache.Get<Dictionary<Type, Agent>>());
            }

            if (agents.TryGetValue(typeof(T), out var agent)) throw new Exception($"agent {typeof(T)} already exists");
            
            agent = ObjectCache.Get<T>();
            agent.Ready(actor, this);
            agents.Add(typeof(T), agent);

            return agent as T;
        }

        private void OnTick(TickEvent e)
        {
            foreach (var kv in agentdict)
            {
                float timescale = 1f;
                if (statebucket.GetState<TickerState>(kv.Key, out var state)) timescale = state.timescale;

                foreach (var agent in kv.Value.Values)
                {
                    agent.Chase(e.tick, timescale);
                }
            }
        }
    }
}