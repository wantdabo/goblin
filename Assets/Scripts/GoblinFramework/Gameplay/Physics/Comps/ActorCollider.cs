using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Comps
{
    public class ActorCollider<T> : PComp where T : Collider, new()
    {
        public T collider;

        protected override void OnCreate()
        {
            base.OnCreate();
            collider = Actor.AddComp<T>();
            collider.entity.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
        }

        public bool IsGround
        {
            get
            {
                Ray ray = new Ray(collider.entity.position, Vector3.Down);
                if (false == Engine.World.Space.RayCast(ray, 1 * Fix64.EN3, out var result)) return false;
                if (0 == result.HitObject.BroadPhase.Overlaps.Count) return false;

                foreach (var item in result.HitObject.BroadPhase.Overlaps)
                {
                    var coll = item.entryA as EntityCollidable;
                    if (null != coll) return true;
                }

                return false;
            }
        }
    }
}
