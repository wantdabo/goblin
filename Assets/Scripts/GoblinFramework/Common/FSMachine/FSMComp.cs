using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common.FSMachine
{
    /// <summary>
    /// Finite-State-Machine-Comp，状态机，状态机组件
    /// </summary>
    /// <typeparam name="E">引擎组件类型</typeparam>
    public abstract class FSMComp<E> : Comp<E> where E : GameEngineComp<E>, new()
    {
        private FSMState<E> targetState;
        public FSMState<E> State { get { return targetState; } private set { targetState = value; } }

        private Dictionary<Type, FSMState<E>> stateDict = new Dictionary<Type, FSMState<E>>();
        public void AddState<T>() where T : FSMState<E>, new()
        {
            var type = typeof(T);

            if (stateDict.ContainsKey(typeof(T))) new Exception("cant't add same state to this machine ->" + nameof(T));

            T state = new T();
            state.Create();
            stateDict.Add(type, state);
        }

        public void EnterState<T>() where T : FSMState<E>
        {
            stateDict.TryGetValue(typeof(T), out targetState);
            if (null == targetState) new Exception($"can't turn to state -> {nameof(T)}, because this machine not found {nameof(T)} state.");
            State.Leave();
            State.Enter();
        }
    }
}
