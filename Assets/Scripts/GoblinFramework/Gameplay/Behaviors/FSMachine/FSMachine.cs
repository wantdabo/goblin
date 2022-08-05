using GoblinFramework.Client;
using GoblinFramework.Client.Common;
using GoblinFramework.Core;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Gameplay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors.FSMachine
{
    /// <summary>
    /// Finite-State-Machine-Comp，状态机，状态机组件
    /// </summary>
    /// <typeparam name="I">BeaviorInfo 类型</typeparam>
    /// <typeparam name="B">行为状态机组件类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMachine<I, B, ST> : Behavior<I>, IPLoop where I : BehaviorInfo, new() where B : FSMachine<I, B, ST>, new() where ST : FSMState<I, B, ST>, new()
    {
        protected ST state;
        public ST State { get { return state; } set { state = value; } }

        protected Dictionary<Type, ST> stateDict = new Dictionary<Type, ST>();
        protected List<ST> stateList = new List<ST>();
        public ST GetState<T>() where T : ST
        {
            return GetState(typeof(T));
        }

        public ST GetState(Type type) 
        {
            stateDict.TryGetValue(type, out ST targetState);

            return targetState;
        }

        protected void SetState<T>() where T : ST, new()
        {
            var state = AddComp<T>();
            state.Behavior = this as B;
            state.Actor = Actor;
            stateDict.Add(typeof(T), state);
            stateList.Add(state);
        }

        /// <summary>
        /// 切换至指定状态，且指定状态在通行列表中
        /// </summary>
        /// <typeparam name="T">FSMState 状态</typeparam>
        public void EnterState<T>() where T : ST
        {
            EnterState(GetState<T>());
        }

        public void EnterState(ST targetState)
        {
            if (targetState == State) return;

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
