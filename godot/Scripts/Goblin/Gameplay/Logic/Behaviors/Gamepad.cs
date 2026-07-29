using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Commands.Input;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 输入中枢（Sa 级）
/// 管理所有 Actor 的输入帧
/// </summary>
public class Gamepad : Behavior
{
    /// <summary>
    /// 压入移动帧
    /// </summary>
    public void PushFrame(ulong actor, MoveFrame frame)
    {
        var info = stage.GetBehaviorInfo<GamepadInfo>(actor, true);
        info.move = frame;
    }

    /// <summary>
    /// 压入技能帧
    /// </summary>
    public void PushSkillFrame(ulong actor, SkillFrame frame)
    {
        var info = stage.GetBehaviorInfo<GamepadInfo>(actor, true);
        info.skills.Add(frame);
    }

    /// <summary>
    /// 压入按键帧
    /// </summary>
    public void PushKeyFrame(ulong actor, KeyFrame frame)
    {
        var info = stage.GetBehaviorInfo<GamepadInfo>(actor, true);
        info.keys.Add(frame);
    }

    protected override void OnEndTick()
    {
        if (false == stage.SeekBehaviorInfos(out List<GamepadInfo> infos, true)) return;
        foreach (var info in infos)
        {
            if (false == info.active) continue;

            if (null != info.move)
            {
                info.move.Reset();
                ObjectCache.Set(info.move);
                info.move = null;
            }
            foreach (var k in info.keys)
            {
                k.Reset();
                ObjectCache.Set(k);
            }
            info.keys.Clear();
            foreach (var s in info.skills)
            {
                s.Reset();
                ObjectCache.Set(s);
            }
            info.skills.Clear();
        }
    }
}
