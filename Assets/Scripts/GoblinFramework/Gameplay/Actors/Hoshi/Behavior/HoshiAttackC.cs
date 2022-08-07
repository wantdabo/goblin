using GoblinFramework.General.Gameplay.RIL.RILS;
using Numerics.Fixed;
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
            return countFrame >= Behavior.Info.attackCKeepFrame;
        }

        private int countFrame = 0;
        protected override void OnEnter()
        {
            base.OnEnter();

            countFrame = 0;

            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateName = "AttackC");
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }

        public override void OnStateTick(int frame)
        {
            base.OnStateTick(frame);
            countFrame++;

            Behavior.MotionBehavior.AddForce(Actor.ActorBehavior.Info.dire * Behavior.Info.attackCMotionForce);
        }
    }
}
