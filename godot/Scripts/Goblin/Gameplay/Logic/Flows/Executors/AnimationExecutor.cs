using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors;

/// <summary>
/// 动画指令执行器
/// </summary>
public class AnimationExecutor : Executor<AnimationData>
{
    protected override void OnEnter((uint pipelineid, uint index) identity, AnimationData data, FlowInfo flowinfo, ulong target)
    {
        base.OnEnter(identity, data, flowinfo, target);
        stage.facade.SetAnimation(target, data.name, ANIM_DEFINE.TICK_MANUAL, data.layer);
    }

    protected override void OnExit((uint pipelineid, uint index) identity, AnimationData data, FlowInfo flowinfo, ulong target)
    {
        base.OnExit(identity, data, flowinfo, target);
        stage.facade.SetAnimation(target, null, ANIM_DEFINE.TICK_AUTOMATIC);
    }

    protected override void OnExecute((uint pipelineid, uint index) identity, AnimationData data, FlowInfo flowinfo, ulong target)
    {
        base.OnExecute(identity, data, flowinfo, target);
        if (stage.SeekBehaviorInfo(target, out FacadeInfo facadeinfo))
            facadeinfo.animelapsed += GAME_DEFINE.LOGIC_TICK;
    }
}
