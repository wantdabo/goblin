using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// 状态机信息
    /// </summary>
    public class StateMachineInfo : BehaviorInfo
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        public byte current { get; set; }
        /// <summary>
        /// 持续帧数
        /// </summary>
        public uint frames { get; set; }
        
        protected override void OnReady()
        {
            current = STATE_DEFINE.IDLE;
            frames = 0;
        }

        protected override void OnReset()
        {
            current = STATE_DEFINE.IDLE;
            frames = 0;
        }
    }
}