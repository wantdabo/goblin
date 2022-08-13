using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.RIL.RILS
{
    public class RILPos : RIL
    {
        public int dire;
        public float x, y, z;

        public override RILType Type => RILType.RILPos;
    }
}
