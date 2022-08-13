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
    /// Circle-Collider，圆形碰撞盒
    /// </summary>
    public class CircleCollider : ShapeCollider
    {
        public GCircle circle { get; private set; }

        private Fix64 mRadius;
        public Fix64 radius
        {
            get { return mRadius; }
            set { mRadius = value; SetDirty(); }
        }

        public override GRect CalcAABB()
        {
            return new GRect
            {
                lt = new GPoint { detail = new Vector2(-radius + pos.x, radius + pos.y) },
                rb = new GPoint { detail = new Vector2(radius + pos.x, -radius + pos.y) }
            };
        }

        public override void OnDirty()
        {
            circle = new GCircle { center = pos, radius = radius };
        }
    }
}
