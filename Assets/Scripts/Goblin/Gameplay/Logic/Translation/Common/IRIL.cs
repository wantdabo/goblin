namespace Goblin.Gameplay.Logic.Translation.Common
{
    /// <summary>
    /// 渲染指令接口
    /// </summary>
    public partial interface IRIL
    {
        /// <summary>
        /// 指令
        /// </summary>
        public ushort id { get; }
        /// <summary>
        /// 序列化
        /// </summary>
        /// <returns>二进制</returns>
        public byte[] Serialize();
        /// <summary>
        /// 对比指令
        /// </summary>
        /// <param name="other">目标指令</param>
        /// <returns>YES/NO</returns>
        public bool Equals(IRIL other);
    }
}
