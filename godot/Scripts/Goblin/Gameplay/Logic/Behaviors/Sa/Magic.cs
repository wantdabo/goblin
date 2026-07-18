using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

/// <summary>
/// 魔法体行为（Sa 级，统一驱动所有 Magic Actor 的管线结束检查）
/// </summary>
public class Magic : Behavior
{
    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        if (false == stage.SeekBehaviorInfos(out List<MagicInfo> magics)) return;
        foreach (var magic in magics) Execute(magic);
    }

    private void Execute(MagicInfo magic)
    {
        if (stage.SeekBehaviorInfo(magic.flow, out FlowInfo flowinfo) && flowinfo.active) return;
        stage.RmvActor(magic.actor);
    }
}