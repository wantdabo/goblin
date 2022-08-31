using GoblinFramework.Common.Gameplay.RIL.RILS;
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

            var attackA = Behavior.State as HoshiAttackA;
            if (null == attackA) return false;

            if (attackA.ElapsedFrames >= 4) return true;

            return false;
        }

        public override bool OnDetectLeave()
        {
            return (ElapsedFrames >= Behavior.info.attackBKeepFrame);
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            actor.actorBehaivor.SendRIL<RILState>((ril) => ril.stateName = "AttackB");
        }
    }
}
