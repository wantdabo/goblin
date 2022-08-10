using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics.Collisions;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics
{
    public class World : PComp, IPLateLoop
    {
        /// <summary>
        /// 四叉树
        /// </summary>
        public QuadTree QuadTree { get; private set; }

        /// <summary>
        /// 碰撞总列表
        /// </summary>
        public List<Shape2D> shape2dList = new List<Shape2D>();

        private int bounds = 32;
        public int Bounds
        {
            get { return bounds; }
            set
            {
                if (0 == bounds || 0 != (bounds & bounds - 1))
                    throw new Exception("boundX need 2 power number. and not (0) zero");

                bounds = value;
            }
        }

        protected override void OnCreate()
        {
            base.OnCreate();
            QuadTree = AddComp<QuadTree>((tree) => tree.World = this);
        }

        public void PLateLoop(int frame)
        {
            //foreach (var shape2d in shape2dList) shape2d.Collision();
        }
    }
}
