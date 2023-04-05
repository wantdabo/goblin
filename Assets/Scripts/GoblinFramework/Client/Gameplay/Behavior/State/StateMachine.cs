using System;
using System.Collections.Generic;

namespace GoblinFramework.Client.Gameplay.State
{
    public class StateMachineInfo : BehaviorInfo
    {
        public List<FSMachine> machines = new List<FSMachine>();
        public Dictionary<int, Type> stateDict = new Dictionary<int, Type>();
    }

    public class StateMachine : Behavior<StateMachineInfo>
    {
        private FSMachine GetMachine(int layer = 0)
        {
            if (info.machines.Count >= layer + 1) return info.machines[layer];

            return null;
        }

        public void SetState<T>(int layer = 0) where T : FSMState, new()
        {
            var machine = GetMachine(layer);
            if (null == machine)
            {
                machine = AddComp<FSMachine>();
                machine.stateNotify += (type) => NotifyStateInfo(layer, type);
                info.machines.Add(machine);
                machine.Create();
            }

            machine.SetState<T>();
        }

        private void NotifyStateInfo(int layer, Type type)
        {
            if (info.stateDict.ContainsKey(layer)) info.stateDict.Remove(layer);
            info.stateDict.Add(layer, type);
        }
    }
}