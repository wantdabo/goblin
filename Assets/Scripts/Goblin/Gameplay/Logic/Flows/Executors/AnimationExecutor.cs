using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 动画指令执行器
    /// </summary>
    public class AnimationExecutor : Executor<AnimationData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, AnimationData data, FlowInfo flowinfo, ulong target)
        {
            base.OnEnter(identity, data, flowinfo, target);
            if (false == stage.SeekBehavior(target, out Facade facade)) return;
            facade.SetAnimation(data.name);
        }

        protected override void OnExit((uint pipelineid, uint index) identity, AnimationData data, FlowInfo flowinfo, ulong target)
        {
            base.OnExit(identity, data, flowinfo, target);
            if (false == stage.SeekBehavior(target, out Facade facade)) return;
            facade.SetAnimation(null);
        }
    }
}