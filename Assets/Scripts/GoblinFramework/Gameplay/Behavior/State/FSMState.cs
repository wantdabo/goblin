using System;
using System.Collections.Generic;
using GoblinFramework.Core;
using GoblinFramework.Gameplay.Common;

namespace GoblinFramework.Gameplay.State
{
    public abstract class FSMState : Comp
    {
        public abstract List<Type> passStates { get; }
        public FSMachine machine;
        public abstract void OnEnter();
        public abstract void OnLeave();
        public abstract bool OnDetect();
        public abstract void OnProcess(float tick);
    }
}