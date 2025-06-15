using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 外观特效执行器
    /// </summary>
    public class EffectExecutor : Executor<EffectData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, EffectData data, FlowInfo flowinfo)
        {
            base.OnEnter(identity, data, flowinfo);

            var pipeline = PipelineDataReader.Read(identity.pipelineid);
            if (null == pipeline) return;
            if (false == pipeline.Query(identity.index, out var instruct)) return;
            
            if (false == stage.SeekBehavior(flowinfo.owner, out Facade facade)) return;
            facade.CreateEffect(new EffectInfo
            {
                effect = data.effect,
                type = data.type,
                follow = data.follow,
                followmask = data.followmask,
                duration = (int)(instruct.end - instruct.begin) * FP.EN3,
                position = data.position.ToFPVector3(),
                euler = data.euler.ToFPVector3(),
                scale = data.scale * FP.EN3,
            });
        }
    }
}