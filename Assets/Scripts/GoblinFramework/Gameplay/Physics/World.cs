using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics
{
    public class World : PComp, IPLateLoop
    {
        private int bounds = 32;
        public int Bounds
        {
            get { return bounds; }
            set
            {
                if (0 == bounds || 0 != (bounds & bounds - 1))
                    throw new Exception("boundX need 2 power number. and not (0) zero");

                bounds = value;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        public void PLateLoop(int frame)
        {
        }
    }
}
