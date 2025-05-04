namespace Goblin.Gameplay.Logic.Common.Defines
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
        /// 技能打断标记事件
        /// </summary>
        public const ushort BREAK_TOKEN_EVENT = 6;
        /// <summary>
        /// 技能跳帧事件
        /// </summary>
        public const ushort BREAK_FRAMES_EVENT = 7;
        /// <summary>
        /// 技能 BUFF 触发事件
        /// </summary>
        public const ushort BUFF_TRIGGER_EVENT = 8;
        /// <summary>
        /// 技能 BUFF 印下事件
        /// </summary>
        public const ushort BUFF_STAMP_EVENT = 9;
    }
}
