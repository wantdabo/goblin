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
            var pos = actor.actorBehaivor.info.pos;
            var scale = actor.actorBehaivor.info.scale;

            var radius = scale.X * Fix64.Half;

            colliderPos = pos;
            colliderScale = scale.ToVector3();

            sphere.Position = pos;

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
