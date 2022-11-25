using GoblinFramework.Logic.Gameplay.Physics.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace GoblinFramework.Logic.Gameplay.Physics.Collision
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
        /// <returns>真假结果</returns>
        public static bool Test(TSVector2 p0, TSVector2 p1)
        {
            return p0 == p1;
        }

        /// <summary>
        /// 测试点与圆的碰撞
        /// </summary>
        /// <param name="p0">点 1</param>
        /// <param name="c0">圆 1</param>
        /// <returns>真假结果</returns>
        public static bool Test(TSVector2 p0, GCircle c0)
        {
            return TSVector2.Distance(p0, c0.position) <= c0.radius;
        }

        /// <summary>
        /// 测试点与多边形的碰撞检测
        /// </summary>
        /// <param name="p0">点 1</param>
        /// <param name="p1">多边形 1</param>
        /// <returns>真假结果</returns>
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
        /// 测试圆与圆的碰撞检测
        /// </summary>
        /// <param name="c0">圆 1</param>
        /// <param name="c1">圆 2</param>
        /// <returns>真假结果</returns>
        public static bool Test(GCircle c0, GCircle c1)
        {
            return TSVector2.Distance(c0.position, c1.position) <= c0.radius + c1.radius;
        }

        /// <summary>
        /// 测试圆与多边形的碰撞检测
        /// </summary>
        /// <param name="c0">圆 1</param>
        /// <param name="p0">多边形 1</param>
        /// <returns>真假结果</returns>
        /// <exception cref="NotImplementedException">未实现</exception>
        public static bool Test(GCircle c0, GPolygon p0)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 测试多边形与多边形的碰撞检测
        /// </summary>
        /// <param name="c0">多边形 1</param>
        /// <param name="p0">多边形 2</param>
        /// <returns>真假结果</returns>
        /// <exception cref="NotImplementedException">未实现</exception>
        public static bool Test(GPolygon p0, GPolygon p1)
        {
            throw new NotImplementedException();
        }
    }
}
