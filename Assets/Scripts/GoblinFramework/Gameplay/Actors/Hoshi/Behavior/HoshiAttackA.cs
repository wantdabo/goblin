using GoblinFramework.General.Gameplay.RIL.RILS;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiAttackA : HoshiState
    {
        public override List<Type> PassStates => null;

        public override bool OnDetect()
        {
            var release = Behavior.InputBehavior.GetInput(Behaviors.InputType.BA).release;

            return release;
        }

        private int countFrame = 0;
        protected override void OnEnter()
        {
            base.OnEnter();
            countFrame = 0;

            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateId = 3);
        }

        protected override void OnLeave()
        {
            base.OnLeave();
        }

        public override void OnStateTick(int frame)
        {
            base.OnStateTick(frame);
            countFrame++;
            if (countFrame >= Behavior.Info.attackAKeepFrame) Leave();

            Behavior.MotionBehavior.AddForce(Actor.ActorBehavior.Info.dire * Behavior.Info.attackAMotionForce);
        }
    }
}
