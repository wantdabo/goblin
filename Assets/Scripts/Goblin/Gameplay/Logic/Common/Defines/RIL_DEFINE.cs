namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// RIL 定义
    /// </summary>
    public partial class RIL_DEFINE
    {
        /// <summary>
        /// RIL_DIFF 删除
        /// </summary>
        public const byte DIFF_DEL = 0;
        /// <summary>
        /// RIL_DIFF 增加
        /// </summary>
        public const byte DIFF_NEW = 1;

        /// <summary>
        /// RIL_EVENT 伤害
        /// </summary>
        public const ushort EVENT_DAMAGE = 1;
        /// <summary>
        /// RIL_EVENT 治疗
        /// </summary>
        public const ushort EVENT_CURE = 2;

        /// <summary>
        /// LOSS 丢弃渲染指令
        /// </summary>
        public const ushort LOSS = 0;
        /// <summary>
        /// STAGE 场景指令
        /// </summary>
        public const ushort STAGE = 1;
        /// <summary>
        /// TICKER 驱动指令
        /// </summary>
        public const ushort TICKER = 2;
        /// <summary>
        /// SEAT 座位指令
        /// </summary>
        public const ushort SEAT = 3;
        /// <summary>
        /// TAG 标签指令
        /// </summary>
        public const ushort TAG = 4;
        /// <summary>
        /// SPATIAL 指令
        /// </summary>
        public const ushort SPATIAL = 5;
        /// <summary>
        /// STATE 状态机指令
        /// </summary>
        public const ushort STATE_MACHINE = 6;
        /// <summary>
        /// ATTRIBUTE 属性指令
        /// </summary>
        public const ushort ATTRIBUTE = 7;
        /// <summary>
        /// ACTOR 单位指令
        /// </summary>
        public const ushort ACTOR = 8;
        /// <summary>
        /// MOTION 运动指令
        /// </summary>
        public const ushort MOTION = 9;
        /// <summary>
        /// FACADE 外观指令
        /// </summary>
        public const ushort FACADE_MODEL = 10;
        /// <summary>
        /// FACADE_ANIMATION 外观动画指令
        /// </summary>
        public const ushort FACADE_ANIMATION = 11;
        /// <summary>
        /// FACADE_EFFECT 外观特效指令
        /// </summary>
        public const ushort FACADE_EFFECT = 12;
    }
}
