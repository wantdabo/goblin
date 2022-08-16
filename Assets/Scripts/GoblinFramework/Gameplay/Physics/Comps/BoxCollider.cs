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
        public Vector3 size { get; set; }

        public override Entity GenEntity()
        {
            return new Box(Actor.ActorBehavior.Info.pos, size.X, size.Y, size.Z);
        }
    }
}
