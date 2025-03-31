using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    public class TagInfo : IBehaviorInfo
    {
        public Dictionary<ushort, int> tags { get; set; }

        public void Ready()
        {
            tags = ObjectCache.Get<Dictionary<ushort, int>>();
        }

        public void Reset()
        {
            tags.Clear();
            ObjectCache.Set(tags);
        }
    }
}