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
        public override void ComputeCPS()
        {
            colliderPos = actor.actorBehaivor.info.pos;
            colliderRotation = actor.actorBehaivor.info.rotation;
            colliderScale = actor.actorBehaivor.info.scale.ToVector3() * actor.actorBehaivor.info.scale.W;

            box.Position = colliderPos;
            box.Orientation = Quaternion.Euler(colliderRotation);
            box.Width = colliderScale.X;
            box.Height = colliderScale.Y;
            box.Length = colliderScale.Z;
        }

        private Box box;
        public override Entity GenEntity()
        {
            box = new Box(Vector3.Zero, 0, 0, 0, 1);
            ComputeCPS();

            return box;
        }
    }
}
