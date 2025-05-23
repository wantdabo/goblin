﻿using Kowtow.Math;

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
        /// 技能管线数据帧率
        /// </summary>
        public static byte SP_DATA_FRAME { get; private set; } = 50;
        /// <summary>
        /// 技能管线数据 TICK
        /// </summary>
        public static float SP_DATA_TICK { get; private set; } = 1f / SP_DATA_FRAME;
        /// <summary>
        /// 重力
        /// </summary>
        public static FPVector3 GRAVITY { get; private set; } = FPVector3.down * 981 * FP.EN2;
    }
}
