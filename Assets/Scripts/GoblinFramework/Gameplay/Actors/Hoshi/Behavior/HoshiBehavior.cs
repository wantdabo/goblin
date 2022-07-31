using GoblinFramework.Gameplay.Behavior.FSMachine;
using GoblinFramework.Gameplay.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiBehavior : FSMachineLockstep<HoshiBehavior.HoshiInfo, HoshiBehavior, HoshiState>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            
            SetState<HoshiIdle>();
            SetState<HoshiRun>();
            SetState<HoshiAttackA>();
            SetState<HoshiAttackB>();
            SetState<HoshiAttackC>();
            SetState<HoshiAttackD>();

            EnterState<HoshiIdle>();
        }

        public override void PLoop(int frame)
        {
            base.PLoop(frame);
        }

        #region
        public class HoshiInfo : BehaviorInfo
        {
            public int walkSpeed;
            public int runSpeed;
        }
        #endregion
    }
}
