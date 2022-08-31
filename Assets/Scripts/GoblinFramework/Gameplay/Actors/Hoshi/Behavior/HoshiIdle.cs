using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Common.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiIdle : HoshiState
    {
        public override bool OnDetectEnter()
        {
            return false == Behavior.InputBehavior.HasAnyInput();
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            actor.actorBehaivor.SendRIL<RILState>((ril) => ril.stateName = "Idle");
        }
    }
}
