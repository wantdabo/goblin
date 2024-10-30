namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// SkillAction/技能行为定义
    /// </summary>
    public class SKILL_ACTION_DEFINE
    {
        /// <summary>
        /// 动画
        /// </summary>
        public const ushort ANIMATION = 0;
        /// <summary>
        /// 特效
        /// </summary>
        public const ushort EFFECT = 1;
        /// <summary>
        /// 音效
        /// </summary>
        public const ushort SOUND = 2;
        /// <summary>
        /// 空间变化
        /// </summary>
        public const ushort SPATIAL = 3;
        /// <summary>
        /// 立方体碰撞检测
        /// </summary>
        public const ushort BOX_DETECTION = 4;
        /// <summary>
        /// 球体碰撞检测
        /// </summary>
        public const ushort SPHERE_DETECTION = 5;
        /// <summary>
        /// 圆柱体碰撞检测
        /// </summary>
        public const ushort CYLINDER_DETECTION = 6;
        /// <summary>
        /// 技能子弹事件
        /// </summary>
        public const ushort BULLET_EVENT = 7;
        /// <summary>
        /// 技能打断事件
        /// </summary>
        public const ushort BREAK_EVENT = 8;
        /// <summary>
        /// 技能跳帧事件
        /// </summary>
        public const ushort BREAK_FRAMES_EVENT = 9;
    }
}
