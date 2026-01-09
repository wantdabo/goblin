using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public partial class TestInfo : BehaviorInfo
    {
        [RIL]
        public int testvalue { get; set; }
        [RIL]
        public string name { get; set; }
        [RIL]
        public FP health { get; set; }
        public List<float> scores { get; set; }


        protected override void OnReady()
        {
            testvalue = 0;
            name = string.Empty;
            health = FP.Zero;
            scores = ObjectCache.Ensure<List<float>>();
        }

        protected override void OnReset()
        {
            testvalue = 0;
            name = string.Empty;
            health = FP.Zero;
            scores.Clear();
            ObjectCache.Set(scores);
        }
    }
}
