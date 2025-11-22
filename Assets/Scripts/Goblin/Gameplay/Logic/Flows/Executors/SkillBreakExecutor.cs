using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 技能中断执行器
    /// </summary>
    public class SkillBreakExecutor : Executor<SkillBreakData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, SkillBreakData data, FlowInfo flowinfo, ulong target)
        {
            base.OnEnter(identity, data, flowinfo, target);
            if (false == stage.SeekBehavior(target, out SkillLauncher skilllauncher)) return;
            skilllauncher.Break();
        }
    }
}