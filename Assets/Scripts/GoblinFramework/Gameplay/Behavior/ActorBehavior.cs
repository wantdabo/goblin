using GoblinFramework.Gameplay.Behaviors;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behavior
{
    public class ActorBehavior : Behavior<ActorBehavior.ActorInfo>
    {
        #region ActorInfo
        public class ActorInfo : BehaviorInfo
        {
            public int actorId;
            public Fixed64Vector3 dir;
            public Fixed64Vector3 pos;
        }
        #endregion
    }
}
