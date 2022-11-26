using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrueSync;

namespace GoblinFramework.Physics.Shape
{
    public struct GLine
    {
        public TSVector2 p0;
        public TSVector2 p1;

        public GLine(TSVector2 p0, TSVector2 p1) 
        {
            this.p0 = p0;
            this.p1 = p1;
        }

        /// <summary>
        /// 获得法线
        /// </summary>
        /// <param name="normalize">是否归一化/标准化向量</param>
        /// <returns>法线</returns>
        public TSVector2 GetNormal(bool normalize = false) 
        {
            var dire = p1 - p0;
            var normal = new TSVector2(-dire.y, dire.x);

            // 需要归一化
            if (normalize) return normal.normalized;

            return normal;
        }
    }
}
