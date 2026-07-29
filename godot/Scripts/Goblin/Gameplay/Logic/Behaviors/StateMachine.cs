using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 状态机（Sa 级）
/// 管理所有 Actor 的状态切换
/// </summary>
public class StateMachine : Behavior
{
    /// <summary>
    /// 中断状态
    /// </summary>
    public void Break(ulong actor)
    {
        if (false == stage.SeekBehaviorInfo(actor, out StateMachineInfo info)) return;
        info.stateduration = FP.Zero;
        ChangeState(actor, STATE_DEFINE.NONE);
    }

    /// <summary>
    /// 延迟中断状态
    /// </summary>
    public void Break(ulong actor, FP delay)
    {
        if (false == stage.SeekBehaviorInfo(actor, out StateMachineInfo info)) return;
        info.usedelaybreak = true;
        info.delaybreak = delay;
    }

    /// <summary>
    /// 尝试切换状态
    /// </summary>
    public bool TryChangeState(ulong actor, byte state)
    {
        if (false == stage.SeekBehaviorInfo(actor, out StateMachineInfo info)) return false;
        if (info.current == state) return true;
        if (false == QueryPassState(info, state)) return false;

        ChangeState(actor, state);

        return true;
    }

    /// <summary>
    /// 切换到指定状态
    /// </summary>
    public void ChangeState(ulong actor, byte state)
    {
        if (false == stage.SeekBehaviorInfo(actor, out StateMachineInfo info)) return;
        info.stateduration = FP.Zero;
        ChangeStateCore(actor, info, state);
    }

    /// <summary>
    /// 切换到限时状态
    /// </summary>
    public void ChangeState(ulong actor, byte state, FP duration, byte fallback = STATE_DEFINE.IDLE)
    {
        if (false == stage.SeekBehaviorInfo(actor, out StateMachineInfo info)) return;
        ChangeStateCore(actor, info, state);
        info.stateduration = duration;
        info.timerfallback = fallback;
    }

    private void ChangeStateCore(ulong actor, StateMachineInfo info, byte state)
    {
        info.last = info.current;
        info.current = state;
        info.usedelaybreak = false;
        info.delaybreak = FP.Zero;

        // 离开 CASTING 时清理命名动画槽位
        if (STATE_DEFINE.CASTING == info.last)
        {
            stage.facade.RmvSlotsByType(actor, ANIM_DEFINE.SLOT_TYPE_NAMED);
        }

        if (STATE_DEFINE.CASTING == info.current)
        {
            stage.facade.SetAnimation(actor, STATE_DEFINE.CASTING);
            return;
        }

        stage.facade.SetAnimation(actor, info.current);
    }

    protected override void OnTick(FP tick)
    {
        if (false == stage.SeekBehaviorInfos(out List<StateMachineInfo> infos)) return;
        foreach (var info in infos)
        {
            if (false == info.active) continue;
            var actor = info.actor;

            // 限时状态倒计时
            if (FP.Zero < info.stateduration)
            {
                info.stateduration -= tick;
                if (FP.Zero >= info.stateduration)
                {
                    info.stateduration = FP.Zero;
                    ChangeState(actor, info.timerfallback);
                }
            }

            if (false == info.usedelaybreak) continue;
            info.delaybreak -= tick;
            if (info.delaybreak <= FP.Zero) Break(actor);
        }
    }

    private bool QueryPassState(StateMachineInfo info, byte state)
    {
        if (STATE_DEFINE.NONE == info.current) return true;
        if (STATE_DEFINE.PASSES.TryGetValue(info.current, out var passes)
            && passes.Contains(state))
        {
            return true;
        }

        return false;
    }
}
