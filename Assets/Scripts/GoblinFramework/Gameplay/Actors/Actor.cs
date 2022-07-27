using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Comps;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Actors
{
    public class Actor : LComp
    {
        public ActorInfo ActorInfo = new ActorInfo();
    }

    #region ActorInfo
    public class ActorInfo : LInfo
    {
        public int actorId;
        public int angle;
        public Fixed64Vector3 pos;

        public override object Clone()
        {
            var actorInfo = new ActorInfo();
            actorInfo.actorId = actorId;
            actorInfo.pos = pos;

            return actorInfo;
        }
    }
    #endregion
}
