namespace Goblin.Gameplay.Logic.RIL.Common
{
    /// <summary>
    /// RIL 渲染状态
    /// </summary>
    public abstract class RILState
    {
        /// <summary>
        /// RIL 类型
        /// </summary>
        public byte type { get; set; }
        /// <summary>
        /// ActorID
        /// </summary>
        public ulong actor { get; set; }
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }

        /// <summary>
        /// 重置 RIL 状态
        /// </summary>
        public void Reset()
        {
            type = 0;
            actor = 0;
            frame = 0;
        }
        
        /// <summary>
        /// 重置 RIL 状态
        /// </summary>
        protected abstract void OnReset();
    }
    
    /// <summary>
    /// RIL 渲染状态
    /// </summary>
    /// <typeparam name="T">RIL</typeparam>
    public class RILState<T> : RILState where T : IRIL
    {
        /// <summary>
        /// RIL 渲染指令
        /// </summary>
        public T ril { get; set; }

        protected override void OnReset()
        {
            ril = default;
        }
    }
}