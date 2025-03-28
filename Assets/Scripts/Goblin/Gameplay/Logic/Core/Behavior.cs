using Goblin.Gameplay.Logic.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Core
{
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
        private T minfo { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public T info
        {
            get
            {
                if (null != minfo) return minfo;
                
                minfo = actor.stage.GetBehaviorInfo<T>(actor.id);
                if (null == minfo)
                {
                    minfo = actor.stage.AddBehaviorInfo<T>(actor.id);
                }

                return minfo;
            }
        }
    }
}