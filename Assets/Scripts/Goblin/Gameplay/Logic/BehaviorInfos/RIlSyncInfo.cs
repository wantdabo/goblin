using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.BehaviorInfos
{
    /// <summary>
    /// RIL/渲染指令信息
    /// </summary>
    public class RIlSyncInfo : BehaviorInfo
    {
        /// <summary>
        /// Stage 中所有的 RIL 字典, 键为实体 ID, 值为该实体的 RIL 列表
        /// </summary>
        public Dictionary<ulong, Dictionary<ushort, IRIL>> rildict { get; set; }

        protected override void OnReady()
        {
            rildict = ObjectCache.Get<Dictionary<ulong, Dictionary<ushort, IRIL>>>();
        }

        protected override void OnReset()
        {
            foreach (var kv in rildict)
            {
                kv.Value.Clear();
                ObjectCache.Set(kv.Value);
            }
            ObjectCache.Set(rildict);
            rildict.Clear();
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<RIlSyncInfo>();
            clone.Ready(id);
            foreach (var kv in rildict)
            {
                var clonekv = ObjectCache.Get<Dictionary<ushort, IRIL>>();
                clone.rildict.Add(kv.Key, clonekv);
                foreach (var kv2 in kv.Value)
                {
                    clonekv.Add(kv2.Key, kv2.Value);
                }
            }
            
            return clone;
        }
    }
}