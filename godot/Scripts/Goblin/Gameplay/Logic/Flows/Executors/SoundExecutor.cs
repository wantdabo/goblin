using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Goblin.Gameplay.Logic.RIL.EVENT;

namespace Goblin.Gameplay.Logic.Flows.Executors;

/// <summary>
/// 音效执行器
/// </summary>
public class SoundExecutor : Executor<SoundInstructData>
{
    protected override void OnEnter((uint pipelineid, uint index) identity, SoundInstructData data, FlowInfo flowinfo, ulong target)
    {
        base.OnEnter(identity, data, flowinfo, target);

        var soundevent = ObjectCache.Ensure<RIL_EVENT_SOUND>();
        soundevent.actor = target;
        soundevent.soundid = data.soundid;
        soundevent.mode = data.mode;
        stage.rilsync.Send(soundevent);
    }
}
