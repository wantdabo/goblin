using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiAttackCHold : HoshiState
    {
        public override List<Type> PassStates => new List<Type> { typeof(HoshiAttackC) };

        public override bool OnDetectEnter()
        {
            return Behavior.InputBehavior.GetInput(Behaviors.InputType.BB).press;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            actor.actorBehaivor.SendRIL<RILState>((ril) => ril.stateName = "AttackCHold");
        }
    }
}
