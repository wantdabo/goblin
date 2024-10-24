namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// SkillAction/技能行为定义
    /// </summary>
    public class SkillActionDef
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
        /// 空间变化
        /// </summary>
        public const ushort SPATIAL = 2;
        /// <summary>
        /// 立方体碰撞检测
        /// </summary>
        public const ushort BOX_DETECTION = 3;
        /// <summary>
        /// 球体碰撞检测
        /// </summary>
        public const ushort SPHERE_DETECTION = 4;
        /// <summary>
        /// 圆柱体碰撞检测
        /// </summary>
        public const ushort CYLINDER_DETECTION = 5;
        /// <summary>
        /// 打断事件
        /// </summary>
        public const ushort SKILL_BREAK_EVENT = 6;
    }
}
