using Goblin.Core;
using System;
using System.Collections.Generic;

namespace Goblin.Common.FSM
{
    /// <summary>
    /// 有限状态机 - 状态
    /// </summary>
    public abstract class State : Comp
    {
        /// <summary>
        /// 设置可通行的状态，如果需要读取，请使用 aisles 字段，否则会造成 GC 性能问题。
        /// </summary>
        protected abstract List<Type> passes { get; }

        private List<Type> mpasses = null;
        /// <summary>
        /// 读到的结果 passes 字段一致，如果是为了读取，请使用此字段。
        /// </summary>
        public List<Type> aisles { get { return mpasses; } private set { mpasses = value; } }

        /// <summary>
        /// 有限状态机
        /// </summary>
        public Machine machine { get; set; }

        /// <summary>
        /// 状态进行中
        /// </summary>
        public bool playing { get { return machine.current == this; } }

        protected override void OnCreate()
        {
            base.OnCreate();
            aisles = passes;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        /// <summary>
        /// 打断状态，强制跳出
        /// </summary>
        protected void Break()
        {
            machine.ChangeState(null);
        }

        /// <summary>
        /// 状态进入条件检查
        /// </summary>
        /// <returns>是否触发</returns>
        public abstract bool OnValid();

        /// <summary>
        /// 状态进入
        /// </summary>
        public virtual void OnEnter() { }

        /// <summary>
        /// 状态离开
        /// </summary>
        public virtual void OnExit() { }

        /// <summary>
        /// 状态驱动
        /// </summary>
        /// <param name="frame">帧</param>
        /// <param name="tick">时间流逝</param>
        public virtual void OnTick(uint frame, float tick) { }
    }
}