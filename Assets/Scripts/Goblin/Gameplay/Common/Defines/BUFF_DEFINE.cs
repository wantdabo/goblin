namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// Buff 定义
    /// </summary>
    public class BUFF_DEFINE
    {
        /// <summary>
        /// 单例 BUFF
        /// </summary>
        public const byte SHARED = 0;
        /// <summary>
        /// 多利 BUFF
        /// </summary>
        public const byte MULTI = 1;
        
        /// <summary>
        /// 失活
        /// </summary>
        public const byte INACTIVE = 0;
        /// <summary>
        /// 激活
        /// </summary>
        public const byte ACTIVE = 1;
        
        /// <summary>
        /// 感电 BUFF
        /// </summary>
        public const uint BUFF_10001 = 10001;
        /// <summary>
        /// 引爆 感电 BUFF
        /// </summary>
        public const uint BUFF_10002 = 10002;
    }
}
