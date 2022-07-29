using GoblinFramework.Gameplay.Common.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Controllers
{
    public class HoshiState : FSMLockstepState<HoshiController, HoshiState>
    {
        public override List<Type> PassStates => new List<Type>
        {
            typeof(HoshiWalkState), typeof(HoshiRunState),
            typeof(HoshiAttackAState), typeof(HoshiAttackBState),
            typeof(HoshiAttackCState), typeof(HoshiAttackDState)
        };

        protected override void OnEnter()
        {
        }

        protected override void OnLeave()
        {
        }
    }
}
