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
    /// Finite-State-Machine-State，状态机，锁步状态
    /// </summary>
    /// <typeparam name="I">BeaviorInfo 类型</typeparam>
    /// <typeparam name="MT">状态机类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMLockstepState<I, MT, ST> : FSMState<I, MT, ST> where I : BehaviorInfo, new() where MT : FSMachineLockstep<I, MT, ST>, new() where ST : FSMLockstepState<I, MT, ST>, new()
    {
        /// <summary>
        /// 定义可通行的状态类型列表，如果下一个装填类型不在此列表中，将不允通过
        /// </summary>
        public abstract List<Type> PassStates { get; }

        public void Detect() 
        {
            if (OnDetect()) Behavior.EnterState(this as ST);
        }

        /// <summary>
        /// 状态检查，能否进入
        /// </summary>
        /// <returns>true 能进，false 不能进</returns>
        protected abstract bool OnDetect();
    }
}
