using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 释放技能指令数据
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class LaunchSkillData : InstructData
{
    public override ushort id => INSTR_DEFINE.LAUNCH_SKILL;

    /// <summary>
    /// 中断释放中技能
    /// </summary>
    public bool breakcasting;
        
    /// <summary>
    /// 技能 ID
    /// </summary>
    public uint skillid;

    public LaunchSkillData()
    {
        et = FLOW_DEFINE.ET_FLOW_OWNER;
    }
}