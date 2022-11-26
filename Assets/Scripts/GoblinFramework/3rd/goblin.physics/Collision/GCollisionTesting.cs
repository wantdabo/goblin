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
            var normals = p1.GetNormals();
            for (int i = 0; i < normals.Length; i++)
            {
                // side > 0 表示该点位于法线的外部，反之则是内部。此处凸面多边形，如果点在任意一条法线的外部，表示未碰撞。
                var side = TSVector2.Dot(p0, normals[i]);
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

            void lineProduct(TSVector2 normal, GLine line, out FP min, out FP max)
            {
                min = FP.MaxValue;
                max = FP.MinValue;

                var dot0 = TSVector2.Dot(normal, line.p0);
                min = TSMath.Min(dot0, min);
                max = TSMath.Max(dot0, max);

                var dot1 = TSVector2.Dot(normal, line.p1);
                max = TSMath.Min(dot1, max);
                max = TSMath.Max(dot1, max);
            }

            void circleProduct(TSVector2 normal, GCircle circle, out FP min, out FP max) 
            {
                min = FP.MaxValue;
                max = FP.MinValue;

                var rad = TSMath.Atan2(normal.y, normal.x);
                var circlePoint = new TSVector2(TSMath.Cos(rad) * circle.radius, TSMath.Sin(rad) * circle.radius);
                var p1 = circle.position - circlePoint;

                var dot0 = TSVector2.Dot(normal, circle.position + circlePoint);
                min = TSMath.Min(dot0, min);
                max = TSMath.Max(dot0, max);

                var dot1 = TSVector2.Dot(normal, circle.position - circlePoint);
                min = TSMath.Min(dot1, min);
                max = TSMath.Max(dot1, max);
            }

            var normal = l0.GetNormal(true);
            lineProduct(normal, l0, out var l0Min, out var l0Max);
            circleProduct(normal, c0, out var c0Min, out var c0Max);

            // 检测重叠
            return l0Min <= c0Max && l0Min >= c0Min || l0Max <= c0Max && l0Max >= c0Min;
        }

        /// <summary>
        /// 测试线与多边形的碰撞
        /// </summary>
        /// <param name="l0">线 1</param>
        /// <param name="p0">多边形 1</param>
        /// <returns>是否相撞</returns>
        public static bool Test(GLine l0, GPolygon p0)
        {
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
            // 投影到一维
            void dotProduct(TSVector2[] normals, List<TSVector2> vertexes, out FP min, out FP max)
            {
                min = FP.MaxValue;
                max = FP.MinValue;
                for (int i = 0; i < normals.Length; i++)
                {
                    var normal = normals[i];
                    foreach (var vec in vertexes)
                    {
                        var dot = TSVector2.Dot(normal, vec);
                        min = TSMath.Min(dot, min);
                        max = TSMath.Max(dot, max);
                    }
                }
            }

            var normals = p0.GetNormals();
            dotProduct(normals, p0.vertexes, out var p0Min, out var p0Max);
            dotProduct(normals, p1.vertexes, out var p1Min, out var p1Max);

            // 检测重叠
            return p0Min <= p1Max && p0Min >= p1Min || p0Max <= p1Max && p0Max >= p1Min;
        }
    }
}
