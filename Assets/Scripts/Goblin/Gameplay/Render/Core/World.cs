using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Directors.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Cameras;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Resolvers;

namespace Goblin.Gameplay.Render.Core
{
    public struct RILEvent : IEvent
    {
        public ABStateInfo state { get; set; }
    }

    public sealed class World : Comp
    {
        /// <summary>
        /// 座位 ID
        /// </summary>
        public ulong seat { get; private set; } = 0;
        /// <summary>
        /// 自我
        /// </summary>
        public ulong self {
            get
            {
                var bundles = summary.GetStateBundles(RIL_DEFINE.SEAT);
                if (null == bundles) return 0;
                
                foreach (var bundle in bundles)
                {
                    var ril = (RIL_SEAT)bundle.ril;
                    if (ril.seat == seat) return bundle.actor;
                }
                
                return 0;
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
        public Summary summary { get; private set; }
        public Eyes eyes { get; private set; }
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
            
            summary = AddComp<Summary>();
            summary.Initialize(this).Create();
            
            eyes = AddComp<Eyes>();
            eyes.Initialize(this).Create();

            Resolvers();

            agentdict = ObjectCache.Get<Dictionary<ulong, Dictionary<Type, Agent>>>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
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
        }
        
        public World Initialize(ulong seat)
        {
            this.seat = seat;
            
            return this;
        }

        private void Resolvers()
        {
            AddComp<Synopsis>().Initialize(this).Create();
            AddComp<Tag>().Initialize(this).Create();
            AddComp<Spatial>().Initialize(this).Create();
            AddComp<StateMachine>().Initialize(this).Create();
        }

        public void RmvAgents(ulong actor)
        {
            if (false == agentdict.TryGetValue(actor, out var agents)) return;
            foreach (var kv in agents)
            {
                kv.Value.Reset();
                ObjectCache.Set(kv.Value);                
            }

            agentdict.Remove(actor);
            agents.Clear();
            ObjectCache.Set(agents);
        }

        public T EnsureAgent<T>(ulong actor) where T : Agent, new()
        {
            var agent = GetAgent<T>(actor);
            if (null == agent) agent = AddAgent<T>(actor);

            return agent;
        }

        public T GetAgent<T>(ulong actor) where T : Agent
        {
            if (false == agentdict.TryGetValue(actor, out var agents)) return default;
            if (false == agents.TryGetValue(typeof(T), out var agent)) return default;

            return agent as T;
        }

        public void RmvAgent<T>(ulong actor) where T : Agent
        {
            RmvAgent(GetAgent<T>(actor));
        }

        public void RmvAgent(Agent agent)
        {
            if (false == agentdict.TryGetValue(agent.actor, out var agents)) return;

            agents.Remove(agent.GetType());
            agent.Reset();
            ObjectCache.Set(agent);
        }

        public T AddAgent<T>(ulong actor) where T : Agent, new()
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
    }
}