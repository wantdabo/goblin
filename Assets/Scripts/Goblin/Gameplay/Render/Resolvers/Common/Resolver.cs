using Goblin.Core;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// 状态解析器
    /// </summary>
    public abstract class Resolver : Comp
    {
        /// <summary>
        /// 数据状态桶
        /// </summary>
        public StateBucket statebucket { get; private set; }

        /// <summary>
        /// 初始化状态解析器
        /// </summary>
        /// <param name="statebucket">数据状态桶</param>
        /// <returns>状态解析器</returns>
        public Resolver Initialize(StateBucket statebucket)
        {
            this.statebucket = statebucket;

            return this;
        }
    }

    /// <summary>
    /// 状态解析器
    /// </summary>
    /// <typeparam name="T">渲染指令类型</typeparam>
    /// <typeparam name="E">数据状态类型</typeparam>
    public abstract class Resolver<T, E> : Resolver where T : IRIL where E : State
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            statebucket.world.eventor.Listen<RILEvent>(OnRIL);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            statebucket.world.eventor.UnListen<RILEvent>(OnRIL);
        }
        
        private void OnRIL(RILEvent e)
        {
            // 如果不是 RIL 状态, 则返回
            if (e.rilstate is not RILState<T> rilstate) return;
            // 处理 RIL 渲染状态为数据状态
            E state = OnRIL(rilstate);
            state.actor = rilstate.actor;
            state.hashcode = rilstate.ril.GetHashCode();
            // 存入状态桶
            statebucket.SetState(rilstate.type, state);
            
            // 释放
            rilstate.Reset();
            ObjectCache.Set(rilstate);
        }

        /// <summary>
        /// 处理 RIL 状态
        /// </summary>
        /// <param name="rilstate">RIL 渲染状态</param>
        /// <returns>数据状态</returns>
        protected abstract E OnRIL(RILState<T> rilstate);
    }
}