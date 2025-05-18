using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Flows.Checkers.Common
{
    /// <summary>
    /// 指令条件检查器
    /// </summary>
    public abstract class Checker
    {
        /// <summary>
        /// 场景
        /// </summary>
        protected Stage stage { get; private set; }

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="stage">场景</param>
        /// <returns>指令条件检查器</returns>
        public Checker Load(Stage stage)
        {
            this.stage = stage;

            return this;
        }
        
        /// <summary>
        /// 卸载
        /// </summary>
        public void Unload()
        {
            this.stage = null;
        }

        /// <summary>
        /// 检查指令条件
        /// </summary>
        /// <param name="condition">指令条件</param>
        /// <param name="flowinfo">管线信息</param>
        /// <returns>YES/NO</returns>
        public bool Check(Condition condition, FlowInfo flowinfo)
        {
            return OnCheck(condition, flowinfo);
        }

        /// <summary>
        /// 检查指令条件
        /// </summary>
        /// <param name="condition">指令条件</param>
        /// <param name="flowinfo">管线信息</param>
        /// <returns>YES/NO</returns>
        protected abstract bool OnCheck(Condition condition, FlowInfo flowinfo);
    }

    /// <summary>
    /// 指令条件检查器
    /// </summary>
    /// <typeparam name="T">指令条件类型</typeparam>
    public abstract class Checker<T> : Checker where T : Condition
    {
        /// <summary>
        /// 检查指令条件
        /// </summary>
        /// <param name="condition">指令条件</param>
        /// <param name="flowinfo">管线信息</param>
        /// <returns>YES/NO</returns>
        protected override bool OnCheck(Condition condition, FlowInfo flowinfo)
        {
            return OnCheck(condition as T, flowinfo);
        }
        
        /// <summary>
        /// 检查指令条件
        /// </summary>
        /// <param name="condition">指令条件</param>
        /// <param name="flowinfo">管线信息</param>
        /// <returns>YES/NO</returns>
        protected abstract bool OnCheck(T condition, FlowInfo flowinfo);
    }
}