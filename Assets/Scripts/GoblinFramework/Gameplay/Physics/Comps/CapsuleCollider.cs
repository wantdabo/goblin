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
    public class CapsuleCollider : Collider
    {
        public Fix64 height;
        public Fix64 radius;

        public override void ComputeCPS()
        {
            var pos = Actor.ActorBehavior.Info.pos;
            var size = Actor.ActorBehavior.Info.size;

            pos.Y += size.Y * Fix64.Half;

            colliderPos = pos;
            colliderSize = size;

            capsule.position = colliderPos;

            capsule.Length = size.Y;
            capsule.Radius = size.X * Fix64.Half;
        }

        private Capsule capsule;
        public override Entity GenEntity()
        {
            capsule = new Capsule(Vector3.Zero, 0, 0);
            ComputeCPS();

            return capsule;
        }
    }
}
