using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// 状态
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// 状态类型
        /// </summary>
        public abstract StateType type { get; }
        /// <summary>
        /// ActorID
        /// </summary>
        public ulong actor { get; set; }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset()
        {
            actor = 0;
            OnReset();
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        protected abstract void OnReset();
    }
}