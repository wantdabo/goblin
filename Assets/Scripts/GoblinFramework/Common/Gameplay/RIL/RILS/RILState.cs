using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Common.Gameplay.RIL.RILS
{
    public class RILState : RIL
    {
        public int machine;
        public string stateName;

        public override RILType Type => RILType.RILState;
    }
}
