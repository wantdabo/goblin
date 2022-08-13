using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Physics.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Shape
{
    public static class GoblinCollision
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
            throw new NotImplementedException();
            return p.detail.X >= r.lt.detail.X &&
                   p.detail.Y >= r.lt.detail.Y &&
                   p.detail.X <= r.rb.detail.X &&
                   p.detail.Y <= r.rb.detail.Y;
        }

        /// <summary>
        /// 点与圆形碰撞检测
        /// </summary>
        /// <param name="p">点</param>
        /// <param name="c">圆形</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GPoint p, GCircle c)
        {
            return Vector2.Distance(p.detail, c.center.detail) < c.radius;
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
            throw new NotImplementedException();
            var minX = Fix64Math.Max(r0.lt.x, r1.lt.x);
            var minY = Fix64Math.Max(r0.lt.y, r1.lt.y);
            var maxX = Fix64Math.Max(r0.rb.x, r1.rb.x);
            var maxY = Fix64Math.Max(r0.rb.y, r1.rb.y);

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
            throw new NotImplementedException();
            if (GoblinCollision.Collision(c.center, r)) return true;
            if (GoblinCollision.Collision(r.lt, c)) return true;
            if (GoblinCollision.Collision(r.rb, c)) return true;
            if (GoblinCollision.Collision(new GPoint { x = r.rb.x, y = r.lt.y }, c)) return true;
            if (GoblinCollision.Collision(new GPoint { x = r.lt.x, y = r.rb.y }, c)) return true;

            var p = new GPoint
            {
                x = Fix64Math.Clamp(c.center.x, r.lt.x, r.rb.x),
                y = Fix64Math.Clamp(c.center.y, r.rb.y, r.lt.y)
            };

            return GoblinCollision.Collision(p, r);
        }

        /// <summary>
        /// 矩形与三角形碰撞检测
        /// </summary>
        /// <param name="r">矩形</param>
        /// <param name="t">三角形</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GRect r, GTriangle t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 圆形与圆形碰撞检测
        /// </summary>
        /// <param name="r">圆形</param>
        /// <param name="t">圆形</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GCircle c0, GCircle c1)
        {
            return Vector2.Distance(c0.center.detail, c1.center.detail) < c0.radius + c1.radius;
        }

        /// <summary>
        /// 圆形与三角形碰撞检测
        /// </summary>
        /// <param name="r">圆形</param>
        /// <param name="t">三角形</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GCircle c, GTriangle t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 三角形与三角形碰撞检测
        /// </summary>
        /// <param name="r">三角形</param>
        /// <param name="t">三角形</param>
        /// <returns>碰撞状态，true 相撞，false 未相撞</returns>
        public static bool Collision(GTriangle t0, GTriangle t1)
        {
            throw new NotImplementedException();
        }
    }
}
