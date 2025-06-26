using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Prefabs
{
    /// <summary>
    /// Buff 预制信息
    /// </summary>
    public struct BuffPrefabInfo : IPrefabInfo
    {
        /// <summary>
        /// BuffID
        /// </summary>
        public int buffid { get; set; }
        /// <summary>
        /// Buff 拥有者
        /// </summary>
        public ulong owner { get; set; }
        /// <summary>
        /// Buff 管线列表
        /// </summary>
        public List<uint> pipelines { get; set; }
    }

    /// <summary>
    /// Buff 预制创建器
    /// </summary>
    public class BuffPrefab : Prefab<BuffPrefabInfo>
    {
        public override byte type => ACTOR_DEFINE.BUFF;
        
        protected override void OnProcessing(ulong actor, BuffPrefabInfo info)
        {
            var buff = stage.AddBehaviorInfo<BuffInfo>(actor);
            buff.buffid = info.buffid;
            buff.owner = info.owner;
            
            if (null == info.pipelines) return;
            var pipelines = ObjectCache.Ensure<List<uint>>();
            pipelines.AddRange(info.pipelines);
            buff.flow = stage.flow.GenPipeline(actor, pipelines);
        }
    }
}