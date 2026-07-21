using System;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 动画指令数据
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class AnimationData : InstructData
{
    public override ushort id => INSTR_DEFINE.ANIMATION;

    /// <summary>
    /// 动画名称
    /// </summary>
    public string name;
    /// <summary>
    /// 动画层（默认全身）
    /// </summary>
    public byte layer = ANIM_DEFINE.LAYER_FULLBODY;

    public AnimationData()
    {
        et = FLOW_DEFINE.ET_FLOW_OWNER;
    }
}