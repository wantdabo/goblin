using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;
using UnityEditorInternal.Profiling.Memory.Experimental;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class TickerInfo : IBehaviorInfo
    {
        /// <summary>
        /// 帧号
        /// </summary>
        public uint frame { get; set; }
        /// <summary>
        /// 流逝时间
        /// </summary>
        public FP elapsed { get; set; }
        /// <summary>
        /// 时间间隔
        /// </summary>
        public FP tick => GAME_DEFINE.LOGIC_TICK * scale;
        /// <summary>
        /// 时间缩放
        /// </summary>
        public FP scale { get; set; } = FP.One;

        public void OnReady()
        {
            OnReset();
        }

        public void OnReset()
        {
            frame = 0;
            elapsed = FP.Zero;
        }
    }
}