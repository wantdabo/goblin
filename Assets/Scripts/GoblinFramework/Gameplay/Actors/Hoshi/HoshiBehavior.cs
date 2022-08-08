using GoblinFramework.Gameplay.Behaviors.FSMachine;
using GoblinFramework.Gameplay.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoblinFramework.General.Gameplay.RIL.RILS;
using Numerics.Fixed;
using GoblinFramework.Gameplay.Actors.Hoshi.Behavior;

namespace GoblinFramework.Gameplay.Actors.Hoshi
{
    public class HoshiBehavior : FSMachine<HoshiBehavior, HoshiBehavior.HoshiInfo, HoshiState>
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
            SetState<HoshiAttackB>();
            SetState<HoshiAttackCHold>();
            SetState<HoshiAttackC>();

            SetEntrance<HoshiIdle>();
        }

        #region HoshiInfo
        public class HoshiInfo : BehaviorInfo
        {
            public Fixed64 runSpeed = 4 * Fixed64.EN1;

            public int attackAKeepFrame = 5;
            public int attackBKeepFrame = 3;

            public int attackCKeepFrame = 10;
            public Fixed64 attackCMotionForce = 10 * Fixed64.EN1;
        }
        #endregion
    }
}
