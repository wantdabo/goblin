using GoblinFramework.Core;
using GoblinFramework.Gameplay.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Common.FSMachine
{

    /// <summary>
    /// Finite-State-Machine-State，状态机，状态
    /// </summary>
    /// <typeparam name="I">BeaviorInfo 类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMState<I, MT, ST> : PComp where I : BehaviorInfo, new() where MT : FSMachine<I, MT, ST>, new() where ST : FSMState<I, MT, ST>, new()
    {
        /// <summary>
        /// 状态机
        /// </summary>
        public MT Machine;

        public void Enter()
        {
            OnEnter();
        }

        public void Leave()
        {
            OnLeave();
        }

        /// <summary>
        /// 状态 Tick
        /// </summary>
        /// <param name="frame">帧数</param>
        public virtual void OnStateTick(int frame) { }

        /// <summary>
        /// 状态进入
        /// </summary>
        protected abstract void OnEnter();

        /// <summary>
        /// 状态离开
        /// </summary>
        protected abstract void OnLeave();
    }
}
