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
        public QuadTree QuadTree;

        /// <summary>
        /// 碰撞总列表
        /// </summary>
        public List<Shape2D> shape2dList = new List<Shape2D>();

        private Fixed64Vector3 worldSize;
        public Fixed64Vector3 WorldSize { get { return worldSize; } private set { worldSize = value; } }

        protected override void OnCreate()
        {
            base.OnCreate();
            QuadTree = AddComp<QuadTree>();
        }



        public void PLateLoop(int frame)
        {
            foreach (var shape2d in shape2dList) shape2d.Collision();
        }
    }
}
