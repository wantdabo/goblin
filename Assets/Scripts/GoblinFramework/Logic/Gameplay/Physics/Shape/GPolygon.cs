using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace GoblinFramework.Logic.Gameplay.Physics.Shape
{
    public struct GPolygon
    {
        public TSVector2 position;
        public List<TSVector2> vertexes;

        /// <summary>
        /// 获取法线列表
        /// </summary>
        /// <param name="normalize">是否归一化/标准化向量</param>
        /// <returns>多边形的法线列表</returns>
        /// <exception cref="Exception">顶点未初始化/未拥有顶点</exception>
        public TSVector2[] GetNormals(bool normalize = false)
        {
            if (null == vertexes) throw new Exception("donot has vertexes.");

            var vertexCount = vertexes.Count;
            TSVector2[] normals = new TSVector2[vertexCount];
            for (int i = 0; i < vertexes.Count - 1; i++)
            {
                // 为了内存命中率，n - 1 的连续遍历采用高速计算法向量，末端计算
                var v0 = vertexes[i];
                var v1 = vertexes[i + 1];
                var v2 = v1 - v0;
                normals[i] = new TSVector2(-v2.y, v2.x);
            }

            // 最后一个顶点与第一个地点的计算法向量，末端分段计算
            var eV0 =  vertexes[vertexCount - 1];
            var eV1 = vertexes[0];
            var eV2 = eV1 - eV0;
            normals[vertexCount - 1] = new TSVector2(-eV2.y, eV2.x);

            // 需要归一化
            if (normalize) for(int i = 0; i < normals.Length; i++) normals[i].Normalize();

            return normals;
        }
    }
}
