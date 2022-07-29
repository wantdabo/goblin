using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Gameplay.Common.FSMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiBehavior : FSMachineLockstep<HoshiBehavior.HoshiInfo, HoshiBehavior, HoshiState>
    {
        public HoshiBehavior Behavior = null;

        protected override void OnCreate()
        {
            base.OnCreate();
            
            Behavior = this;
            
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
            public int walkSpeed;
            public int runSpeed;
        }
        #endregion
    }
}
