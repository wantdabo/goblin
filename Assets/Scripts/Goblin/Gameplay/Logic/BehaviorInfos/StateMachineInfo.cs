using System.Collections.Generic;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class StateMachineInfo : BehaviorInfo
    {
        public byte current { get; set; } = STATE_DEFINE.IDLE;
        public uint frames { get; set; } = 0;
        
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