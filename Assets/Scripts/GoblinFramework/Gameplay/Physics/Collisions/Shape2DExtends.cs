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
        public interface IArea
        {
            public Fixed64 area { get; }
        }

        #region GPoint
        /// <summary>
        /// Goblin-Point，点
        /// </summary>
        public struct GPoint : IEquatable<GPoint>, IArea
        {
            public Fixed64 x { get { return detail.x; } set { detail.x = value; } }
            public Fixed64 y { get { return detail.y; } set { detail.y = value; } }

            public Fixed64 area => Fixed64.Zero;

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
        public struct GRect : IEquatable<GRect>, IArea
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
        public struct GCircle : IEquatable<GCircle>, IArea
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
        public struct GTriangle : IEquatable<GTriangle>, IArea
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
        /// <summary>
        /// 点与点碰撞检测
        /// </summary>
        /// <param name="p0">点 0</param>
        /// <param name="p1">点 1</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GPoint p0, GPoint p1)
        {
            return p0.Equals(p1);
        }

        /// <summary>
        /// 点与矩形碰撞检测
        /// </summary>
        /// <param name="p">点</param>
        /// <param name="r"><矩形/param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GPoint p, GRect r)
        {
            return p.detail.x >= r.lt.detail.x &&
                   p.detail.y >= r.lt.detail.y &&
                   p.detail.x <= r.rb.detail.x &&
                   p.detail.y <= r.rb.detail.y;
        }

        /// <summary>
        /// 点与圆形碰撞检测
        /// </summary>
        /// <param name="p">点</param>
        /// <param name="c">圆形</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GPoint p, GCircle c)
        {
            return Fixed64Vector2.Distance(p.detail, c.center.detail) < c.radius;
        }

        /// <summary>
        /// 点与三角形碰撞检测
        /// </summary>
        /// <param name="p">点</param>
        /// <param name="t">三角形</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GPoint p, GTriangle t)
        {
            var tri0 = new GTriangle { p0 = t.p0, p1 = t.p1, p2 = p };
            var tri1 = new GTriangle { p0 = t.p0, p1 = t.p2, p2 = p };
            var tri2 = new GTriangle { p0 = t.p1, p1 = t.p2, p2 = p };

            return t.area.Equals(tri0.area + tri1.area + tri2.area);
        }

        /// <summary>
        /// 矩形与矩形碰撞检测
        /// </summary>
        /// <param name="r0">矩形</param>
        /// <param name="r1">矩形</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GRect r0, GRect r1)
        {
            var minX = FixedMath.Max(r0.lt.x, r1.lt.x);
            var minY = FixedMath.Max(r0.lt.y, r1.lt.y);
            var maxX = FixedMath.Max(r0.rb.x, r1.rb.x);
            var maxY = FixedMath.Max(r0.rb.y, r1.rb.y);

            return minX > maxX || minY > maxY;
        }

        /// <summary>
        /// 矩形与圆形碰撞检测
        /// </summary>
        /// <param name="r">矩形</param>
        /// <param name="c">圆形</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GRect r, GCircle c)
        {
            if (Shape2D.Collision(c.center, r)) return true;
            if (Shape2D.Collision(r.lt, c)) return true;
            if (Shape2D.Collision(r.rb, c)) return true;
            if (Shape2D.Collision(new GPoint { x = r.rb.x, y = r.lt.y }, c)) return true;
            if (Shape2D.Collision(new GPoint { x = r.lt.x, y = r.rb.y }, c)) return true;

            var p = new GPoint
            {
                x = FixedMath.Clamp(c.center.x, r.lt.x, r.rb.x),
                y = FixedMath.Clamp(c.center.y, r.rb.y, r.lt.y)
            };

            return Shape2D.Collision(p, r);
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
