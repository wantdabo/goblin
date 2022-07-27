using GoblinFramework.Gameplay.Common;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Comps
{
    public class MotionComp : BehaviorComp<MotionComp.MotionInfo>, IPLoop
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
                throw new NotImplementedException();
            }
        }
        #endregion
    }
}
