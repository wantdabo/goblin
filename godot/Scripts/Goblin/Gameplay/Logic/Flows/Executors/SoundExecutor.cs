using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;

namespace Goblin.Gameplay.Logic.Flows.Executors;

/// <summary>
/// 音效执行器
/// </summary>
public class SoundExecutor : Executor<SoundInstructData>
{
    protected override void OnEnter((uint pipelineid, uint index) identity, SoundInstructData data, FlowInfo flowinfo, ulong target)
    {
        base.OnEnter(identity, data, flowinfo, target);
    }
}
