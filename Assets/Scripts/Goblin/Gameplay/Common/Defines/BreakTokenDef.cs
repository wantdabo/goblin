namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// 打断标记定义
    /// </summary>
    public class BreakTokenDef
    {
        /// <summary>
        /// 无标记
        /// </summary>
        public const int NONE = 0;
        /// <summary>
        /// 摇杆打断标记
        /// </summary>
        public const int JOYSTICK = 1;
        /// <summary>
        /// 受击打断标记
        /// </summary>
        public const int RECV_HURT = 2;
        /// <summary>
        /// 受控打断标记
        /// </summary>
        public const int RECV_CONTROL = 4;
        /// <summary>
        /// 技能释放打断标记
        /// </summary>
        public const int SKILL_CAST = 8;
    }
}
