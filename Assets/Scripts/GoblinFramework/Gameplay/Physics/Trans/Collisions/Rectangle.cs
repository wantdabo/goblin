﻿using Numerics.Fixed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    /// <summary>
    /// Rectangle，矩形碰撞盒
    /// </summary>
    public class Rectangle : Shape2D
    {
        public GRect rectangle { get; private set; }

        private GPoint mSize;
        public GPoint size
        {
            get { return mSize; }
            set { mSize = value; SetDirty(); }
        }

        public override GRect ComputeAABB()
        {
            return rectangle;
        }

        public override void OnDirty()
        {
            rectangle = new GRect()
            {
                lt = new GPoint { detail = new Fixed64Vector2(pos.x - size.x / 2, pos.y + size.y / 2) },
                rb = new GPoint { detail = new Fixed64Vector2(pos.x + size.x / 2, pos.y - size.y / 2) }
            };
        }
    }
}