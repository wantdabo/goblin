﻿using GoblinFramework.Gameplay.Common;
using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    public abstract partial class Shape2D : PComp
    {
        /// <summary>
        /// 方向
        /// </summary>
        public GPoint dire { get; private set; }
        /// <summary>
        /// 坐标
        /// </summary>
        public GPoint pos { get; private set; }
        /// <summary>
        /// 简单包围盒
        /// </summary>
        public GRect box { get; private set; }

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
            this.dire = new GPoint() { detail = new Fixed64Vector2(dire.x, dire.z) };
            SetDirty();
        }

        private void PosChanged(Fixed64Vector3 pos)
        {
            this.pos = new GPoint() { detail = new Fixed64Vector2(pos.x, pos.z) };
            SetDirty();
        }

        /// <summary>
        /// 重新生成
        /// </summary>
        public void SetDirty()
        {
            OnDirty();
            this.box = ComputeAABB();
        }

        public abstract void OnDirty();
        public abstract GRect ComputeAABB();
    }
}