using BEPUphysics;
using GoblinFramework.Gameplay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics
{
    public class World : PComp, IPLateLoop
    {
        public Space Space;

        protected override void OnCreate()
        {
            base.OnCreate();
            Space = new Space();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public void PLateLoop(int frame)
        {
            Space.Update();
        }
    }
}
