using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows.Common;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 特效执行器
    /// </summary>
    public class EffectExecutor : Executor<EffectData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, EffectData data, FlowInfo flowinfo, ulong target)
        {
            base.OnEnter(identity, data, flowinfo, target);
            if (false == stage.SeekBehavior(target, out Facade facade)) return;
            
            FP duration = FP.Zero;
            switch (data.durationtype)
            {
                case EFFECT_DEFINE.DURATION_TIMELINE:
                    var pipeline = PipelineDataReader.Read(identity.pipelineid);
                    if (null != pipeline && pipeline.Query(identity.index, out var instruct)) duration = (int)(instruct.end - instruct.begin) * FP.EN3;
                    break;
                case EFFECT_DEFINE.DURATION_CUSTOM:
                    duration = data.duration * FP.EN3;
                    break;
                case EFFECT_DEFINE.DURATION_USECFG:
                    if (stage.cfg.location.EffectInfos.TryGetValue(data.effect, out var effectinfo)) duration = effectinfo.Duration * FP.EN3;
                    break;
            }

            uint effect = facade.CreateEffect(new EffectInfo
            {
                effect = data.effect,
                type = data.type,
                follow = data.follow,
                followmask = data.followmask,
                duration = duration,
                position = data.position.ToFPVector3(),
                euler = data.euler.ToFPVector3(),
                scale = data.scale * FP.EN3,
            });
            
            if (false == data.recywithflow) return;
            if (false == stage.SeekBehaviorInfo(flowinfo.actor, out FlowEffectInfo floweffectinfo)) floweffectinfo = stage.AddBehaviorInfo<FlowEffectInfo>(flowinfo.actor);
            floweffectinfo.effects.Add(effect);
        }
    }
}