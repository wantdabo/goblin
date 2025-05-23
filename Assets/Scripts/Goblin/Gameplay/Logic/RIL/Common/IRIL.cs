using Goblin.Common;

namespace Goblin.Gameplay.Logic.RIL.Common
{
    /// <summary>
    /// 渲染指令
    /// </summary>
    public abstract class IRIL
    {
        /// <summary>
        /// 指令
        /// </summary>
        public abstract ushort id { get; }
        /// <summary>
        /// ActorID
        /// </summary>
        public ulong actor { get; private set; }
        /// <summary>
        /// 哈希值
        /// </summary>
        public int hashcode { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="hashcode">哈希值</param>
        public void Ready(ulong actor, int hashcode)
        {
            this.actor = actor;
            this.hashcode = hashcode;
            OnReady();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            OnReset();
            this.actor = 0;
            this.hashcode = 0;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void OnReady();
        /// <summary>
        /// 重置
        /// </summary>
        protected abstract void OnReset();
    }
}
