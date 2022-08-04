using GoblinFramework.Gameplay.Behaviors.FSMachine;
using GoblinFramework.Gameplay.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoblinFramework.General.Gameplay.RIL.RILS;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiBehavior : FSMachineLockstep<HoshiBehavior.HoshiInfo, HoshiBehavior, HoshiState>
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            Actor.ActorBehavior.SendRIL<RILModel>((ril) => ril.modelName = "Hoshi/Hoshi");

            SetState<HoshiIdle>();
            SetState<HoshiRun>();
            SetState<HoshiAttackA>();
            SetState<HoshiAttackB>();
            SetState<HoshiAttackC>();
            SetState<HoshiAttackD>();

            EnterState<HoshiIdle>();
        }

        #region
        public class HoshiInfo : BehaviorInfo
        {
            public int runSpeed;
        }
        #endregion
    }
}
