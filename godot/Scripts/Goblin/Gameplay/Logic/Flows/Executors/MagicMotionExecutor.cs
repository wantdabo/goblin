using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors;

/// <summary>
/// 魔法体运动执行器
/// </summary>
public class MagicMotionExecutor : Executor<MagicMotionData>
{
    protected override void OnExecute((uint pipelineid, uint index) identity, MagicMotionData data, FlowInfo flowinfo, ulong target)
    {
        base.OnExecute(identity, data, flowinfo, target);
        if (false == stage.SeekBehaviorInfo(target, out SpatialInfo spatial)) return;

        switch (data.motion)
        {
            case FLOW_MAGIC_DEFINE.MOTION_STRAIGHT:
                var rotation = FPQuaternion.Euler(spatial.euler);
                var forward = rotation * FPVector3.forward;
                var speed = data.speed * stage.cfg.int2fp;
                spatial.position += forward * data.speedrate * stage.cfg.int2fp * speed * GAME_DEFINE.LOGIC_TICK;
                break;
        }
    }
}
