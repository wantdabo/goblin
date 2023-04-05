using System;
using System.Collections.Generic;
using GoblinFramework.Client.Common;

namespace GoblinFramework.Client.Gameplay.State
{
    public abstract class FSMState : CComp
    {
        public abstract List<Type> passStates { get; }
        public FSMachine machine;
        public abstract void OnEnter();
        public abstract void OnLeave();
        public abstract bool OnDetect();
        public abstract void OnProcess(float tick);
    }
}