using System.Collections.Generic;
using Goblin.Gameplay.Logic.Commands.Input;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 输入中枢信息 — 按类型分槽存储本帧指令，OnEndTick 清空
/// </summary>
public class GamepadInfo : BehaviorInfo
{
    /// <summary>
    /// 移动指令（每帧最多一个）
    /// </summary>
    public MoveFrame move { get; set; }
    /// <summary>
    /// 按键指令列表
    /// </summary>
    public List<KeyFrame> keys { get; set; }
    /// <summary>
    /// 技能指令列表
    /// </summary>
    public List<SkillFrame> skills { get; set; }

    protected override void OnReady()
    {
        keys = ObjectCache.Ensure<List<KeyFrame>>();
        skills = ObjectCache.Ensure<List<SkillFrame>>();
    }

    protected override void OnReset()
    {
        move = null;
        keys.Clear();
        ObjectCache.Set(keys);
        skills.Clear();
        ObjectCache.Set(skills);
    }

    protected override BehaviorInfo OnClone()
    {
        var clone = ObjectCache.Ensure<GamepadInfo>();
        clone.Ready(actor);
        return clone;
    }
}
