using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiAttackA : HoshiState
    {
        public override List<Type> PassStates => new List<Type> { typeof(HoshiAttackB) };

        public override bool OnDetectEnter()
        {
            return Behavior.InputBehavior.GetInput(InputType.BA).press;
        }

        public override bool OnDetectLeave()
        {
            return ElapsedFrames >= Behavior.Info.attackAKeepFrame;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateName = "AttackA");
        }
    }
}
