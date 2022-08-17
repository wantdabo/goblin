using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using BEPUutilities;
using FixMath.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Comps
{
    public class BoxCollider : Collider
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void ComputeCPS()
        {
            var pos = Actor.ActorBehavior.Info.pos;
            var size = Actor.ActorBehavior.Info.size;

            pos.Y += size.Y * Fix64.Half;

            colliderPos = pos;
            colliderSize = size;

            box.position = pos;

            box.Width = size.X;
            box.Height = size.Y;
            box.Length = size.Z;
        }

        private Box box;
        public override Entity GenEntity()
        {
            box = new Box(Vector3.Zero, 0, 0, 0);
            ComputeCPS();

            return box;
        }
    }
}
