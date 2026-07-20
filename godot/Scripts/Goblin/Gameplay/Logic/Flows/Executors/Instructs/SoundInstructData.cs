using System;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 音效指令数据
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class SoundInstructData : InstructData
{
    public override ushort id => INSTR_DEFINE.SOUND;

    /// <summary>
    /// 音效配置 ID
    /// </summary>
    public uint soundid;

    /// <summary>
    /// 播放模式, 参考 SoundMode
    /// </summary>
    public byte mode = (byte)SoundMode.OneShot;

    public SoundInstructData()
    {
        et = FLOW_DEFINE.ET_FLOW_OWNER;
    }
}
