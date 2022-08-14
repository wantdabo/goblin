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
        public Fix64? mass;

        protected override void OnCreate()
        {
            if(null == mass)
                body = new Box(pos, size.X, size.Y, size.Z);
            else
                body = new Box(pos, size.X, size.Y, size.Z, mass.Value);

            base.OnCreate();
        }
    }
}
