using GoblinFramework.Core;
using GoblinFramework.Gameplay.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors.FSMachine
{
    /// <summary>
    /// Finite-State-Machine-Lock，状态机，锁步状态机组件
    /// </summary>
    /// <typeparam name="I">BeaviorInfo 类型</typeparam>
    /// <typeparam name="MT">状态机类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMachineLockstep<I, MT, ST> : FSMachine<I, MT, ST> where I : BehaviorInfo, new() where MT : FSMachineLockstep<I, MT, ST>, new() where ST : FSMLockstepState<I, MT, ST>, new()
    {
        /// <summary>
        /// 切换至指定状态，且指定状态在通行列表中
        /// </summary>
        /// <typeparam name="T">FSMLockstepState 状态类型</typeparam>
        public new void EnterState<T>() where T : ST
        {
            var targetState = GetState<T>();

            EnterState(targetState);
        }

        public new void EnterState(ST targetState)
        {
            if (null == State)
            {
                base.EnterState(targetState);

                return;
            }

            if (null == State.PassStates) return;

            if (false == State.PassStates.Contains(targetState.GetType())) return;

            base.EnterState(targetState);
        }

        public override void PLoop(int frame)
        {
            foreach (var state in stateList) state.Detect();
            base.PLoop(frame);
        }
    }
}
