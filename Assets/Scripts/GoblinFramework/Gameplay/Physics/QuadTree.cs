using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics.Collisions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics
{
    /// <summary>
    /// Quad-Tree， 四叉平面碰撞树
    /// </summary>
    public class QuadTree : PComp
    {
        /// <summary>
        /// 物理世界
        /// </summary>
        public World World { get; set; }

        /// <summary>
        /// 树的细分深度
        /// </summary>
        public int Depth { get; set; } = int.MaxValue;

        public Quad quad;

        protected override void OnCreate()
        {
            base.OnCreate();

            quad = GenQuad(0, World.Bounds);
        }

        /// <summary>
        /// 一次性生成四叉树
        /// </summary>
        /// <param name="depth">节点深度</param>
        /// <param name="bounds">节点大小</param>
        /// <returns>节点</returns>
        public Quad GenQuad(int depth, int bounds)
        {
            Quad quad = new Quad();
            quad.depth = depth;
            quad.iBounds = bounds;
            quad.oBounds = bounds;
            quad.quads = null;

            if (1 == quad.iBounds || depth >= Depth) return quad;

            quad.quads = new Quad[4];
            for (int i = 0; i < quad.quads.Length; i++)
            {
                quad.quads[i] = GenQuad(depth + 1, bounds / 2);
            }

            return quad;
        }

        /// <summary>
        /// 四叉树
        /// </summary>
        public struct Quad
        {
            /// <summary>
            /// 当前节点深度
            /// </summary>
            public int depth;

            /// <summary>
            /// In-Bound-X，松散四叉树，入范围
            /// </summary>
            public int iBounds;

            /// <summary>
            /// Out-Bound-X，松散四叉树，出范围
            /// </summary>
            public int oBounds;

            /// <summary>
            /// 该节点下的所有物体
            /// </summary>
            public List<Shape2D> shapeList;

            /// <summary>
            /// [1, 0]
            /// [3, 2]
            /// 四叉树，对应的四个位置
            /// </summary>
            public Quad[] quads;
        }
    }
}
