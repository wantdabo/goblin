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
    /// Finite-State-Machine，有限状态机
    /// </summary>
    /// <typeparam name="I">BeaviorInfo 类型</typeparam>
    /// <typeparam name="B">状态机类型</typeparam>
    /// <typeparam name="ST">状态类型</typeparam>
    public abstract class FSMachine<I, B, ST> : Behavior<I>, IPLoop where I : BehaviorInfo, new() where B : FSMachine<I, B, ST>, new() where ST : FSMState<I, B, ST>, new()
    {
        protected ST state;
        public ST State { get { return state; } private set { state = value; } }

        protected Dictionary<Type, ST> stateDict = new Dictionary<Type, ST>();
        protected List<ST> stateList = new List<ST>();

        public void Entrance<T>() where T : ST
        {
            if (null != State) throw new Exception("entrance only setting once.");

            EnterState(GetState(typeof(T)));
        }

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

        private void EnterState(ST targetState)
        {
            if (targetState == State) return;

            if (null != State)
            {
                if (null == State.PassStates) return;
                if (false == State.PassStates.Contains(targetState.GetType())) return;

                State.Leave();
            }

            targetState.Enter();
        }

        private Stack<ST> downAutoStateStack = new Stack<ST>();
        private ST PopDownAutoState()
        {
            if (1 >= downAutoStateStack.Count) return downAutoStateStack.Peek();

            return downAutoStateStack.Pop();
        }

        private void PushDownAutoState(ST state)
        {
            if (downAutoStateStack.Count > 0)
            {
                var peekState = downAutoStateStack.Peek();
                if (peekState.Equals(state)) return;
            }

            downAutoStateStack.Push(state);
        }

        public virtual void OnEnter(ST state)
        {
            State = state;
            PushDownAutoState(state);
        }

        public virtual void OnLeave(ST state)
        {
            State = null;
            PopDownAutoState();
        }

        private void StateDetect()
        {
            var detected = false;
            foreach (var state in stateList)
            {
                if (state.OnDetect())
                {
                    detected = true;
                    EnterState(state);
                }
            }

            if (detected) return;
            EnterState(PopDownAutoState());
        }

        public virtual void PLoop(int frame)
        {
            StateDetect();
            State?.OnStateTick(frame);
        }
    }
}
