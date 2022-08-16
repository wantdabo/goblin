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
    public class CapsuleCollider : Collider
    {
        public Fix64 height;
        public Fix64 radius;

        public override Entity GenEntity()
        {
            return new Capsule(Actor.ActorBehavior.Info.pos, height, radius);
        }
    }
}
