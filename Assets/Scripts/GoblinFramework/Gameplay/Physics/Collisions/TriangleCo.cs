using GoblinFramework.Gameplay.Physics.Shape;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    /// <summary>
    /// Triangle-Collider，三角形碰撞盒
    /// </summary>
    public class TriangleCo : ShapeCo
    {
        public GTriangle triangle { get; private set; }

        private GTriangle mVertex;
        public GTriangle vertex
        {
            get { return mVertex; }
            set { mVertex = value; SetDirty(); }
        }

        public override GRect ComputeAABB()
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

            return new GRect() { lt = new GPoint { detail = new Fixed64Vector2(minX, maxY) }, rb = new GPoint { detail = new Fixed64Vector2(maxX, minY) } };
        }

        public override void OnDirty()
        {
            triangle = new GTriangle
            {
                center = pos,
                p0 = new GPoint { detail = vertex.p0.detail + pos.detail },
                p1 = new GPoint { detail = vertex.p1.detail + pos.detail },
                p2 = new GPoint { detail = vertex.p2.detail + pos.detail }
            };
        }
    }
}
