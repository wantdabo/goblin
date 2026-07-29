using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.BuildDatas;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 关卡序列，解析 StageSequenceData，监控胜负条件并派发结果事件
/// </summary>
public class StageSequence : Behavior
{
    private StageSequenceData sequencedata { get; set; }
    private bool finished { get; set; }

    public void Initialize(StageSequenceData data)
    {
        sequencedata = data;
        finished = false;
    }

    protected override void OnEndTick()
    {
        base.OnEndTick();
        if (finished) return;

        if (CheckCondition(sequencedata.win)) Finish(true);
        else if (CheckCondition(sequencedata.lose)) Finish(false);
    }

    private bool CheckCondition(StageSequenceCondition condition)
    {
        switch (condition)
        {
            case StageSequenceCondition.AllEnemiesDead:
                return CountActorType(ACTOR_DEFINE.ENEMY) == 0;
            case StageSequenceCondition.HeroDead:
                return CountActorType(ACTOR_DEFINE.HERO) == 0;
            default:
                return false;
        }
    }

    private int CountActorType(long type)
    {
        if (false == stage.SeekBehaviorInfos(out List<TagInfo> infos)) return 0;
        int count = 0;
        foreach (var info in infos)
        {
            if (info.actor == stage.sa) continue;
            var t = stage.tag.Get(info.actor, TAG_DEFINE.ACTOR_TYPE);
            if (t == type) count++;
        }
        return count;
    }

    private void Finish(bool win)
    {
        finished = true;
    }
}
