using GoblinFramework.Core;
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

        private Quad quad;

        protected override void OnCreate()
        {
            base.OnCreate();

            quad = GenQuad(quad, World.Bounds);
        }

        /// <summary>
        /// 一次性生成四叉树
        /// </summary>
        /// <param name="bounds">节点大小</param>
        /// <returns>节点</returns>
        public Quad GenQuad(Quad parent, int bounds)
        {
            Quad quad = new Quad();
            quad.Engine = Engine;
            quad.parent = parent;
            quad.iBounds = bounds;
            quad.oBounds = bounds;
            quad.quads = null;

            return quad;
        }

        private Dictionary<Shape2D, Quad> shapeQuadDict;

        public void Add2QuadTree(Shape2D shape2d) 
        {
            if (null == shapeQuadDict) shapeQuadDict = new Dictionary<Shape2D, Quad>();
            if (shapeQuadDict.ContainsKey(shape2d)) throw new Exception("repeat. plz check.");
            // TODO 插入树中
        }

        public void Rmv4QuadTree(Shape2D shape2d) 
        {
            if (false == shapeQuadDict.ContainsKey(shape2d)) throw new Exception("not found. plz check.");
            shapeQuadDict.Remove(shape2d);
            // TODO 树中移除
        }

        public void QuadTreeDirty(Shape2D shape2d) 
        {
            var quad = GetQuad(shape2d);
            //shapeQuadDict.TryGetValue(shape2d, out Quad quad);
            // TODO 检查，是否出范围。如果出了范围就向上提一层，在查一遍。
        }

        public void CheckPlace(Shape2D resident, Quad quad) 
        {
        }

        public Quad GetQuad(Shape2D shape2d) 
        {
            shapeQuadDict.TryGetValue(shape2d, out var ret);

            return ret;
        }

        public List<Shape2D> GetNeighbor(Shape2D shape2d) 
        {
            List<Shape2D> shapeList = new List<Shape2D>();


            return shapeList;
        }

        /// <summary>
        /// 四叉树
        /// </summary>
        public class Quad : Goblin<PGEngine>
        {
            public Quad parent;

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
            /// [2, 3]
            /// 四叉树，对应的四个位置
            /// </summary>
            public Quad[] quads;

            protected override void OnCreate()
            {
            }

            protected override void OnDestroy()
            {
            }
        }
    }
}
