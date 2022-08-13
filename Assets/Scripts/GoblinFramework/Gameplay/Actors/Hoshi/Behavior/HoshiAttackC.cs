using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiAttackC : HoshiState
    {
        public override List<Type> PassStates => null;

        public override bool OnDetectEnter()
        {
            return Behavior.InputBehavior.GetInput(Behaviors.InputType.BB).release;
        }

        public override bool OnDetectLeave()
        {
            return ElapsedFrames >= Behavior.Info.attackCKeepFrame;
        }

        protected override void OnEnter()
        {
            base.OnEnter();

            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateName = "AttackC");
        }

        public override void OnStateTick(int frame)
        {
            base.OnStateTick(frame);

            Behavior.MotionBehavior.AddForce(Actor.ActorBehavior.Info.dire * Behavior.Info.attackCMotionForce);
        }
    }
}
