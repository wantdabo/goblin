using BEPUutilities;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    public abstract partial class ShapeCollider : PComp
    {
        /// <summary>
        /// 坐标
        /// </summary>
        public GPoint pos { get; private set; }
        /// <summary>
        /// 方向
        /// </summary>
        public GPoint dire { get; private set; }
        /// <summary>
        /// AABB 包围盒
        /// </summary>
        public GRect aabb { get; private set; }

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

        private void DireChanged(Vector3 dire)
        {
            this.dire = new GPoint() { detail = new Vector2(dire.X, dire.Z) };
            SetDirty();
        }

        private void PosChanged(Vector3 pos)
        {
            this.pos = new GPoint() { detail = new Vector2(pos.X, pos.Z) };
            SetDirty();
        }

        /// <summary>
        /// 重新生成
        /// </summary>
        public void SetDirty()
        {
            OnDirty();
            this.aabb = CalcAABB();
        }

        public abstract void OnDirty();
        public abstract GRect CalcAABB();
    }
}
