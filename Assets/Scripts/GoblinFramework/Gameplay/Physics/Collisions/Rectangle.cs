using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    /// <summary>
    /// Rectangle，矩形碰撞盒
    /// </summary>
    public class Rectangle : Shape2D
    {
        private GPoint mBounds;
        public GPoint bounds
        {
            get { return bounds; }
            set { bounds = value; DirtyRect(); }
        }

        public override GPoint GenRect()
        {
            return bounds;
        }
    }
}
