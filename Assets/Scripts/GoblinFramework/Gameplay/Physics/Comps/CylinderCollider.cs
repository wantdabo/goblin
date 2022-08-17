using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
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
        public Fix64 height;
        public Fix64 radius;

        public override void ComputeCPS()
        {
            throw new NotImplementedException();
        }

        public override Entity GenEntity()
        {
            return new Cylinder(Actor.ActorBehavior.Info.pos, height, radius);
        }
    }
}
