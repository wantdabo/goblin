using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// RIL/ 渲染指令同步
    /// </summary>
    public class RILSync : Behavior<RIlSyncInfo>
    {
        /// <summary>
        /// 推送渲染指令
        /// </summary>
        /// <param name="id">ActorID</param>
        /// <param name="ril">渲染指令</param>
        public void PushRIL(ulong id, IRIL ril)
        {
            if (false == info.rildict.TryGetValue(id, out var dict))
            {
                dict = ObjectCache.Get<Dictionary<ushort, IRIL>>();
                info.rildict.Add(id, dict);
            }
            
            if (dict.TryGetValue(ril.id, out var oldril))
            {
                if (oldril.Equals(ril)) return;

                dict.Remove(ril.id);
            }
            dict.Add(ril.id, ril);
            
            actor.stage.onril?.Invoke(id, ril);
        }
    }
}