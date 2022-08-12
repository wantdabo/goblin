using GoblinFramework.Gameplay.Physics.Shape;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    public partial class ShapeCo
    {
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
            if (ShapeCo.Collision(c.center, r)) return true;
            if (ShapeCo.Collision(r.lt, c)) return true;
            if (ShapeCo.Collision(r.rb, c)) return true;
            if (ShapeCo.Collision(new GPoint { x = r.rb.x, y = r.lt.y }, c)) return true;
            if (ShapeCo.Collision(new GPoint { x = r.lt.x, y = r.rb.y }, c)) return true;

            var p = new GPoint
            {
                x = FixedMath.Clamp(c.center.x, r.lt.x, r.rb.x),
                y = FixedMath.Clamp(c.center.y, r.rb.y, r.lt.y)
            };

            return ShapeCo.Collision(p, r);
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
    }
}
