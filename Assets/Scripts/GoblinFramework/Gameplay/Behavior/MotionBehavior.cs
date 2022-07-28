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
            Actor.ActorInfo.pos += Info.force;
            Info.force = Fixed64Vector3.zero;
        }

        #region MotionInfo
        public class MotionInfo : LInfo
        {
            public Fixed64Vector3 force;

            public override object Clone()
            {
                MotionInfo motionInfo = new MotionInfo();
                motionInfo.force = Fixed64Vector3.zero;

                return motionInfo;
            }
        }
        #endregion
    }
}
