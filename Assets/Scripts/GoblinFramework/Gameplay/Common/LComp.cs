using GoblinFramework.Core;
using GoblinFramework.Gameplay.Actors;
using GoblinFramework.Gameplay.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Common
{
    /// <summary>
    /// Logic-Comp，逻辑层组件
    /// </summary>
    public class LComp : Comp<PGEngine>
    {
        public Actor Actor;
    }
}
