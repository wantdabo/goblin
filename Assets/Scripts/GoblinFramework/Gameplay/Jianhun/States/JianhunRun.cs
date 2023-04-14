using System;
using System.Collections.Generic;

namespace GoblinFramework.Gameplay.States
{
    public class JianhunRun : FSMState
    {
        public override List<Type> passStates => new() { typeof(JianhunIdle) };
        
        public override bool OnDetect()
        {
            return false;
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