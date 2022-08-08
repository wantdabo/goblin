using GoblinFramework.Gameplay.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    public abstract class Shape2D : PComp
    {
        public virtual void Collision()
        {
        }
    }
}
