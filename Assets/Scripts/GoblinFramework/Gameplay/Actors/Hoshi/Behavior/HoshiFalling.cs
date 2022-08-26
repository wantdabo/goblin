using FixMath.NET;
using GoblinFramework.Gameplay.Behaviors;
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
            General.GoblinDebug.Log("========> Falling Enter");
        }

        protected override void OnLeave()
        {
            base.OnLeave();
            General.GoblinDebug.Log("========> Falling Leave");
        }

        public override void OnStateTick(int frame, Fix64 detailTime)
        {
            base.OnStateTick(frame, detailTime);
            General.GoblinDebug.Log("========> Falling");
        }
    }
}
