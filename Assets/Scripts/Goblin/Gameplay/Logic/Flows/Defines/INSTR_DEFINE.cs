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
        /// 生成子弹指令
        /// </summary>
        public const ushort CREATE_BULLET = 2;
        /// <summary>
        /// 子弹运动指令
        /// </summary>
        public const ushort BULLET_MOTION = 3;
    }
}