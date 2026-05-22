using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 伤害结算指令数据
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class DamageData : InstructData
{
    public override ushort id => INSTR_DEFINE.DAMAGE;

    /// <summary>
    /// 伤害强度（配置整数，乘以 int2fp 后作为倍率）
    /// </summary>
    public int strength;

    public DamageData()
    {
        et = FLOW_DEFINE.ET_FLOW_HIT;
    }
}
