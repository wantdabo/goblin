using System;

namespace GoblinFramework.Gameplay.Phys
{
    public class ColliderInfo : BehaviorInfo
    {
        public PhysXBinding physXBinding;
        public event Action<Actor> onCollision;
    }

    public class Collider<T> : Behavior<ColliderInfo>
    {
    }
}