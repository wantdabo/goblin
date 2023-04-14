using System;
using System.Collections.Generic;
using System.Linq;
using GoblinFramework.Common;
using GoblinFramework.Core;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Events;

namespace GoblinFramework.Gameplay.States
{
    public class FSMachine : Comp
    {
        public StateMachine stateMachine;
        public int layer;
        private FSMState current;
        private List<FSMState> states = new();

        public void SetState<T>() where T : FSMState, new()
        {
            foreach (var s in states)
            {
                if (typeof(T) == s.GetType()) throw new Exception($"can't set same state -> {typeof(T)}");
            }

            var state = AddComp<T>();
            state.machine = this;
            states.Add(state);
            state.Create();
        }

        private void StateDetect()
        {
            foreach (var state in states)
            {
                if (null != current && current.passStates.Contains(state.GetType())) continue;
                if (false == state.OnDetect()) continue;

                current?.OnLeave();
                current = state;
                current.OnEnter();

                stateMachine.actor.eventor.Tell(new StateChangeEvent() { layer = layer, type = current.GetType() });
            }
        }

        public void OnTick(uint frame, float tick)
        {
            StateDetect();
            current?.OnProcess(tick);
        }
    }
}