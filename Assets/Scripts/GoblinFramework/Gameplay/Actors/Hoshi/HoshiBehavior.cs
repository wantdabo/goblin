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
        public MotionBehavior MotionBehavior;
        public ColliderBehavior ColliderBehavior;

        protected override void OnCreate()
        {
            base.OnCreate();

            Actor.ActorBehavior.SendRIL<RILModel>((ril) => ril.modelName = "Hoshi/Hoshi");

            InputBehavior = Actor.GetBehavior<InputBehavior>();
            MotionBehavior = Actor.GetBehavior<MotionBehavior>();
            ColliderBehavior = Actor.GetBehavior<ColliderBehavior>();

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
            public readonly Fix64 runSpeed = 6;

            public readonly int attackAKeepFrame = 5;
            public readonly int attackBKeepFrame = 3;

            public readonly int attackCKeepFrame = 10;
            public readonly Fix64 attackCMotionForce = 10 * Fix64.EN1;

            public readonly Vector3 jumpMotionForce = new Vector3(0, 55 * Fix64.EN1, 0);

            /// <summary>
            /// 跳跃状态下的力量
            /// </summary>
            public Vector3 jumpingForce = Vector3.Zero;
        }
        #endregion
    }
}
