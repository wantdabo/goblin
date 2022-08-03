using GoblinFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.General.Gameplay.RIL.RILS
{
    public abstract class RIL
    {
        /// <summary>
        /// RILType, RIL 的类型
        /// </summary>
        public enum RILType
        {
            RIL,
            RILAdd,
            RILRmv,
            RILPos,
            RILState,
            RILModel,
        }

        public int actorId;
        public abstract RILType Type { get; }
    }
}
