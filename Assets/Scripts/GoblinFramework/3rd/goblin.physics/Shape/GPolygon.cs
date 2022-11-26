using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace GoblinFramework.Physics.Shape
{
    public struct GPolygon
    {
        public TSVector2 position;
        public List<TSVector2> vertexes;

        public GLine[] GetLines()
        {
            if (null == vertexes) throw new Exception("donot has vertexes.");

            var vertexCount = vertexes.Count;
            GLine[] lines = new GLine[vertexCount];
            for (int i = 0; i < vertexCount - 1; i++)
            {
                // 为了内存命中率，n - 1 的连续遍历采用高速计算线段，末端计算
                var v0 = vertexes[i];
                var v1 = vertexes[i + 1];
                lines[i].p0 = v0;
                lines[i].p1 = v1;
            }

            // 最后一个顶点与第一个顶点的计算线段，末端分段计算
            var eV0 = vertexes[vertexCount - 1];
            var eV1 = vertexes[0];
            lines[vertexCount - 1].p0 = eV0;
            lines[vertexCount - 1].p1 = eV1;

            return lines;
        }

        /// <summary>
        /// 获取法线列表
        /// </summary>
        /// <param name="normalize">是否归一化/标准化向量</param>
        /// <returns>多边形的法线列表</returns>
        public TSVector2[] GetNormals(bool normalize = false)
        {
            var lines = GetLines();
            TSVector2[] normals = new TSVector2[lines.Length];
            for (int i = 0; i < lines.Length; i++) normals[i] = lines[i].GetNormal(normalize);

            return normals;
        }
    }
}
