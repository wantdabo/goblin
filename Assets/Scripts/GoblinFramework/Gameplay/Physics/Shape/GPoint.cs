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
    /// Goblin-Point，点
    /// </summary>
    public struct GPoint : IEquatable<GPoint>, IShape
    {
        public Fixed64 x { get { return detail.x; } set { detail.x = value; } }
        public Fixed64 y { get { return detail.y; } set { detail.y = value; } }

        public Fixed64 area => Fixed64.Zero;

        public Fixed64Vector2 detail;

        public bool Equals(GPoint other)
        {
            return detail == other.detail;
        }

        public bool Collision(GPoint p)
        {
            return GoblinCollision.Collision(this, p);
        }

        public bool Collision(GRect r)
        {
            return GoblinCollision.Collision(this, r);
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
