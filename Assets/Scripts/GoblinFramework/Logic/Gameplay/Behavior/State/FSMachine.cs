using System;
using System.Collections.Generic;
using System.Linq;
using GoblinFramework.Logic.Common;

namespace GoblinFramework.Logic.Gameplay.State
{
    public class FSMachine : LComp, ILoop
    {
        public StateMachine stateMachine;
        public event Action<Type> stateNotify;

        private FSMState current;
        private List<FSMState> states = new List<FSMState>();

        public void SetState<T>() where T : FSMState, new()
        {
            foreach (var s in states) if (typeof(T) == s.GetType()) throw new Exception($"can't set same state -> {typeof(T)}");

            var state = AddComp<T>();
            state.machine = this;
            states.Add(state);
            state.Create();
        }

        private void StateDetect()
        {
            foreach (var state in states)
            {
                if(null != current && current.passStates.Contains(state.GetType())) continue;
                if (false == state.OnDetect()) continue;
                
                current?.OnLeave();
                current = state;
                current.OnEnter();
                
                stateNotify?.Invoke(current.GetType());
            }
        }

        public void PLoop(int frame, float detailTime)
        {
            StateDetect();
            current?.OnProcess(frame, detailTime);
        }
    }
}