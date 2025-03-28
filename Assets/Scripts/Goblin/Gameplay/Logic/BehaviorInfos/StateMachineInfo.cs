using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class StateMachineInfo : IBehaviorInfo
    {
        public byte current { get; set; } = STATE_DEFINE.IDLE;
        public uint frames { get; set; } = 0;
        
        public void Ready()
        {
            current = STATE_DEFINE.IDLE;
            frames = 0;
        }

        public void Reset()
        {
            current = STATE_DEFINE.IDLE;
            frames = 0;
        }
    }
}