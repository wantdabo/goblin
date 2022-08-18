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
    public class CylinderCollider : Collider
    {
        public override void ComputeCPS()
        {
            var pos = Actor.ActorBehavior.Info.pos;
            var size = Actor.ActorBehavior.Info.scale;

            colliderPos = pos;
            colliderSize = size.ToVector3();

            cylinder.position = pos;

            cylinder.Height = size.Y;
            cylinder.Radius = size.Z * Fix64.Half;
        }

        private Cylinder cylinder;
        public override Entity GenEntity()
        {
            cylinder = new Cylinder(Vector3.Zero, 0, 0);
            ComputeCPS();

            return cylinder;
        }
    }
}
