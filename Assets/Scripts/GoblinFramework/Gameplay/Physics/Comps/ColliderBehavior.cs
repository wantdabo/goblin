using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUutilities;
using FixMath.NET;
using GoblinFramework.Gameplay.Behaviors;
using GoblinFramework.Gameplay.Common;
using GoblinFramework.Gameplay.Physics.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoblinFramework.Gameplay.Physics.Comps
{
    public class ColliderBehavior : Behavior<ColliderBehavior.ColliderInfo>
    {
        public Collider collider;

        public bool IsOnGround
        {
            get
            {
                var pos = collider.colliderPos;
                pos.Y -= collider.colliderScale.Y * Fix64.Half + 1 * Fix64.EN3;
                Ray ray = new Ray(pos, Vector3.Down);
                if (false == Engine.World.Space.RayCast(ray, 1 * Fix64.EN4, out var result)) return false;
                if (0 == result.HitObject.BroadPhase.Overlaps.Count) return false;

                foreach (var item in result.HitObject.BroadPhase.Overlaps)
                {
                    var coll = item.entryA as EntityCollidable;
                    if (null != coll && coll.Entity != collider.entity) return true;
                }

                return false;
            }
        }

        #region ColliderInfo
        public class ColliderInfo : BehaviorInfo
        {

        }
        #endregion
    }

    public class ColliderBehavior<T> : ColliderBehavior where T : Collider, new()
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            // 绑定 Behavior 到基类
            actor.BindingBehavior<ColliderBehavior>(this);

            collider = AddComp<T>();
            collider.entity.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            actor.UnBindingBehavior<ColliderBehavior>();
        }
    }
}
