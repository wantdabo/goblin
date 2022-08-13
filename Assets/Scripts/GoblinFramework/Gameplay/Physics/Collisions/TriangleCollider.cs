using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Physics.Shape;
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
    public class TriangleCollider : ShapeCollider
    {
        public GTriangle triangle { get; private set; }

        private GTriangle mVertex;
        public GTriangle vertex
        {
            get { return mVertex; }
            set { mVertex = value; SetDirty(); }
        }

        public override GRect CalcAABB()
        {
            Fix64 minX = Fix64.MaxValue;
            Fix64 maxX = Fix64.MinValue;
            minX = Fix64Math.Min(triangle.p0.x, minX);
            minX = Fix64Math.Min(triangle.p1.x, minX);
            minX = Fix64Math.Min(triangle.p2.x, minX);
            maxX = Fix64Math.Max(triangle.p0.x, maxX);
            maxX = Fix64Math.Max(triangle.p1.x, maxX);
            maxX = Fix64Math.Max(triangle.p2.x, maxX);

            Fix64 minY = Fix64.MaxValue;
            Fix64 maxY = Fix64.MinValue;
            minY = Fix64Math.Min(triangle.p0.y, minY);
            minY = Fix64Math.Min(triangle.p1.y, minY);
            minY = Fix64Math.Min(triangle.p2.y, minY);
            maxY = Fix64Math.Max(triangle.p0.y, maxY);
            maxY = Fix64Math.Max(triangle.p1.y, maxY);
            maxY = Fix64Math.Max(triangle.p2.y, maxY);

            return new GRect() { lt = new GPoint { detail = new Vector2(minX, maxY) }, rb = new GPoint { detail = new Vector2(maxX, minY) } };
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
