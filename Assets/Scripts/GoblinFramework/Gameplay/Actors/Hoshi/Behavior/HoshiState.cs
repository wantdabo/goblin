﻿using GoblinFramework.Gameplay.Behavior.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiState : FSMLockstepState<HoshiBehavior.HoshiInfo, HoshiBehavior, HoshiState>
    {
        public override List<Type> PassStates => new List<Type>
        {
            typeof(HoshiIdle), typeof(HoshiRun),
            typeof(HoshiAttackA), typeof(HoshiAttackB),
            typeof(HoshiAttackC), typeof(HoshiAttackD)
        };

        protected override void OnEnter()
        {
        }

        protected override void OnLeave()
        {
        }
    }
}