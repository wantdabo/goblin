using System;
using System.Collections.Generic;
using System.Linq;
using GoblinFramework.Logic.Common;
using TrueSync;

namespace GoblinFramework.Logic.Gameplay
{
    public abstract class FSMState : LComp
    {
        public abstract List<Type> passStates { get; }
        public FSMachine machine;
        public abstract void OnEnter();
        public abstract void OnLeave();
        public abstract bool OnDetect();
        public abstract void OnProcess(int frame, FP detailTime);
    }

    public class FSMachine : LComp, ILoop
    {
        public StateMachine stateMachine;
        
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
            }
        }

        public void PLoop(int frame, FP detailTime)
        {
            StateDetect();
            current?.OnProcess(frame, detailTime);
        }
    }
}