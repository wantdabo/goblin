using GoblinFramework.Gameplay.Common;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    public abstract class Shape2D : PComp
    {
        public Fixed64Vector3 dire { get; private set; }
        public Fixed64Vector3 pos { get; private set; }
        public GPoint rect { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            Actor.ActorBehavior.Info.direChanged += DireChanged;
            Actor.ActorBehavior.Info.posChanged += PosChanged;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Actor.ActorBehavior.Info.posChanged -= PosChanged;
            Actor.ActorBehavior.Info.posChanged -= PosChanged;
        }

        private void DireChanged(Fixed64Vector3 dire)
        {
            this.dire = dire;
        }

        private void PosChanged(Fixed64Vector3 pos)
        {
            this.pos = pos;
        }

        public virtual void DirtyRect()
        {
            rect = GenRect();
        }

        public abstract GPoint GenRect();

        /// <summary>
        /// Goblin-Point
        /// </summary>
        public struct GPoint
        {
            public Fixed64 x;
            public Fixed64 y;
        }

        /// <summary>
        /// Goblin-Triangle
        /// </summary>
        public struct GTriangle 
        {
            public GPoint p0;
            public GPoint p1;
            public GPoint p2;
        }
    }
}
