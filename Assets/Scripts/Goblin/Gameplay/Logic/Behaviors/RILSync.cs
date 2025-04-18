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
        /// <param name="actor">ActorID</param>
        /// <param name="ril">渲染指令</param>
        public void Push(ulong actor, IRIL ril)
        {
            if (false == info.rildict.TryGetValue(actor, out var dict))
            {
                dict = ObjectCache.Get<Dictionary<ushort, IRIL>>();
                info.rildict.Add(actor, dict);
            }
            
            // 如果已经存在, 则不推送
            if (dict.TryGetValue(ril.id, out var oldril))
            {
                if (oldril.Equals(ril)) return;

                dict.Remove(ril.id);
            }
            dict.Add(ril.id, ril);
            
            // 产生渲染状态
            stage.onril?.Invoke(new()
            {
                index = GenRILIndex(actor, ril.id),
                frame = stage.frame,
                actor = actor,
                ril = ril
            });
        }
        
        /// <summary>
        /// 生成渲染指令序号
        /// </summary>
        /// <param name="actor">ActorID</param>
        /// <param name="ril">RIL ID</param>
        /// <returns>渲染指令序号</returns>
        private uint GenRILIndex(ulong actor, ushort ril)
        {
            if (info.rilindexs.TryGetValue((actor, ril), out var index)) info.rilindexs.Remove((actor, ril));
            index++;
            info.rilindexs.Add((actor, ril), index);

            return index;
        }
        
        /// <summary>
        /// 清除渲染指令 (会导致重新全推到渲染层)
        /// </summary>
        public void Clear()
        {
            info.Reset();
            info.Ready(id);
        }
    }
}