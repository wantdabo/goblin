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
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}