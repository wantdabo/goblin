using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.RIL.Common;
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
        /// 事件订阅派发者
        /// </summary>
        public Eventor eventor { get; private set; }
        /// <summary>
        /// Ticker/时间驱动器
        /// </summary>
        public Ticker ticker { get; private set; }
        public Summary summary { get; private set; }
        private Dictionary<ulong, Dictionary<Type, Agent>> agentdict { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            eventor = AddComp<Eventor>();
            eventor.Create();
            
            ticker = AddComp<Ticker>();
            ticker.Create();
            
            summary = AddComp<Summary>();
            summary.Initialize(this);
            summary.Create();

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

        private void Resolvers()
        {
            AddComp<Synopsis>().Initialize(this).Create();
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