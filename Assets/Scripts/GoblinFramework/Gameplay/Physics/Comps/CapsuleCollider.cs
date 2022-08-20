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
        public override void ComputeCPS()
        {
            var pos = Actor.ActorBehavior.Info.pos;
            var size = Actor.ActorBehavior.Info.scale;

            pos.Y += size.Y * Fix64.Half;

            colliderPos = pos;
            colliderScale = size.ToVector3();

            capsule.Position = colliderPos;

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
