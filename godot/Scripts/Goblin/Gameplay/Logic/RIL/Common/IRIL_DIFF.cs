namespace Goblin.Gameplay.Logic.RIL.Common
{
    /// <summary>
    /// RIL 差值, 不允许存在引用 Field, 请使用纯值类型
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
        /// 克隆
        /// </summary>
        /// <param name="clone">目标</param>
        /// <returns>目标克隆后</returns>
        public IRIL_DIFF Clone(IRIL_DIFF clone)
        {
            clone.Ready(actor, token);
            OnClone(clone);

            return clone;
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
        /// 克隆
        /// </summary>
        /// <param name="clone">目标</param>
        protected abstract void OnClone(IRIL_DIFF clone);
    }
}