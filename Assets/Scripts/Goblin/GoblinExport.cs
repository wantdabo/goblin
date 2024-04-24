using Goblin.Core;

namespace Goblin
{
    /// <summary>
    /// Goblin 对外暴露
    /// </summary>
    public class GoblinExport
    {
        private static Engine engine;

        /// <summary>
        /// 初始化 Goblin 逻辑
        /// </summary>
        public static void Init()
        {
            engine = Engine.CreateEngine();
        }

        /// <summary>
        /// Tick 驱动
        /// </summary>
        /// <param name="tick">变值步长</param>
        public static void Tick(float tick)
        {
            engine.ticker.Tick(tick);
        }

        /// <summary>
        /// FixedTick 驱动
        /// </summary>
        /// <param name="fixedTick">固定步长</param>
        public static void FixedTick(float fixedTick)
        {
            engine.ticker.FixedTick(fixedTick);
        }
    }
}