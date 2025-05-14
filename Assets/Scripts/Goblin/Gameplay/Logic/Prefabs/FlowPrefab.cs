using System.Collections.Generic;
using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Prefabs.Common;

namespace Goblin.Gameplay.Logic.Prefabs
{
    /// <summary>
    /// 管线预制创建器信息
    /// </summary>
    public struct FlowPrefabInfo : IPrefabInfo
    {
        /// <summary>
        /// 管线的拥有者
        /// </summary>
        public ulong owner { get; set; }
        /// <summary>
        /// 管线的 ID 列表, 用于指向管线数据
        /// </summary>
        public List<uint> pipelines { get; set; }
    }

    /// <summary>
    /// 管线预制创建器
    /// </summary>
    public class FlowPrefab : Prefab<FlowPrefabInfo>
    {
        public override byte type => ACTOR_DEFINE.FLOW;
        
        protected override void OnProcessing(Actor actor, FlowPrefabInfo info)
        {
            var flowinfo = actor.AddBehaviorInfo<FlowInfo>();
            flowinfo.owner = info.owner;
            
            info.pipelines.Clear();
            ObjectCache.Set(flowinfo.pipelines);
            flowinfo.pipelines = info.pipelines;
        }
    }
}