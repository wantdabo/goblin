using Goblin.Core;
using Goblin.Gameplay.Logic.Translation.Common;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Common
{
    /// <summary>
    /// RIL/ 渲染指令同步
    /// </summary>
    public class RILSync : Comp
    {
        private Dictionary<uint, Dictionary<ushort, IRIL>> rildict = new();

        public void SetRIL(uint actorId, IRIL ril)
        {
            if (false == rildict.TryGetValue(actorId, out var dict)) rildict[actorId] = dict = new Dictionary<ushort, IRIL>();
            if (dict.TryGetValue(ril.id, out var oldril))
            {
                if (oldril.Equals(ril)) return;
                
                dict.Remove(ril.id);
            }

            dict.Add(ril.id, ril);
        }
    }
}
