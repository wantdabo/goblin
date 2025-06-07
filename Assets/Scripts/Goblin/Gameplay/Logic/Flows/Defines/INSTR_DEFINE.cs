namespace Goblin.Gameplay.Logic.Flows.Defines
{
    /// <summary>
    /// 指令定义
    /// </summary>
    public class INSTR_DEFINE
    {
        /// <summary>
        /// 动画指令
        /// </summary>
        public const ushort ANIMATION = 1;
        /// <summary>
        /// POSITION 变化指令
        /// </summary>
        public const ushort SPATIAL_POSITION = 2;
        /// <summary>
        /// 生成子弹指令
        /// </summary>
        public const ushort CREATE_BULLET = 3;
        /// <summary>
        /// 子弹运动指令
        /// </summary>
        public const ushort BULLET_MOTION = 4;
        /// <summary>
        /// 释放技能
        /// </summary>
        public const ushort LAUNCH_SKILL = 5;
    }
}