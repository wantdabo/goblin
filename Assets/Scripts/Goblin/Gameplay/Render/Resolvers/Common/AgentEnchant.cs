using Goblin.Core;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;

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
        protected RILBucket rilbucket { get; private set; }
        
        /// <summary>
        /// 初始化代理赋能
        /// </summary>
        /// <param name="rilbucket">数据状态桶</param>
        /// <returns>代理赋能</returns>
        public AgentEnchant Initialize(RILBucket rilbucket)
        {
            this.rilbucket = rilbucket;

            return this;
        }

        /// <summary>
        /// 处理数据状态
        /// </summary>
        /// <param name="ril">数据状态</param>
        public virtual void DoRIL(IRIL ril)
        {
            
        }
        
        /// <summary>
        /// 处理数据状态
        /// </summary>
        /// <param name="ril"></param>
        public void LossRIL(RIL_LOSS ril)
        {
            OnLossRIL(ril);
        }
        
        /// <summary>
        /// 处理数据状态移除
        /// </summary>
        /// <param name="ril">LOSS 丢弃渲染指令</param>
        protected virtual void OnLossRIL(RIL_LOSS ril)
        {
        }
    }

    /// <summary>
    /// 代理赋能, 根据状态信息, 添加 Agent/移除 Agent, 进行业务
    /// </summary>
    /// <typeparam name="T">数据状态类型</typeparam>
    public abstract class AgentEnchant<T> : AgentEnchant where T : IRIL
    {
        /// <summary>
        /// 处理数据状态
        /// </summary>
        /// <param name="ril">数据状态</param>
        public override void DoRIL(IRIL ril)
        {
            base.DoRIL(ril);
            OnRIL(ril as T);
        }

        /// <summary>
        /// 处理数据状态
        /// </summary>
        /// <param name="ril">数据状态</param>
        protected abstract void OnRIL(T ril);
    }
}