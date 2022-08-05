using GoblinFramework.Gameplay.Behaviors.FSMachine;
using GoblinFramework.Gameplay.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoblinFramework.General.Gameplay.RIL.RILS;
using Numerics.Fixed;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiBehavior : FSMachine<HoshiBehavior.HoshiInfo, HoshiBehavior, HoshiState>
    {
        public InputBehavior InputBehavior;
        public MotionBehavior MotionBehavior;

        protected override void OnCreate()
        {
            base.OnCreate();

            Actor.ActorBehavior.SendRIL<RILModel>((ril) => ril.modelName = "Hoshi/Hoshi");

            InputBehavior = Actor.GetBehavior<InputBehavior>();
            MotionBehavior = Actor.GetBehavior<MotionBehavior>();

            SetState<HoshiIdle>();
            SetState<HoshiRun>();
            SetState<HoshiAttackA>();

            Entrance<HoshiIdle>();
        }

        #region HoshiInfo
        public class HoshiInfo : BehaviorInfo
        {
            public Fixed64 runSpeed = 24 * Fixed64.EN2;

            public int attackAKeepFrame = 13;
            public Fixed64 attackAMotionForce = 4 * Fixed64.EN1;
        }
        #endregion
    }
}
