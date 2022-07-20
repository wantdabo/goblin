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
    /// <typeparam name="E">引擎组件</typeparam>
    public abstract class FSMState<E> : Goblin where E : GameEngineComp<E>, new()
    {
        public FSMComp<E> Machine;

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
