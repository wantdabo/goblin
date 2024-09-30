using Goblin.Core;
using System;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.Common.StateMachine
{
    /// <summary>
    /// 有限状态机 - 状态
    /// </summary>
    public abstract partial class State : Comp
    {
        /// <summary>
        /// 状态 ID
        /// </summary>
        public abstract uint id { get; }
        /// <summary>
        /// 设置可通行的状态，如果需要读取，请使用 aisles 字段，否则会造成 GC 性能问题。
        /// </summary>
        protected abstract List<uint> passes { get; }
        private List<uint> mPasses = null;
        /// <summary>
        /// 读到的结果 passes 字段一致，如果是为了读取，请使用此字段。
        /// </summary>
        public List<uint> aisles { get { return mPasses; } private set { mPasses = value; } }
        /// <summary>
        /// 状态进入持续了多少帧
        /// </summary>
        public uint frames { get; private set; }

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
        /// 有限状态机
        /// </summary>
        public Machine machine { get; set; }

        /// <summary>
        /// 状态进行中
        /// </summary>
        public bool playing { get { return machine.current == this; } }

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
        public abstract bool OnCheck();

        /// <summary>
        /// 状态进入
        /// </summary>
        public virtual void OnEnter()
        {
            frames = 0;
            machine.paramachine.actor.eventor.Tell(new StateEnterEvent { state = this, layer = machine.layer }); 
        }

        /// <summary>
        /// 状态离开
        /// </summary>
        public virtual void OnExit()
        {
            frames = 0;
            machine.paramachine.actor.eventor.Tell(new StateExitEvent { state = this, layer = machine.layer });
        }

        /// <summary>
        /// 状态驱动
        /// </summary>
        /// <param name="frame">帧</param>
        /// <param name="fixedTick">s/秒</param>
        public virtual void OnFPTick(uint frame, FP fixedTick)
        {
            frames++;
        }
    }
}
