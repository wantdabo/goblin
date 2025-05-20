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
        /// 初始化
        /// </summary>
        /// <param name="actor">ActorID</param>
        public void Ready(ulong actor)
        {
            this.actor = actor;
            OnReady();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            OnReset();
            this.actor = 0;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public abstract void OnReady();
        /// <summary>
        /// 重置
        /// </summary>
        public abstract void OnReset();

        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns>二进制</returns>
        public abstract byte[] Serialize();
    }
}
