namespace Goblin.Gameplay.Logic.Translation.Common
{
    /// <summary>
    /// 渲染指令接口
    /// </summary>
    public interface IRIL
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
    
    /// <summary>
    /// RIL 定义
    /// </summary>
    public class RIL
    {
        /// <summary>
        /// SPATIAL 平移指令
        /// </summary>
        public const ushort SPATIAL_POSITION = 1;
        /// <summary>
        /// SPATIAL 旋转指令
        /// </summary>
        public const ushort SPATIAL_ROTATION = 2;
        /// <summary>
        /// SPATIAL 缩放指令
        /// </summary>
        public const ushort SPATIAL_SCALE = 3;
    }
}
