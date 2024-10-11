namespace Goblin.Gameplay.Common.Defines
{
    /// <summary>
    /// Game 定义
    /// </summary>
    public class GameDef
    {
        /// <summary>
        /// 逻辑帧率
        /// </summary>
        public const byte LOGIC_FRAME = 16;
        /// <summary>
        /// 逻辑帧 TICK
        /// </summary>
        public const float LOGIC_TICK = 1f / LOGIC_FRAME;
        /// <summary>
        /// 技能管线数据帧率
        /// </summary>
        public const byte SP_DATA_FRAME = 50;
    }
}
