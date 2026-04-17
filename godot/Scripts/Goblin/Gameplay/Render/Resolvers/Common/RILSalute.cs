using Goblin.Core;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// RIL 事件处理器
    /// </summary>
    public abstract class RILSalute : Comp
    {
        /// <summary>
        /// 数据状态桶
        /// </summary>
        protected RILBucket rilbucket { get; private set; }

        /// <summary>
        /// 初始化 RIL 事件处理器
        /// </summary>
        /// <param name="rilbucket">数据状态桶</param>
        /// <returns>RIL 事件处理器</returns>
        public RILSalute Initialize(RILBucket rilbucket)
        {
            this.rilbucket = rilbucket;

            return this;
        }

        /// <summary>
        /// 处理 RIL 事件
        /// </summary>
        /// <param name="e">RIL 事件</param>
        public virtual void Salute(IRIL_EVENT e)
        {
        }
    }
    
    /// <summary>
    /// RIL 事件处理器
    /// </summary>
    /// <typeparam name="T">RIL_EVENT 类型</typeparam>
    public abstract class RILSalute<T> : RILSalute where T : IRIL_EVENT
    {
        /// <summary>
        /// 处理 RIL 事件
        /// </summary>
        /// <param name="e">RIL 事件</param>
        public override void Salute(IRIL_EVENT e)
        {
            base.Salute(e);
            OnSalute(e as T);
        }

        /// <summary>
        /// 处理 RIL 事件
        /// </summary>
        /// <param name="e">RIL 事件</param>
        protected abstract void OnSalute(T e);
    }
}