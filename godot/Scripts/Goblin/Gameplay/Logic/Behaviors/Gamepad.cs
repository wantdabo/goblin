using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Commands.Input;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 输入中枢 — 按类型分槽存储本帧输入，OnEndTick Reset 后归还池（per-actor）
/// </summary>
public class Gamepad : Behavior<GamepadInfo>
{
    public void PushFrame(InputFrame frame)
    {
        switch (frame)
        {
            case MoveFrame m: info.move = m; break;
            case KeyFrame k: info.keys.Add(k); break;
            case SkillFrame s: info.skills.Add(s); break;
        }
    }

    public MoveFrame move => info.move;
    public GBLList<KeyFrame> keys => info.keys;
    public GBLList<SkillFrame> skills => info.skills;

    protected override void OnEndTick()
    {
        base.OnEndTick();

        if (null != info.move) { info.move.Reset(); ObjectCache.Set(info.move); info.move = null; }
        foreach (var k in info.keys) { k.Reset(); ObjectCache.Set(k); }
        info.keys.Clear();
        foreach (var s in info.skills) { s.Reset(); ObjectCache.Set(s); }
        info.skills.Clear();
    }
}
