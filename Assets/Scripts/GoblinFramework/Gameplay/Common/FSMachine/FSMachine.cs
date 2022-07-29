using GoblinFramework.Client;
using GoblinFramework.Client.Common;
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
    /// Finite-State-Machine-Comp，状态机，状态机组件
    /// </summary>
    /// <typeparam name="I">BeaviorInfo 类型</typeparam>
    /// <typeparam name="MT">状态机类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMachine<I, MT, ST> : Behavior<I>, IPLoop where I : BehaviorInfo, new() where MT : FSMachine<I, MT, ST>, new() where ST : FSMState<I, MT, ST>, new()
    {
        protected ST state;
        public ST State { get { return state; } private set { state = value; } }

        protected Dictionary<Type, ST> stateDict = new Dictionary<Type, ST>();
        protected ST GetState<T>() where T : ST
        {
            stateDict.TryGetValue(typeof(T), out ST targetState);

            return targetState;
        }

        public void SetState<T>() where T : ST, new()
        {
            var state = AddComp<T>();
            stateDict.Add(typeof(T), state);
        }

        /// <summary>
        /// 切换至指定状态，且指定状态在通行列表中
        /// </summary>
        /// <typeparam name="T">FSMState 状态</typeparam>
        public void EnterState<T>() where T : ST
        {
            EnterState(GetState<T>());
        }

        protected void EnterState(ST targetState)
        {
            if (null != State) State.Leave();

            State = targetState;
            targetState.Enter();
        }

        public virtual void PLoop(int frame)
        {
            if (null == State) return;
            State.OnStateTick(frame);
        }
    }
}
