using GoblinFramework.Physics.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace GoblinFramework.Physics.Collision
{
    /// <summary>
    /// 碰撞检测
    /// </summary>
    public static class GCollisionTesting
    {
        /// <summary>
        /// 测试两点碰撞
        /// </summary>
        /// <param name="p0">点 1</param>
        /// <param name="p1">点 2</param>
        /// <returns>是否碰撞</returns>
        public static bool Test(TSVector2 p0, TSVector2 p1)
        {
            return p0 == p1;
        }

        /// <summary>
        /// 测试点与线的碰撞
        /// </summary>
        /// <param name="p0">点 1</param>
        /// <param name="l0">线 1</param>
        /// <returns>是否碰撞</returns>
        public static bool Test(TSVector2 p0, GLine l0)
        {
            var lineLen = TSVector2.Distance(l0.p0, l0.p1);
            var dis0 = TSVector2.Distance(p0, l0.p0);
            var dis1 = TSVector2.Distance(p0, l0.p1);

            // 应该相等才是相撞，避免计算误差，降低精度 FP.EN1(0.1f)
            return TSMath.Abs((dis0 + dis1) - lineLen) < FP.EN1;
        }

        /// <summary>
        /// 测试点与圆的碰撞
        /// </summary>
        /// <param name="p0">点 1</param>
        /// <param name="c0">圆 1</param>
        /// <returns>是否碰撞</returns>
        public static bool Test(TSVector2 p0, GCircle c0)
        {
            return TSVector2.Distance(p0, c0.position) <= c0.radius;
        }

        /// <summary>
        /// 测试点与多边形的碰撞检测
        /// </summary>
        /// <param name="p0">点 1</param>
        /// <param name="p1">多边形 1</param>
        /// <returns>是否碰撞</returns>
        public static bool Test(TSVector2 p0, GPolygon p1)
        {
            // from https://docs.godotengine.org/zh_CN/latest/tutorials/math/vectors_advanced.html#distance-to-plane
            var planes = p1.GetPlanes();
            for (int i = 0; i < planes.Length; i++)
            {
                var plane = planes[i];
                // side > 0 表示该点位于平面的外部，反之则是内部。此处凸面多边形，如果点在任意一条平面的外部，表示未碰撞。
                var side = TSVector2.Dot(p0, plane.ToTSVector2()) - plane.z;
                if (side > 0) return false;
            }

            return true;
        }

        /// <summary>
        /// 测试线与线的碰撞
        /// </summary>
        /// <param name="l0">线 1</param>
        /// <param name="l1">线 2</param>
        /// <returns>是否碰撞</returns>
        public static bool Test(GLine l0, GLine l1)
        {
            // from https://github.com/XXHolic/blog/issues/61#situation3
            // (x1, y1) 线1的一个端点
            // (x2, y2) 线1的另一个端点
            // (x3, y3) 线2的一个端点
            // (x4, y4) 线2的另一个端点
            FP x1 = l0.p0.x; FP y1 = l0.p0.y;
            FP x2 = l0.p1.x; FP y2 = l0.p1.y;
            FP x3 = l1.p0.x; FP y3 = l1.p0.y;
            FP x4 = l1.p1.x; FP y4 = l1.p1.y;

            FP t1 = ((x4 - x3) * (y1 - y3) - (y4 - y3) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));
            FP t2 = ((x2 - x1) * (y1 - y3) - (y2 - y1) * (x1 - x3)) / ((y4 - y3) * (x2 - x1) - (x4 - x3) * (y2 - y1));

            return t1 >= 0 && t1 <= 1 && t2 >= 0 && t2 <= 1;
        }

        /// <summary>
        /// 检测线与圆的碰撞
        /// </summary>
        /// <param name="l0">线 1</param>
        /// <param name="c0">圆 1</param>
        /// <returns>是否相撞</returns>
        public static bool Test(GLine l0, GCircle c0)
        {
            // 快速检测，两点是否在圆内
            if (Test(l0.p0, c0) || Test(l0.p1, c0)) return true;

            // 快速检测，两点是否在圆内
            if (Test(l0.p0, c0) || Test(l0.p1, c0)) return true;

            // from https://github.com/XXHolic/blog/issues/61#situation3
            // (x1, y1) 线的一个端点
            // (x2, y2) 线的另一个端点
            // (px, py) 圆心的坐标
            // radius   圆的半径
            FP x1 = l0.p0.x; FP y1 = l0.p0.y;
            FP x2 = l0.p1.x; FP y2 = l0.p1.y;
            FP cx = c0.position.x; FP cy = c0.position.y;
            FP radius = c0.radius;

            FP pointVectorX = x1 - x2;
            FP pointVectorY = y1 - y2;
            FP t = (pointVectorX * (cx - x1) + pointVectorY * (cy - y1)) / (pointVectorX * pointVectorX + pointVectorY * pointVectorY);
            FP closestX = x1 + t * pointVectorX;
            FP closestY = y1 + t * pointVectorY;

            if (false == Test(new TSVector2(closestX, closestY), l0)) return false;

            FP distX = closestX - cx;
            FP distY = closestY - cy;
            FP distance = TSMath.Sqrt((distX * distX) + (distY * distY));

            return distance <= radius;
        }

        /// <summary>
        /// 测试线与多边形的碰撞
        /// </summary>
        /// <param name="l0">线 1</param>
        /// <param name="p0">多边形 1</param>
        /// <returns>是否相撞</returns>
        public static bool Test(GLine l0, GPolygon p0)
        {
            // 检测线的端点是否在多边形内
            if (Test(l0.p0, p0) || Test(l0.p1, p0)) return true;

            var lines = p0.GetLines();
            for (int i = 0; i < lines.Length; i++) if (Test(l0, lines[i])) return true;

            return false;
        }

        /// <summary>
        /// 测试圆与圆的碰撞检测
        /// </summary>
        /// <param name="c0">圆 1</param>
        /// <param name="c1">圆 2</param>
        /// <returns>是否碰撞</returns>
        public static bool Test(GCircle c0, GCircle c1)
        {
            return TSVector2.Distance(c0.position, c1.position) <= c0.radius + c1.radius;
        }

        /// <summary>
        /// 测试圆与多边形的碰撞检测
        /// </summary>
        /// <param name="c0">圆 1</param>
        /// <param name="p0">多边形 1</param>
        /// <returns>是否碰撞</returns>
        public static bool Test(GCircle c0, GPolygon p0)
        {
            var lines = p0.GetLines();
            for (int i = 0; i < lines.Length; i++) if (Test(lines[i], c0)) return true;

            return false;
        }

        /// <summary>
        /// 测试多边形与多边形的碰撞检测
        /// </summary>
        /// <param name="p0">多边形 1</param>
        /// <param name="p1">多边形 2</param>
        /// <returns>是否碰撞</returns>
        public static bool Test(GPolygon p0, GPolygon p1)
        {
            var lines0 = p0.GetLines();
            for (int i = 0; i < lines0.Length; i++) if (Test(lines0[i], p1)) return true;

            var lines1 = p1.GetLines();
            for (int i = 0; i < lines1.Length; i++) if (Test(lines1[i], p0)) return true;

            return false;
        }
    }
}
