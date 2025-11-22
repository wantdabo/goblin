namespace Goblin.Gameplay.Logic.Flows.Defines
{
    /// <summary>
    /// 受击定义
    /// </summary>
    public class BEHIT_DEFINE
    {
        /// <summary>
        /// 受击运动以自身前方做参考
        /// </summary>
        public const byte MOTION_SELF_FORWARD = 0;
        /// <summary>
        /// 受击运动以攻击者前方做参考
        /// </summary>
        public const byte MOTION_ATTACK_FORWARD = 1;
        /// <summary>
        /// 受击运动以自身指向攻击者方向做参考
        /// </summary>
        public const byte MOTION_ATTACKER_TO_SELF = 2;
        /// <summary>
        /// 受击运动以攻击者指向自身方向做参考
        /// </summary>
        public const byte MOTION_SELF_TO_ATTACKER = 3;
    }
}