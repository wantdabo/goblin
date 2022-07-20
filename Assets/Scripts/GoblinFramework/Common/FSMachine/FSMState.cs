using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common.FSMachine
{

    /// <summary>
    /// Finite-State-Machine-State，状态机，状态
    /// </summary>
    /// <typeparam name="E">引擎组件类型</typeparam>
    /// <typeparam name="MT">状态机类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMState<E, MT, ST> : Goblin where E : GameEngineComp<E>, new() where MT : FSMComp<E, MT, ST>, new() where ST : FSMState<E, MT, ST>, new()
    {
        public MT Machine;

        public void Enter()
        {
            OnEnter();
        }

        public void Leave()
        {
            OnLeave();
        }

        protected abstract void OnEnter();
        protected abstract void OnLeave();
    }
}
