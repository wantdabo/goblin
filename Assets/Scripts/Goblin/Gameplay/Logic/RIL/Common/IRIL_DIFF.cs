namespace Goblin.Gameplay.Logic.RIL.Common
{
    /// <summary>
    /// RIL 差值
    /// </summary>
    public abstract class IRIL_DIFF
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
        /// 差值标记
        /// </summary>
        public byte token { get; private set; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="token">差值标记</param>
        public void Ready(ulong actor, byte token)
        {
            this.actor = actor;
            this.token = token;
            OnReady();
        }

        /// <summary>
        /// 重置
        /// </summary>
        public void Reset()
        {
            OnReset();
            this.actor = 0;
            this.token = 0;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void OnReady();
        /// <summary>
        /// 重置
        /// </summary>
        protected abstract void OnReset();
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns>二进制</returns>
        public abstract byte[] Serialize();
    }
}