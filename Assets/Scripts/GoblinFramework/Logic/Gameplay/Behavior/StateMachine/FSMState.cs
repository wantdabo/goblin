using System;
using System.Collections.Generic;
using GoblinFramework.Logic.Common;

namespace GoblinFramework.Logic.Gameplay
{
    public abstract class FSMState : LComp
    {
        public abstract List<Type> passStates { get; }
        public FSMachine machine;
        public abstract void OnEnter();
        public abstract void OnLeave();
        public abstract bool OnDetect();
        public abstract void OnProcess(int frame, float detailTime);
    }
}