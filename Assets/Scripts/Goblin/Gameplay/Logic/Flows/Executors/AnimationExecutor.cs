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
        protected override void OnEnter(AnimationData data, FlowInfo flowinfo)
        {
            base.OnEnter(data, flowinfo);
            if (false == stage.SeekBehavior(flowinfo.owner, out Facade facade)) return;
            facade.SetAnimationName(data.name, 0);
        }

        protected override void OnExecute(AnimationData data, FlowInfo flowinfo)
        {
            base.OnExecute(data, flowinfo);
            if (false == stage.SeekBehavior(flowinfo.owner, out Facade facade)) return;
            var animelapsed = facade.info.animelapsed;
            animelapsed += (int)flowinfo.framepass * stage.cfg.int2fp;
            facade.SetAnimationName(data.name, animelapsed);
        }

        protected override void OnExit(AnimationData data, FlowInfo flowinfo)
        {
            base.OnExit(data, flowinfo);
            if (false == stage.SeekBehavior(flowinfo.owner, out Facade facade)) return;
            facade.SetAnimationName(null, 0);
        }
    }
}