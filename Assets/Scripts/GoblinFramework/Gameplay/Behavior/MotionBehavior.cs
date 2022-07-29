using GoblinFramework.Gameplay.Common;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Behaviors
{
    public class MotionBehavior : Behavior<MotionBehavior.MotionInfo>, IPLoop
    {
        public void AddForce(Fixed64Vector3 force) 
        {
            Info.force += force;
        }

        public void PLoop(int frame)
        {
            Actor.ActorBehavior.Info.pos += Info.force;
            Info.force = Fixed64Vector3.zero;
        }

        #region MotionInfo
        public class MotionInfo : BehaviorInfo
        {
            public Fixed64Vector3 force;
        }
        #endregion
    }
}
