using System;
using System.Collections.Generic;

namespace GoblinFramework.Gameplay.States
{
    public class JianhunIdle : FSMState
    {
        private List<Type> mPassStates = new() { typeof(JianhunRun) };
        public override List<Type> passStates => mPassStates;

        public override bool OnDetect()
        {
            return true;
        }

        public override void OnEnter()
        {
        }

        public override void OnLeave()
        {
        }

        public override void OnProcess(float tick)
        {
        }
    }
}