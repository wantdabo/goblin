using Goblin.Core;
using System;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.States.Common
{
    /// <summary>
    /// 有限状态机 - 状态
    /// </summary>
    public abstract class FSMState : Comp
    {
        /// <summary>
        /// 设置可通行的状态，如果需要读取，请使用 aisles 字段，否则会造成 GC 性能问题。
        /// </summary>
        protected abstract List<Type> passes { get; }

        private List<Type> mPasses = null;
        /// <summary>
        /// 读到的结果 passes 字段一致，如果是为了读取，请使用此字段。
        /// </summary>
        public List<Type> aisles { get { return mPasses; } private set { mPasses = value; } }

        protected override void OnCreate()
        {
            base.OnCreate();
            aisles = passes;
        }

        /// <summary>
        /// 有限状态机
        /// </summary>
        public FSMachine fsm;

        /// <summary>
        /// 状态进行中
        /// </summary>
        public bool isPlaying { get { return fsm.current == this; } }

        /// <summary>
        /// 打断状态，强制跳出
        /// </summary>
        protected void Break()
        {
            fsm.ChangeState(null);
        }

        /// <summary>
        /// 状态进入条件检查
        /// </summary>
        /// <returns>是否触发</returns>
        public abstract bool OnCheck();

        /// <summary>
        /// 状态进入
        /// </summary>
        public virtual void OnEnter() { fsm.sm.actor.eventor.Tell(new SMStateEnterEvent { state = this, layer = fsm.layer }); }

        /// <summary>
        /// 状态离开
        /// </summary>
        public virtual void OnExit() { fsm.sm.actor.eventor.Tell(new SMStateExitEvent { state = this, layer = fsm.layer }); }

        /// <summary>
        /// 状态驱动
        /// </summary>
        /// <param name="fixedTick">s/秒</param>
        public virtual void OnFPTick(FP fixedTick) { }
    }
}