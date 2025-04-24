using Goblin.Core;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// 代理赋能, 根据状态信息, 添加 Agent/移除 Agent
    /// </summary>
    public abstract class AgentEnchant : Comp
    {
        /// <summary>
        /// 数据状态桶
        /// </summary>
        protected StateBucket statebucket { get; private set; }
        
        /// <summary>
        /// 初始化代理赋能
        /// </summary>
        /// <param name="statebucket">数据状态桶</param>
        /// <returns>代理赋能</returns>
        public AgentEnchant Initialize(StateBucket statebucket)
        {
            this.statebucket = statebucket;

            return this;
        }
    }

    /// <summary>
    /// 代理赋能, 根据状态信息, 添加 Agent/移除 Agent, 进行业务
    /// </summary>
    /// <typeparam name="T">数据状态类型</typeparam>
    public abstract class AgentEnchant<T> : AgentEnchant where T : State
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            statebucket.eventor.Listen<RStateEvent<T>>(OnRState);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            statebucket.eventor.UnListen<RStateEvent<T>>(OnRState);
        }

        private void OnRState(RStateEvent<T> e)
        {
            OnRState(e.state);
        }
        
        /// <summary>
        /// 处理数据状态
        /// </summary>
        /// <param name="state">数据状态</param>
        protected abstract void OnRState(T state);
    }
}