using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 执行释放技能指令的执行器
    /// </summary>
    public class LaunchSkillExecutor : Executor<LaunchSkillData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, LaunchSkillData data, FlowInfo flowinfo, ulong target)
        {
            base.OnEnter(identity, data, flowinfo, target);
            if (false == stage.SeekBehavior(target, out SkillLauncher skilllauncher)) return;
            if (data.breakcasting) skilllauncher.Break();
            skilllauncher.Launch(data.skillid);
        }
    }
}