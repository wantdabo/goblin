namespace Goblin.Gameplay.Common.Translations.Common
{
    /// <summary>
    /// RIL 定义
    /// </summary>
    public class RILDef
    {
        /// <summary>
        /// LIVE 出生指令
        /// </summary>
        public const ushort LIVE_BORN = 0;
        /// <summary>
        /// LIVE 死亡指令
        /// </summary>
        public const ushort LIVE_DEAD = 1;
        /// <summary>
        /// SPATIAL 平移指令
        /// </summary>
        public const ushort SPATIAL_POSITION = 2;
        /// <summary>
        /// SPATIAL 旋转指令
        /// </summary>
        public const ushort SPATIAL_ROTATION = 3;
        /// <summary>
        /// SPATIAL 缩放指令
        /// </summary>
        public const ushort SPATIAL_SCALE = 4;
        /// <summary>
        /// STATEMACHINE ZERO 层状态机指令
        /// </summary>
        public const ushort STATEMACHINE_ZERO = 5;
        /// <summary>
        /// STATEMACHINE ONE 层状态机指令
        /// </summary>
        public const ushort STATEMACHINE_ONE = 6;
        /// <summary>
        /// ATTR SURFACE 指令
        /// </summary>
        public const ushort ATTR_SURFACE = 7;
    }
}
