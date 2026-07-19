using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL.EVENT;

/// <summary>
/// RIL 事件 - 音效事件
/// </summary>
public class RIL_EVENT_SOUND : IRIL_EVENT
{
    public override ushort id => RIL_DEFINE.EVENT_SOUND;

    /// <summary>
    /// 发出音效的 Actor
    /// </summary>
    public ulong actor { get; set; }
    /// <summary>
    /// 音效配置 ID
    /// </summary>
    public uint soundid { get; set; }
    /// <summary>
    /// 模式, 参考 SoundMode
    /// OneShot=0 / Loop=1 / Stop=2
    /// </summary>
    public byte mode { get; set; }

    protected override void OnReset()
    {
        actor = 0;
        soundid = 0;
        mode = 0;
    }

    protected override void OnClone(IRIL_EVENT clone)
    {
        if (clone is not RIL_EVENT_SOUND e) return;

        e.actor = actor;
        e.soundid = soundid;
        e.mode = mode;
    }
}
