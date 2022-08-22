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
            public event Action<Vector3> posChanged;
            private Vector3 mPos;
            public Vector3 pos
            {
                get { return mPos; }
                set
                {
                    mPos = value;
                    posChanged?.Invoke(pos);
                }
            }

            public event Action<Vector3> rotationChanged;
            private Vector3 mRotation = Vector3.forward;
            public Vector3 rotation
            {
                get { return mRotation; }
                set
                {
                    mRotation = value;
                    rotationChanged?.Invoke(rotation);
                }
            }

            public event Action<Vector4> scaleChanged;
            private Vector4 mScale;
            public Vector4 scale
            {
                get { return mScale; }
                set
                {
                    mScale = value;
                    scaleChanged?.Invoke(scale);
                }
            }
        }
        #endregion
    }

    public class ColliderBehavior<T> : ColliderBehavior where T : Collider, new()
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            // 绑定 Behavior 到基类
            Actor.SetBehavior<ColliderBehavior>(this);

            collider = AddComp<T>();
            collider.entity.PositionUpdateMode = BEPUphysics.PositionUpdating.PositionUpdateMode.Continuous;
        }
    }
}
