using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 状态机, 用于管理实体的状态切换
/// </summary>
public class StateMachine : Behavior<StateMachineInfo>
{
    /// <summary>
    /// 状态持续时长（FP.Zero = 无限，限时状态到期自动切 fallback）
    /// </summary>
    public FP stateduration { get; set; }
    /// <summary>
    /// duration 到期后切换的目标状态
    /// </summary>
    public byte timerfallback { get; set; }

    /// <summary>
    /// 中断状态
    /// </summary>
    public void Break()
    {
        stateduration = FP.Zero;
        ChangeState(STATE_DEFINE.NONE);
    }
        
    /// <summary>
    /// 延迟中断状态
    /// </summary>
    /// <param name="delay">延迟时间</param>
    public void Break(FP delay)
    {
        info.usedelaybreak = true;
        info.delaybreak = delay;
    }

    /// <summary>
    /// 尝试切换状态
    /// </summary>
    /// <param name="state">状态</param>
    /// <returns>YES/NO</returns>
    public bool TryChangeState(byte state)
    {
        if (info.current == state) return true;
        if (false == QueryPassState(state)) return false;

        ChangeState(state);

        return true;
    }
        
    /// <summary>
    /// 切换到指定状态
    /// </summary>
    /// <param name="state">状态</param>
    public void ChangeState(byte state)
    {
        stateduration = FP.Zero;
        ChangeStateCore(state);
    }

    /// <summary>
    /// 切换到限时状态（duration > 0 才启用计时器）
    /// </summary>
    /// <param name="state">状态</param>
    /// <param name="duration">状态持续时长</param>
    /// <param name="fallback">到期后切回的状态</param>
    public void ChangeState(byte state, FP duration, byte fallback = STATE_DEFINE.IDLE)
    {
        ChangeStateCore(state);
        stateduration = duration;
        timerfallback = fallback;
    }

    /// <summary>
    /// 状态切换核心（不操作计时器）
    /// </summary>
    private void ChangeStateCore(byte state)
    {
        info.last = info.current;
        info.current = state;
        info.usedelaybreak = false;
        info.delaybreak = FP.Zero;

        if (false == stage.SeekBehavior(actor, out Facade facade)) return;

        // 离开 CASTING 时清理命名动画槽位
        if (STATE_DEFINE.CASTING == info.last)
        {
            facade.RmvSlotsByType(ANIM_DEFINE.SLOT_TYPE_NAMED);
        }

        if (STATE_DEFINE.CASTING == info.current)
        {
            facade.SetAnimation(STATE_DEFINE.CASTING);
            return;
        }

        facade.SetAnimation(info.current);
    }

    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);

        // 限时状态倒计时（通用，不硬编码任何具体状态）
        if (FP.Zero < stateduration)
        {
            stateduration -= tick;
            if (FP.Zero >= stateduration)
            {
                stateduration = FP.Zero;
                ChangeState(timerfallback);
            }
        }

        if (false == info.usedelaybreak) return;
        info.delaybreak -= tick;
        if (info.delaybreak <= FP.Zero) Break();
    }

    /// <summary>
    /// 查询当前状态是否可以切换到指定状态
    /// </summary>
    /// <param name="state">状态</param>
    /// <returns>YES/NO</returns>
    private bool QueryPassState(byte state)
    {
        if (STATE_DEFINE.NONE == info.current) return true;
        if (STATE_DEFINE.PASSES.TryGetValue(info.current, out var passes) && passes.Contains(state))
        {
            return true;
        }

        return false;
    }
}