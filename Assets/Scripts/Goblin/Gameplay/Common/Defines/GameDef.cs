using TrueSync;

namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// Game 设定
    /// </summary>
    public class GameDef
    {
        /// <summary>
        /// 逻辑帧率
        /// </summary>
        public static byte LOGIC_FRAME { get; private set; } = 25;
        /// <summary>
        /// 技能管线数据帧率
        /// </summary>
        public static byte SP_DATA_FRAME { get; private set; } = 50;
        /// <summary>
        /// 逻辑帧 TICK
        /// </summary>
        public static FP LOGIC_TICK { get; private set; } = FP.One / LOGIC_FRAME;
        /// <summary>
        /// 技能管线数据 TICK
        /// </summary>
        public static float SP_DATA_TICK { get; private set; } = 1f / SP_DATA_FRAME;
        /// <summary>
        /// 逻辑帧率对技能管线数据帧率的比例
        /// </summary>
        public static FP LOGIC_SP_DATA_FRAME_SCALE { get; private set; } = (FP)LOGIC_FRAME / (FP)SP_DATA_FRAME;
        /// <summary>
        /// 技能管线数据帧率对逻辑帧率的比例
        /// </summary>
        public static float SP_DATA_LOGIC_FRAME_SCALE { get; private set; } = 1 / LOGIC_SP_DATA_FRAME_SCALE.AsFloat();
    }
}
