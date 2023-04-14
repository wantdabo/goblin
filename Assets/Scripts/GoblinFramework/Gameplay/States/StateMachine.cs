using System;
using System.Collections.Generic;
using GoblinFramework.Gameplay.Events;

namespace GoblinFramework.Gameplay.States
{
    public class StateMachine : Behavior
    {
        private List<FSMachine> machines = new();
        private Dictionary<int, Type> stateDict = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            actor.stage.ticker.eventor.Listen<TickEvent>(OnTick);
            actor.eventor.Listen<StateChangeEvent>(OnStateChange);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.stage.ticker.eventor.UnListen<TickEvent>(OnTick);
            actor.eventor.UnListen<StateChangeEvent>(OnStateChange);
        }

        private FSMachine GetMachine(int layer = 0)
        {
            if (machines.Count >= layer + 1) return machines[layer];

            return null;
        }

        public void SetState<T>(int layer = 0) where T : FSMState, new()
        {
            var machine = GetMachine(layer);
            if (null == machine)
            {
                machine = AddComp<FSMachine>();
                machine.stateMachine = this;
                machine.layer = layer;
                machines.Add(machine);
                machine.Create();
            }

            machine.SetState<T>();
        }

        private void OnStateChange(StateChangeEvent e)
        {
            if (stateDict.ContainsKey(e.layer)) stateDict.Remove(e.layer);
            stateDict.Add(e.layer, e.type);
        }

        private void OnTick(TickEvent e)
        {
            foreach (var machine in machines) machine.OnTick(e.frame, e.tick);    
        }
    }
}
