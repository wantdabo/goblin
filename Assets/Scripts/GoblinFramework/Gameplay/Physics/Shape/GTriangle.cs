using GoblinFramework.Gameplay.Physics.Collisions;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Shape
{
    /// <summary>
    /// Goblin-Triangle，三角形
    /// </summary>
    public struct GTriangle : IEquatable<GTriangle>, IShape
    {
        public GPoint center;

        public GPoint p0;
        public GPoint p1;
        public GPoint p2;

        /// <summary>
        /// 海伦-秦九韶公式
        /// 已知三边是a, b, c
        /// 令p=(a+b+c)/2
        /// 则S=√[p(p - a)(p-b)(p-c)]
        /// </summary>
        public Fixed64 area
        {
            get
            {
                var a = Fixed64Vector2.Distance(p0.detail, p1.detail);
                var b = Fixed64Vector2.Distance(p0.detail, p2.detail);
                var c = Fixed64Vector2.Distance(p1.detail, p2.detail);
                var p = (a + b + c) * Fixed64.Half;

                return FixedMath.Sqrt(p * (p - a) * (p - b) * (p - c));
            }
        }

        public bool Equals(GTriangle other)
        {
            return p0.Equals(other.p0) && p1.Equals(other.p1) && p2.Equals(other.p2);
        }

        public bool Collision(GPoint p)
        {
            return GoblinCollision.Collision(p, this);
        }

        public bool Collision(GRect r)
        {
            return GoblinCollision.Collision(r, this);
        }

        public bool Collision(GCircle c)
        {
            return GoblinCollision.Collision(c, this);
        }

        public bool Collision(GTriangle t)
        {
            return GoblinCollision.Collision(t, this);
        }
    }
}
