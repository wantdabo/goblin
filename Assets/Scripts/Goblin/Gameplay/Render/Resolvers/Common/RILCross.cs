using Goblin.Core;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// RIL 合并器
    /// </summary>
    public abstract class RILCross : Comp
    {
        /// <summary>
        /// 数据状态桶
        /// </summary>
        protected RILBucket rilbucket { get; private set; }
        
        /// <summary>
        /// 初始化 RIL 合并器
        /// </summary>
        /// <param name="rilbucket">数据状态桶</param>
        /// <returns>RIL 合并器</returns>
        public RILCross Initialize(RILBucket rilbucket)
        {
            this.rilbucket = rilbucket;

            return this;
        }

        /// <summary>
        /// 合并移除的数据状态
        /// </summary>
        /// <param name="ril">数据状态</param>
        /// <param name="diff">移除的数据状态</param>
        public virtual void HasDels(IRIL ril, IRIL_DIFF diff)
        {
            
        }

        /// <summary>
        /// 合并新增的数据状态
        /// </summary>
        /// <param name="ril">数据状态</param>
        /// <param name="diff">新增的数据状态</param>
        public virtual void HasNews(IRIL ril, IRIL_DIFF diff)
        {
            
        }
    }

    /// <summary>
    /// RIL 合并器
    /// </summary>
    /// <typeparam name="T">数据状态类型</typeparam>
    /// <typeparam name="E">差异的数据状态类型</typeparam>
    public abstract class RILCross<T, E> : RILCross where T : IRIL where E : IRIL_DIFF
    {
        public override void HasDels(IRIL ril, IRIL_DIFF diff)
        {
            base.HasDels(ril, diff);
            OnHasDels(ril as T, diff as E);
        }

        public override void HasNews(IRIL ril, IRIL_DIFF diff)
        {
            base.HasNews(ril, diff);
            OnHasNews(ril as T, diff as E);
        }

        /// <summary>
        /// 合并新增的数据状态
        /// </summary>
        /// <param name="ril">数据状态</param>
        /// <param name="diff">新增的数据状态</param>
        protected abstract void OnHasDels(T ril, E diff);
        /// <summary>
        /// 合并移除的数据状态
        /// </summary>
        /// <param name="ril">数据状态</param>
        /// <param name="diff">移除的数据状态</param>
        protected abstract void OnHasNews(T ril, E diff);
    }
}