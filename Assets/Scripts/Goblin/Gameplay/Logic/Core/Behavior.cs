using Goblin.Gameplay.Logic.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Core
{
    /// <summary>
    /// 行为信息
    /// </summary>
    public interface IBehaviorInfo
    {
        public void Ready();
        
        public void Reset();
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

        public void Assemble(Actor actor)
        {
            this.actor = actor;
            OnAssemble();
        }

        public void Disassemble()
        {
            OnDisassemble();
            this.actor = null;
        }

        public void Tick(FP tick)
        {
            OnTick(tick);
        }

        public void TickEnd()
        {
            OnTickEnd();
        }

        protected virtual void OnAssemble()
        {
        }

        protected virtual void OnDisassemble()
        {
        }

        protected virtual void OnTick(FP tick)
        {
        }

        protected virtual void OnTickEnd()
        {
        }
    }

    public abstract class Behavior<T> : Behavior where T : IBehaviorInfo, new()
    {
        /// <summary>
        /// 信息
        /// </summary>
        public T info { get; private set; }

        protected override void OnAssemble()
        {
            base.OnAssemble();
            if (false == actor.stage.SeekBehaviorInfo(actor.id, out T temp)) temp = actor.stage.AddBehaviorInfo<T>(actor.id);
            info = temp;
        }

        protected override void OnDisassemble()
        {
            base.OnDisassemble();
            info = default;
        }
    }
}