namespace Goblin.Gameplay.Logic.Flows.Defines
{
    /// <summary>
    /// 管线定义
    /// </summary>
    public class FLOW_DEFINE
    {
        /// <summary>
        /// 管线长度最大值
        /// </summary>
        public const ulong MAX_LENGTH = ulong.MaxValue / 2;
        /// <summary>
        /// 管线长度最大值 - 溢出
        /// </summary>
        public const ulong OVERFLOW_LENGTH = ulong.MaxValue;

        /// <summary>
        /// 执行目标 - 管线
        /// </summary>
        public const byte ET_FLOW = 1;
        /// <summary>
        /// 执行目标 - 管线拥有者
        /// </summary>
        public const byte ET_FLOW_OWNER = 2;
        /// <summary>
        /// 执行目标 - 管线命中
        /// </summary>
        public const byte ET_FLOW_HIT = 3;
        
        /// <summary>
        /// 脚本 ID 100000001
        /// </summary>
        public const uint S100000001 = 100000001;
        /// <summary>
        /// 脚本 ID 100000002
        /// </summary>
        public const uint S100000002 = 100000002;
    }
}