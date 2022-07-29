using GoblinFramework.Gameplay.Common.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Controllers
{
    public class HoshiController : FSMachineLockstep<HoshiController, HoshiState>
    {
        public HoshiBehavior Behavior = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            Behavior = Actor.GetBehavior<HoshiBehavior>();

            SetState<HoshiIdleState>();
            SetState<HoshiWalkState>();
            SetState<HoshiRunState>();
            SetState<HoshiAttackAState>();
            SetState<HoshiAttackBState>();
            SetState<HoshiAttackCState>();
            SetState<HoshiAttackDState>();

            EnterState<HoshiIdleState>();
        }
    }
}
