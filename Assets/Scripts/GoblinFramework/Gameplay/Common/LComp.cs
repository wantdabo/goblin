using GoblinFramework.Core;
using GoblinFramework.Gameplay.Actors;
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

        protected override void OnCreate()
        {
            base.OnCreate();
            if (this is IPLoop) Engine.TickEngine.AddPLoop(this as IPLoop);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this is IPLoop) Engine.TickEngine.RmvPLoop(this as IPLoop);
        }
    }
}
