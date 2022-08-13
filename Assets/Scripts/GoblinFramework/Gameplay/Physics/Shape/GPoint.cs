using BEPUutilities;
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
    /// Goblin-Point，点
    /// </summary>
    public struct GPoint : IEquatable<GPoint>, IShape
    {
        public Fix64 x { get { return detail.X; } set { detail.X = value; } }
        public Fix64 y { get { return detail.Y; } set { detail.Y = value; } }

        public Fix64 area => Fix64.Zero;

        public Vector2 detail;

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
