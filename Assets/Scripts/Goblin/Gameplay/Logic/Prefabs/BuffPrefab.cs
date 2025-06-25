using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Prefabs.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Prefabs
{
    public struct BuffPrefabInfo : IPrefabInfo
    {
        public uint buffid { get; set; }
        public uint layer { get; set; }
        public FP lifetime { get; set; }
        public ulong owner { get; set; }
        public List<uint> pipelines { get; set; }
    }

    public class BuffPrefab : Prefab<BuffPrefabInfo>
    {
        public override byte type => ACTOR_DEFINE.BUFF;
        
        protected override void OnProcessing(ulong actor, BuffPrefabInfo info)
        {
            var buff = stage.AddBehaviorInfo<BuffInfo>(actor);
            buff.buffid = info.buffid;
            buff.layer = info.layer;
            buff.lifetime = info.lifetime;
            buff.owner = info.owner;
            
            if (null == info.pipelines) return;
            var pipelines = ObjectCache.Ensure<List<uint>>();
            pipelines.AddRange(info.pipelines);
            buff.flow = stage.flow.GenPipeline(actor, pipelines);
        }
    }
}