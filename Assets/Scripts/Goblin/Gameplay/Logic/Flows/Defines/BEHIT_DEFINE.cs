namespace Goblin.Gameplay.Logic.Flows.Defines
{
    /// <summary>
    /// 受击定义
    /// </summary>
    public class BEHIT_DEFINE
    {
        /// <summary>
        /// 受击运动以自身做参考
        /// </summary>
        public const byte MOTION_SELF = 0;
        /// <summary>
        /// 受击运动以攻击者做参考
        /// </summary>
        public const byte MOTION_ATTACK = 1;
    }
}