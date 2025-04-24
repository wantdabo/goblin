using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// 数据状态
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
        /// 哈希值
        /// </summary>
        public int hashcode { get; set; }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset()
        {
            actor = 0;
            hashcode = 0;
            OnReset();
        }

        /// <summary>
        /// 比较状态是否相等
        /// </summary>
        /// <param name="other">其他状态</param>
        /// <returns>YES/NO</returns>
        public bool Equals(State other)
        {
            if (null == other) return false;
            
            return hashcode == other.hashcode;
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        protected abstract void OnReset();
    }
}