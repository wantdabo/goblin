using BEPUutilities;
using GoblinFramework.Gameplay.Physics.Shape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Collisions
{
    /// <summary>
    /// Rectangle-Collider，矩形碰撞盒
    /// </summary>
    public class RectangleCollider : ShapeCollider
    {
        public GRect rectangle { get; private set; }

        private GPoint mSize;
        public GPoint size
        {
            get { return mSize; }
            set { mSize = value; SetDirty(); }
        }

        public override GRect CalcAABB()
        {
            return rectangle;
        }

        public override void OnDirty()
        {
            rectangle = new GRect()
            {
                lt = new GPoint { detail = new Vector2(pos.x - size.x / 2, pos.y + size.y / 2) },
                rb = new GPoint { detail = new Vector2(pos.x + size.x / 2, pos.y - size.y / 2) }
            };
        }
    }
}
