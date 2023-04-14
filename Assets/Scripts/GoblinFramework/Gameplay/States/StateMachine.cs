using System;
using System.Collections.Generic;

namespace GoblinFramework.Gameplay.States
{
    public class StateMachine : Behavior
    {
        private List<FSMachine> machines = new List<FSMachine>();
        private Dictionary<int, Type> stateDict = new Dictionary<int, Type>();
        
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
                machine.stateNotify += (type) => NotifyStateInfo(layer, type);
                machines.Add(machine);
                machine.Create();
            }

            machine.SetState<T>();
        }

        private void NotifyStateInfo(int layer, Type type)
        {
            if (stateDict.ContainsKey(layer)) stateDict.Remove(layer);
            stateDict.Add(layer, type);
        }
    }
}