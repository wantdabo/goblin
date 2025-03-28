using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class RIlSyncInfo : BehaviorInfo
    {
        public Dictionary<ulong, Dictionary<ushort, IRIL>> rildict { get; set; }

        protected override void OnReady()
        {
            rildict = ObjectCache.Get<Dictionary<ulong, Dictionary<ushort, IRIL>>>();
        }

        protected override void OnReset()
        {
            rildict.Clear();
            ObjectCache.Set(rildict);
            rildict = null;
        }
    }
}