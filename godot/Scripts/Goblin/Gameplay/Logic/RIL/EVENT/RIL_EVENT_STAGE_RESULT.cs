using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL.EVENT;

/// <summary>
/// RIL 事件 - 关卡结果事件
/// </summary>
public class RIL_EVENT_STAGE_RESULT : IRIL_EVENT
{
    public override ushort id => RIL_DEFINE.EVENT_STAGE_RESULT;

    /// <summary>
    /// 是否胜利
    /// </summary>
    public bool win { get; set; }

    protected override void OnReset()
    {
        win = false;
    }

    protected override void OnClone(IRIL_EVENT clone)
    {
        if (clone is not RIL_EVENT_STAGE_RESULT e) return;
        e.win = win;
    }
}
