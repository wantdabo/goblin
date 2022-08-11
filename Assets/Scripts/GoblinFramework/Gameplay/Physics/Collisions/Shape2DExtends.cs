using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    public partial class Shape2D
    {
        #region GPoint
        /// <summary>
        /// Goblin-Point，点
        /// </summary>
        public struct GPoint : IEquatable<GPoint>
        {
            public Fixed64 x { get { return detail.x; } set { detail.x = value; } }
            public Fixed64 y { get { return detail.y; } set { detail.y = value; } }

            public Fixed64Vector2 detail;

            public bool Equals(GPoint other)
            {
                return detail.x == other.detail.x && detail.y == other.detail.y;
            }

            public bool Collision(GPoint p)
            {
                return Shape2D.Collision(this, p);
            }

            public bool Collision(GRect r)
            {
                return Shape2D.Collision(this, r);
            }

            public bool Collision(GCircle c)
            {
                return Shape2D.Collision(this, c);
            }

            public bool Collision(GTriangle t)
            {
                return Shape2D.Collision(this, t);
            }
        }
        #endregion

        #region GRect
        /// <summary>
        /// Goblin-Rect，矩形
        /// </summary>
        public struct GRect : IEquatable<GRect>
        {
            /// <summary>
            /// 左上角
            /// </summary>
            public GPoint lt;
            /// <summary>
            /// 右下角
            /// </summary>
            public GPoint rb;

            public bool Equals(GRect other)
            {
                return lt.Equals(other.lt) && rb.Equals(other.rb);
            }

            public bool Collision(GPoint p)
            {
                return Shape2D.Collision(p, this);
            }

            public bool Collision(GRect r)
            {
                return Shape2D.Collision(r, this);
            }

            public bool Collision(GCircle c)
            {
                return Shape2D.Collision(this, c);
            }

            public bool Collision(GTriangle t)
            {
                return Shape2D.Collision(this, t);
            }
        }
        #endregion

        #region GCircle
        /// <summary>
        /// Goblin-Circle，圆形
        /// </summary>
        public struct GCircle : IEquatable<GCircle>
        {
            public GPoint pos;
            public Fixed64 radius;

            public bool Equals(GCircle other)
            {
                return radius.Equals(other.radius);
            }

            public bool Collision(GPoint p)
            {
                return Shape2D.Collision(p, this);
            }

            public bool Collision(GRect r)
            {
                return Shape2D.Collision(r, this);
            }

            public bool Collision(GCircle c)
            {
                return Shape2D.Collision(c, this);
            }

            public bool Collision(GTriangle t)
            {
                return Shape2D.Collision(this, t);
            }
        }
        #endregion

        #region GTriangle
        /// <summary>
        /// Goblin-Triangle，三角形
        /// </summary>
        public struct GTriangle : IEquatable<GTriangle>
        {
            public GPoint p0;
            public GPoint p1;
            public GPoint p2;

            public bool Equals(GTriangle other)
            {
                return p0.Equals(other.p0) && p1.Equals(other.p1) && p2.Equals(other.p2);
            }

            public bool Collision(GPoint p)
            {
                return Shape2D.Collision(p, this);
            }

            public bool Collision(GRect r)
            {
                return Shape2D.Collision(r, this);
            }

            public bool Collision(GCircle c)
            {
                return Shape2D.Collision(c, this);
            }

            public bool Collision(GTriangle t)
            {
                return Shape2D.Collision(t, this);
            }
        }
        #endregion

        #region Collision
        public static bool Collision(GPoint p0, GPoint p1)
        {
            return p0.Equals(p1);
        }

        public static bool Collision(GPoint p, GRect r)
        {
            throw new NotImplementedException();
        }

        public static bool Collision(GPoint p, GCircle c)
        {
            throw new NotImplementedException();
        }

        public static bool Collision(GPoint p, GTriangle t)
        {
            throw new NotImplementedException();
        }

        public static bool Collision(GRect r0, GRect r1)
        {
            throw new NotImplementedException();
        }

        public static bool Collision(GRect r, GCircle c)
        {
            throw new NotImplementedException();
        }

        public static bool Collision(GRect r, GTriangle t)
        {
            throw new NotImplementedException();
        }

        public static bool Collision(GCircle c0, GCircle c1)
        {
            throw new NotImplementedException();
        }

        public static bool Collision(GCircle c, GTriangle t)
        {
            throw new NotImplementedException();
        }

        public static bool Collision(GTriangle t0, GTriangle t1)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
