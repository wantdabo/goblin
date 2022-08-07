using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiAttackB : HoshiState
    {
        public override List<Type> PassStates => null;

        public override bool OnDetectEnter()
        {
            if (false == Behavior.InputBehavior.GetInput(Behaviors.InputType.BA).press) return false;

            var attackA = Behavior.GetState<HoshiAttackA>();

            if (attackA.countFrame >= 4) return true;

            return false;
        }

        public override bool OnDetectLeave()
        {
            return (countFrame >= Behavior.Info.attackBKeepFrame);
        }

        private int countFrame = 0;
        protected override void OnEnter()
        {
            base.OnEnter();
            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateName = "AttackB");
        }

        protected override void OnLeave()
        {
            base.OnLeave();
            countFrame = 0;
        }

        public override void OnStateTick(int frame)
        {
            base.OnStateTick(frame);
            countFrame++;
        }
    }
}
