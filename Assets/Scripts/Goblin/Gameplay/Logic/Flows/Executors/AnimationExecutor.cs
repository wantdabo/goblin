using Goblin.Gameplay.BehaviorInfos;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 动画指令执行器
    /// </summary>
    public class AnimationExecutor : Executor<AnimationData>
    {
        protected override void OnExecute(InstructData data, FlowInfo flowinfo)
        {
            base.OnExecute(data, flowinfo);
        }
    }
}