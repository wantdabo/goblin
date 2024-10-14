namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// 技能管线状态定义
    /// </summary>
    public class SPStateDef
    {
        /// <summary>
        /// 闲置
        /// </summary>
        public const byte None = 0;
        /// <summary>
        /// 打断
        /// </summary>
        public const byte Break = 1;
        /// <summary>
        /// 开始
        /// </summary>
        public const byte Start = 2;
        /// <summary>
        /// 释放中
        /// </summary>
        public const byte Casting = 3;
        /// <summary>
        /// 结束
        /// </summary>
        public const byte End = 4;
    }
}
