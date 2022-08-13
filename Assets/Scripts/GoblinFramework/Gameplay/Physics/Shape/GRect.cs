using GoblinFramework.Gameplay.Physics.Collisions;
using GoblinFramework.Gameplay.Physics.Shape;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Shape
{
    /// <summary>
    /// Goblin-Rect，矩形
    /// </summary>
    public struct GRect : IEquatable<GRect>, IShape
    {
        /// <summary>
        /// 中心点
        /// </summary>
        public GPoint center => new GPoint { x = rb.x - lt.x, y = lt.y - rb.y };
        /// <summary>
        /// 左上角
        /// </summary>
        public GPoint lt;
        /// <summary>
        /// 右下角
        /// </summary>
        public GPoint rb;

        public Fixed64 area
        {
            get
            {
                return FixedMath.Abs(lt.x - rb.x) * FixedMath.Abs(lt.y - rb.y);
            }
        }

        public bool Equals(GRect other)
        {
            return lt.Equals(other.lt) && rb.Equals(other.rb);
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
            return GoblinCollision.Collision(this, c);
        }

        public bool Collision(GTriangle t)
        {
            return GoblinCollision.Collision(this, t);
        }
    }
}
