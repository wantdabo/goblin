namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// 特效定义
    /// </summary>
    public class EFFECT_DEFINE
    {
        /// <summary>
        /// 特效类型, 标准特效
        /// </summary>
        public const byte TYPE_STANDAR = 0;
        /// <summary>
        /// 特效类型, 线条特效 (LineRenderer 连线)
        /// </summary>
        public const byte TYPE_LINE = 1;
        
        /// <summary>
        /// 特效跟随, 跟随 Actor
        /// </summary>
        public const byte FOLLOW_ACTOR = 0;
        /// <summary>
        /// 特效跟随, 跟随模型挂点
        /// </summary>
        public const byte FOLLOW_MOUNT = 1;

        /// <summary>
        /// 特效跟随掩码 NONE 标志位
        /// </summary>
        public const int FOLLOW_NONE = 0;
        /// <summary>
        /// 特效跟随掩码 POSITION 标志位
        /// </summary>
        public const int FOLLOW_POSITION = 1 << 0;
        /// <summary>
        /// 特效跟随掩码 ROTATION 标志位
        /// </summary>
        public const int FOLLOW_ROTATION = 1 << 1;
        /// <summary>
        /// 特效跟随掩码 SCALE 标志位
        /// </summary>
        public const int FOLLOW_SCALE = 1 << 2;
    }
}