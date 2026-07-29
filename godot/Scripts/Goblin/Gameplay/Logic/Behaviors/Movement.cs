using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 运动系统（Sa 级）
/// 管理所有 Actor 的移动
/// </summary>
public class Movement : Behavior
{
    /// <summary>
    /// 移动
    /// </summary>
    public void Move(ulong actor, FPVector3 dire, FP tick)
    {
        if (false == stage.statemachine.TryChangeState(actor, STATE_DEFINE.MOVE)) return;
        if (false == stage.SeekBehaviorInfo(actor, out MovementInfo info)) return;
        if (false == stage.SeekBehaviorInfo(actor, out SpatialInfo spatial)) return;

        dire.Normalize();
        var speed = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.MOVESPEED);
        var motion = dire * speed * tick;
        spatial.position += motion;

        FP angle = FPMath.Atan2(dire.x, dire.z) * FPMath.Rad2Deg;
        spatial.euler = FPVector3.up * angle;

        info.turnmotion = true;
    }

    protected override void OnTick(FP tick)
    {
        if (false == stage.SeekBehaviorInfos(out List<MovementInfo> infos)) return;
        foreach (var info in infos)
        {
            if (false == info.active) continue;
            var actor = info.actor;

            if (false == stage.SeekBehaviorInfo(actor, out GamepadInfo gamepadinfo)) continue;
            if (null == gamepadinfo.move) continue;

            Move(actor, new FPVector3(gamepadinfo.move.dire.x, 0, gamepadinfo.move.dire.y), tick);
        }
    }

    protected override void OnEndTick()
    {
        if (false == stage.SeekBehaviorInfos(out List<MovementInfo> infos)) return;
        foreach (var info in infos)
        {
            if (false == info.active) continue;
            var actor = info.actor;

            if (false == info.turnmotion)
            {
                stage.statemachine.TryChangeState(actor, STATE_DEFINE.IDLE);
            }
            info.turnmotion = false;
        }
    }
}
