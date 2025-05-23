namespace Goblin.Gameplay.Logic.Flows.Defines
{
    /// <summary>
    /// 管线子弹定义
    /// </summary>
    public class FLOW_BULLET_DEFINE
    {
        /// <summary>
        /// 生成子弹原点
        /// </summary>
        public const byte BORN_ORIGIN_OWNER = 1;
        
        /// <summary>
        /// 生成子弹初始旋转
        /// </summary>
        public const byte BORN_EULER_OWNER = 1;
        
        /// <summary>
        /// 正前方直线运动
        /// </summary>
        public const ushort MOTION_STRAIGHT = 1;
    }
}