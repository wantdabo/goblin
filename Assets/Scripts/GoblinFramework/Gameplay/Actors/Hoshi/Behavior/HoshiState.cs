using GoblinFramework.Gameplay.Behaviors.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiState : FSMState<HoshiBehavior.HoshiInfo, HoshiBehavior, HoshiState>
    {
        public override List<Type> PassStates => new List<Type>
        {
            typeof(HoshiIdle), typeof(HoshiRun),
            typeof(HoshiAttackA),
        };

        public override bool OnDetect()
        {
            return false;
        }

        protected override void OnEnter()
        {
        }

        protected override void OnLeave()
        {
        }
    }
}
