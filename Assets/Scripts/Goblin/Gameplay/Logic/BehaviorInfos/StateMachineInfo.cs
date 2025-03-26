using System.Collections.Generic;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class StateMachineInfo : BehaviorInfo
    {
        public List<byte> states { get; private set; }
        public byte current { get; set; } = STATE_DEFINE.IDLE;
        public uint frames { get; set; } = 0;
        
        protected override void OnReady()
        {
            states = ObjectCache.Get<List<byte>>();
            current = STATE_DEFINE.IDLE;
            frames = 0;
        }

        protected override void OnReset()
        {
            states.Clear();
            ObjectCache.Set(states);
            states = null;
            current = STATE_DEFINE.IDLE;
            frames = 0;
        }
    }
}