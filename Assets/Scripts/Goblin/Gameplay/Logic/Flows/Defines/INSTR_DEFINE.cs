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
        /// <summary>
        /// 特效指令
        /// </summary>
        public const ushort EFFECT = 6;
        /// <summary>
        /// 碰撞指令
        /// </summary>
        public const ushort COLLISION = 7;
        /// <summary>
        /// 移除 Actor 指令
        /// </summary>
        public const ushort RMV_ACTOR = 8;
        /// <summary>
        /// 状态变化指令
        /// </summary>
        public const ushort CHANGE_STATE = 9;
        /// <summary>
        /// 火花指令
        /// </summary>
        public const ushort SPARK = 10;
        /// <summary>
        /// 顿帧指令
        /// </summary>
        public const ushort HIT_LAG = 11;
        /// <summary>
        /// 时间缩放指令
        /// </summary>
        public const ushort TIMESCALE = 12;
    }
}