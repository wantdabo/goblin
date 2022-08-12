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
        public Fixed64 radius;

        public Fixed64 area => FixedMath.Pi * (radius * radius);

        public bool Equals(GCircle other)
        {
            return radius.Equals(other.radius);
        }

        public bool Collision(GPoint p)
        {
            return ShapeCo.Collision(p, this);
        }

        public bool Collision(GRect r)
        {
            return ShapeCo.Collision(r, this);
        }

        public bool Collision(GCircle c)
        {
            return ShapeCo.Collision(c, this);
        }

        public bool Collision(GTriangle t)
        {
            return ShapeCo.Collision(this, t);
        }
    }
}
