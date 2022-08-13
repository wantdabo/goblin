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
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public void PLateLoop(int frame)
        {
        }
    }
}
