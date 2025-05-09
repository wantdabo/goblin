using System.Collections.Generic;
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
    public struct PipelinePrefabInfo : IPrefabInfo
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
    public class PipelinePrefab : Prefab<PipelinePrefabInfo>
    {
        public override byte type => ACTOR_DEFINE.PIPELINE;
        
        protected override void OnProcessing(Actor actor, PipelinePrefabInfo info)
        {
            var pipelineinfo = actor.AddBehaviorInfo<PipelineInfo>();
            pipelineinfo.owner = info.owner;
            
            info.pipelines.Clear();
            ObjectCache.Set(pipelineinfo.pipelines);
            pipelineinfo.pipelines = info.pipelines;
        }
    }
}