using GoblinFramework.Core;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Gameplay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behavior.FSMachine
{

    /// <summary>
    /// Finite-State-Machine-State，状态机，状态
    /// </summary>
    /// <typeparam name="I">BeaviorInfo 类型</typeparam>
    /// <typeparam name="B">行为组件</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMState<I, B, ST> : PComp where I : BehaviorInfo, new() where B : FSMachine<I, B, ST>, new() where ST : FSMState<I, B, ST>, new()
    {
        /// <summary>
        /// 状态机
        /// </summary>
        public B Behavior;

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
