using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.BuildDatas;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL.EVENT;

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

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
        if (false == stage.SeekBehaviors(out List<Tag> tags)) return 0;
        int count = 0;
        foreach (var tag in tags)
        {
            if (tag.actor == stage.sa) continue;
            if (false == tag.Get(TAG_DEFINE.ACTOR_TYPE, out long t)) continue;
            if (t == type) count++;
        }
        return count;
    }

    private void Finish(bool win)
    {
        finished = true;
        var e = ObjectCache.Ensure<RIL_EVENT_STAGE_RESULT>();
        e.win = win;
        stage.rilsync.Send(e);
    }
}
