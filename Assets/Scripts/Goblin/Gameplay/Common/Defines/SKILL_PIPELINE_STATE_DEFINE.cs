namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// 技能管线状态定义
    /// </summary>
    public class SKILL_PIPELINE_STATE_DEFINE
    {
        /// <summary>
        /// 闲置
        /// </summary>
        public const byte NONE = 0;
        /// <summary>
        /// 打断
        /// </summary>
        public const byte BREAK = 1;
        /// <summary>
        /// 开始
        /// </summary>
        public const byte START = 2;
        /// <summary>
        /// 释放中
        /// </summary>
        public const byte CASTING = 3;
        /// <summary>
        /// 结束
        /// </summary>
        public const byte END = 4;
    }
}
