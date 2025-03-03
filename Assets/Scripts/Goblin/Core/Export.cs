﻿namespace Goblin.Core
{
    /// <summary>
    /// Goblin 对外暴露
    /// </summary>
    public class Export
    {
        /// <summary>
        /// 引擎
        /// </summary>
        public static Engine engine { get; private set; }

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
            if (null == engine) return;
            
            engine.ticker.Tick(tick);
        }

        /// <summary>
        /// FixedTick 驱动
        /// </summary>
        /// <param name="tick">固定步长</param>
        public static void FixedTick(float tick)
        {
            if (null == engine) return;

            engine.ticker.FixedTick(tick);
        }
    }
}
