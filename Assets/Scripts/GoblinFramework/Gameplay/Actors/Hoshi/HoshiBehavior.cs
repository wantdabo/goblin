using GoblinFramework.Gameplay.Behaviors.FSMachine;
using GoblinFramework.Gameplay.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoblinFramework.General.Gameplay.RIL.RILS;
using GoblinFramework.Gameplay.Actors.Hoshi.Behavior;
using FixMath.NET;
using GoblinFramework.Gameplay.Physics.Comps;
using BEPUutilities;

namespace GoblinFramework.Gameplay.Actors.Hoshi
{
    public class HoshiBehavior : FSMachine<HoshiBehavior, HoshiBehavior.HoshiInfo, HoshiState>
    {
        public InputBehavior InputBehavior;
        public ColliderBehavior ColliderBehavior;

        protected override void OnCreate()
        {
            base.OnCreate();

            actor.actorBehaivor.SendRIL<RILModel>((ril) => ril.modelName = "Hoshi/Hoshi");

            InputBehavior = actor.GetBehavior<InputBehavior>();
            ColliderBehavior = actor.GetBehavior<ColliderBehavior>();

            SetState<HoshiIdle>();
            SetState<HoshiRun>();
            SetState<HoshiJumping>();
            SetState<HoshiFalling>();
            SetState<HoshiAttackA>();
            SetState<HoshiAttackB>();
            SetState<HoshiAttackCHold>();
            SetState<HoshiAttackC>();

            SetEntrance<HoshiIdle>();
        }

        public override void PLoop(int frame, Fix64 detailTime)
        {
            base.PLoop(frame, detailTime);
            General.GoblinDebug.Log(State.ToString());
        }

        #region HoshiInfo
        public class HoshiInfo : BehaviorInfo
        {
            public readonly Fix64 runSpeed = 6;

            public readonly int attackAKeepFrame = 5;
            public readonly int attackBKeepFrame = 3;

            public readonly int attackCKeepFrame = 10;
            public readonly Fix64 attackCMotionForce = 10 * Fix64.EN1;

            public readonly Vector3 jumpMotionForce = new Vector3(0, 10, 0);
        }
        #endregion
    }
}
