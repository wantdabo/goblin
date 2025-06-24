using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Common.Defines
{
    /// <summary>
    /// Game 设定
    /// </summary>
    public class GAME_DEFINE
    {
        /// <summary>
        /// 逻辑帧率
        /// </summary>
        public static byte LOGIC_FRAME { get; private set; } = 25;
        /// <summary>
        /// 逻辑帧 TICK
        /// </summary>
        public static FP LOGIC_TICK { get; private set; } = FP.One / LOGIC_FRAME;
        /// <summary>
        /// 逻辑帧 TICK (毫秒)
        /// </summary>
        public static uint LOGIC_TICK_MS { get; private set; } = (uint)(1000 / LOGIC_FRAME);
        /// <summary>
        /// 最大渲染帧 TICK (秒)
        /// </summary>
        public static float MAX_TICK { get; private set; } = LOGIC_TICK.AsFloat() * 1.5f;
        /// <summary>
        /// 重力
        /// </summary>
        public static FPVector3 GRAVITY { get; private set; } = FPVector3.down * 981 * FP.EN2;
    }
}
