using System.Collections.Generic;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors.StageBehavior
{
    public class RIlSyncInfo : BehaviorInfo
    {
        private Dictionary<ulong, Dictionary<ushort, IRIL>> rildict { get; set; }

        protected override void OnReady()
        {
            OnReset();
        }

        protected override void OnReset()
        {
            rildict = ObjectCache.Get<Dictionary<ulong, Dictionary<ushort, IRIL>>>();
        }
    }
}