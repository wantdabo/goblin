using FixMath.NET;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.General.Gameplay.RIL.RILS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors.Hoshi.Behavior
{
    public class HoshiFalling : HoshiState
    {
        public override List<Type> PassStates => null;

        public override bool OnDetectEnter()
        {
            return false == Behavior.ColliderBehavior.IsOnGround;
        }

        public override bool OnDetectLeave()
        {
            return Behavior.ColliderBehavior.IsOnGround;
        }

        protected override void OnEnter()
        {
            base.OnEnter();
            Actor.ActorBehavior.SendRIL<RILState>((ril) => ril.stateName = "Falling");
        }
    }
}
