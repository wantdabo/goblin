using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    /// <summary>
    /// Triangle，三角形碰撞盒
    /// </summary>
    public class Triangle : Shape2D
    {
        private GTriangle mTriangle;
        public GTriangle triangle
        {
            get { return mTriangle; }
            set { mTriangle = value; DirtyRect(); }
        }

        public override GPoint GenRect()
        {
            Fixed64 minX = Fixed64.MaxValue;
            Fixed64 maxX = Fixed64.MinValue;
            minX = FixedMath.Min(triangle.p0.x, minX);
            minX = FixedMath.Min(triangle.p1.x, minX);
            minX = FixedMath.Min(triangle.p2.x, minX);
            maxX = FixedMath.Max(triangle.p0.x, maxX);
            maxX = FixedMath.Max(triangle.p1.x, maxX);
            maxX = FixedMath.Max(triangle.p2.x, maxX);

            Fixed64 minY = Fixed64.MaxValue;
            Fixed64 maxY = Fixed64.MinValue;
            minY = FixedMath.Min(triangle.p0.y, minY);
            minY = FixedMath.Min(triangle.p1.y, minY);
            minY = FixedMath.Min(triangle.p2.y, minY);
            maxY = FixedMath.Max(triangle.p0.y, maxY);
            maxY = FixedMath.Max(triangle.p1.y, maxY);
            maxY = FixedMath.Max(triangle.p2.y, maxY);

            return new GPoint() { x = maxX - minX, y = maxY - minY };
        }
    }
}
