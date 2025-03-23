namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 行为信息
    /// </summary>
    public abstract class BehaviorInfo
    {
    }

    /// <summary>
    /// 行为
    /// </summary>
    public abstract class Behavior
    {
        /// <summary>
        /// 实体
        /// </summary>
        public Actor actor { get; private set; }

        public virtual void Assembly(Actor actor)
        {
            this.actor = actor;
        }
    }

    public abstract class Behavior<T, E> : Behavior where T : Behavior where E : BehaviorInfo, new()
    {
        /// <summary>
        /// 信息
        /// </summary>
        public E info { get; private set; }

        public override void Assembly(Actor actor)
        {
            base.Assembly(actor);
            var i = actor.stage.GetBehaviorInfo<T>(actor.id);
            if (null == i) info = actor.stage.pool.GetBehaviorInfo<E>();
            actor.stage.AddBehaviorInfo<T>(actor.id, info);
        }
    }
}