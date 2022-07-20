using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common.FSMachine
{
    /// <summary>
    /// Finite-State-Machine-State，状态机，锁步状态
    /// </summary>
    /// <typeparam name="E">引擎组件类型</typeparam>
    /// <typeparam name="MT">状态机类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMLockstepState<E, MT, ST> : FSMState<E, MT, ST> where E : GameEngineComp<E>, new() where MT : FSMLockstepComp<E, MT, ST>, new() where ST : FSMLockstepState<E, MT, ST>, new()
    {
        /// <summary>
        /// 定义可通行的状态类型列表，如果下一个装填类型不在此列表中，将不允通过
        /// </summary>
        public abstract List<Type> PassStates { get; }
    }
}
