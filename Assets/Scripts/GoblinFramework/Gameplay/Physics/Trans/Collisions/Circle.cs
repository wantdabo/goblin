﻿using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    /// <summary>
    /// Circle，圆形碰撞盒
    /// </summary>
    public class Circle : Shape2D
    {
        public GCircle circle { get; private set; }

        private Fixed64 mRadius;
        public Fixed64 radius
        {
            get { return mRadius; }
            set { mRadius = value; SetDirty(); }
        }

        public override GRect ComputeAABB()
        {
            return new GRect
            {
                lt = new GPoint { detail = new Fixed64Vector2(-radius + pos.x, radius + pos.y) },
                rb = new GPoint { detail = new Fixed64Vector2(radius + pos.x, -radius + pos.y) }
            };
        }

        public override void OnDirty()
        {
            circle = new GCircle { center = pos, radius = radius };
        }
    }
}