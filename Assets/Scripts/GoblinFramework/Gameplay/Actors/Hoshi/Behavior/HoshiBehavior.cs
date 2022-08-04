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

        public override void PLoop(int frame)
        {
            base.PLoop(frame);
            var inputBehavior = Actor.GetBehavior<InputBehavior>();
            var joystick = inputBehavior.GetInput(InputType.Joystick);

            if (joystick.press)
                EnterState<HoshiRun>();
            else
                EnterState<HoshiIdle>();
        }

        #region HoshiInfo
        public class HoshiInfo : BehaviorInfo
        {
            public Fixed64 runSpeed = 14 * Fixed64.EN2;
        }
        #endregion
    }
}
