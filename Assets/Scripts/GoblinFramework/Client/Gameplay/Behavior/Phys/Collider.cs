namespace GoblinFramework.Client.Gameplay.Phys
{
    public class ColliderInfo<E> : BehaviorInfo where E : UnityEngine.Collider2D
    {
        public E u3dcollider;
    }

    public class Collider<T, E> : Behavior<T> where T : ColliderInfo<E>, new() where E : UnityEngine.Collider2D
    {
    }
}