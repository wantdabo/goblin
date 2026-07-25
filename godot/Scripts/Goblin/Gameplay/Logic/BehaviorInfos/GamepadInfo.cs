using System.Collections.Generic;
using Goblin.Gameplay.Logic.Commands.Input;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 输入中枢信息 — 按类型分槽存储本帧指令，OnEndTick 清空
/// </summary>
public partial class GamepadInfo : BehaviorInfo
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
        // 容器只清不还，首次 Ensure，复用时 Reset 已 Clear
        if (null == keys) keys = ObjectCache.Ensure<List<KeyFrame>>();
        if (null == skills) skills = ObjectCache.Ensure<List<SkillFrame>>();
    }
}
