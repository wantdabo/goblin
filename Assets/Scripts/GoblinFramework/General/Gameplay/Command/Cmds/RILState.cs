using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.RIL.RILS
{
    public class RILState : RIL
    {
        public int stateId;

        public override RILType Type => RILType.RILState;
    }
}
