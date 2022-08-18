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
    public class SphereCollider : Collider
    {
        public override void ComputeCPS()
        {
            var pos = Actor.ActorBehavior.Info.pos;
            var size = Actor.ActorBehavior.Info.size;

            var radius = size.X * Fix64.Half;

            pos.Y += radius;

            colliderPos = pos;
            colliderSize = size;

            sphere.position = pos;

            sphere.Radius = radius;
        }

        private Sphere sphere;
        public override Entity GenEntity()
        {
            sphere = new Sphere(Vector3.zero, 0);
            ComputeCPS();

            return sphere;
        }
    }
}
