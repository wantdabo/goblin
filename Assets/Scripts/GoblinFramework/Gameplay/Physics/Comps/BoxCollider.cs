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
            var pos = Actor.ActorBehavior.Info.pos;
            var rot = Actor.ActorBehavior.Info.rotation;
            var scale = Actor.ActorBehavior.Info.scale;

            colliderPos = pos;
            colliderRotation = rot;
            colliderScale = scale.ToVector3();

            box.Position = pos;
            box.Orientation = Quaternion.Euler(rot);
            box.Width = scale.X;
            box.Height = scale.Y;
            box.Length = scale.Z;
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
