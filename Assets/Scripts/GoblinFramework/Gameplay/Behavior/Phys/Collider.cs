using System;

namespace GoblinFramework.Gameplay.Phys
{
    public class ColliderInfo : BehaviorInfo
    {
        public PhysAssis physAssis;
    }

    public class Collider<T> : Behavior<ColliderInfo>
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            info.physAssis = actor.GetBehavior<PhysAssis>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            info.physAssis = null;
        }
    }
}