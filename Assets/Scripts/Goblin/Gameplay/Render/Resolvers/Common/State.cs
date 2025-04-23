using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Render.Resolvers.Common
{
    /// <summary>
    /// 状态接口
    /// </summary>
    public abstract class State
    {
        /// <summary>
        /// 状态类型
        /// </summary>
        public abstract StateType type { get; }
        /// <summary>
        /// RIL 指令类型
        /// </summary>
        public byte rstype { get; set; }
        /// <summary>
        /// ActorID
        /// </summary>
        public ulong actor { get; set; }
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void Reset()
        {
            rstype = 0;
            actor = 0;
            frame = 0;
            OnReset();
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        protected abstract void OnReset();
    }
}