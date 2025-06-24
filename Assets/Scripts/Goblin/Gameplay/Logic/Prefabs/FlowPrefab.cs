using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Flows;
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
        
        protected override void OnProcessing(ulong actor, FlowPrefabInfo info)
        {
            var flowinfo = stage.AddBehaviorInfo<FlowInfo>(actor);
            flowinfo.active = true;
            flowinfo.owner = info.owner;
            flowinfo.pipelines.AddRange(info.pipelines);
            
            // 计算管线的时间长度
            foreach (var pipeline in flowinfo.pipelines)
            {
                var data = PipelineDataReader.Read(pipeline);
                if (data.length > flowinfo.length ) flowinfo.length = data.length;
            }
        }
    }
}