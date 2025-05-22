namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// RIL 定义
    /// </summary>
    public class RIL_DEFINE
    {
        /// <summary>
        /// RIL 删除
        /// </summary>
        public const byte DIFF_DEL = 0;
        /// <summary>
        /// RIL 增加
        /// </summary>
        public const byte DIFF_NEW = 1;
        
        /// <summary>
        /// STAGE 场景指令
        /// </summary>
        public const ushort STAGE = 0;
        /// <summary>
        /// TICKER 驱动指令
        /// </summary>
        public const ushort TICKER = 1;
        /// <summary>
        /// SEAT 座位指令
        /// </summary>
        public const ushort SEAT = 2;
        /// <summary>
        /// TAG 标签指令
        /// </summary>
        public const ushort TAG = 3;
        /// <summary>
        /// SPATIAL 指令
        /// </summary>
        public const ushort SPATIAL = 4;
        /// <summary>
        /// STATE 状态机指令
        /// </summary>
        public const ushort STATE_MACHINE = 5;
        /// <summary>
        /// ATTRIBUTE 属性指令
        /// </summary>
        public const ushort ATTRIBUTE = 6;
        /// <summary>
        /// ACTOR 单位指令
        /// </summary>
        public const ushort ACTOR = 7;
    }
}
