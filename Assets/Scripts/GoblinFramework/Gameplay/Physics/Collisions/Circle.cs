using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    /// <summary>
    /// Circle，圆形碰撞盒
    /// </summary>
    public class Circle : Shape2D
    {
        private Fixed64 mRadius;
        public Fixed64 radius
        {
            get { return mRadius; }
            set { mRadius = value; DirtyRect(); }
        }

        public override GPoint GenRect()
        {
            var size = radius * 2;

            return new GPoint { x = size, y = size };
        }
    }
}
