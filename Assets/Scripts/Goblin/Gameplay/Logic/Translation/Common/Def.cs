namespace Goblin.Gameplay.Logic.Translation.Common
{
    /// <summary>
    /// RIL 定义
    /// </summary>
    public partial interface IRIL
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
        /// <summary>
        /// STATEMACHINE 状态机指令
        /// </summary>
        public const ushort STATEMACHINE = 4;
    }
}
