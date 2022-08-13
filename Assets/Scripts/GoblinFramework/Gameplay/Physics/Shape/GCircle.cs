using FixMath.NET;
using GoblinFramework.Gameplay.Physics.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Shape
{
    /// <summary>
    /// Goblin-Circle，圆形
    /// </summary>
    public struct GCircle : IEquatable<GCircle>, IShape
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public GPoint center;
        /// <summary>
        /// 半径
        /// </summary>
        public Fix64 radius;

        public Fix64 area => Fix64Math.Pi * (radius * radius);

        public bool Equals(GCircle other)
        {
            return radius.Equals(other.radius);
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
            return GoblinCollision.Collision(this, t);
        }
    }
}
