using System;
using System.Collections.Generic;
using GoblinFramework.Gameplay.Common;

namespace GoblinFramework.Gameplay.State
{
    public abstract class FSMState : PComp
    {
        public abstract List<Type> passStates { get; }
        public FSMachine machine;
        public abstract void OnEnter();
        public abstract void OnLeave();
        public abstract bool OnDetect();
        public abstract void OnProcess(float tick);
    }
}