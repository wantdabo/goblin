using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 顿帧指令数据
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class HitLagData : InstructData
{
    public override ushort id => INSTR_DEFINE.HIT_LAG;

    /// <summary>
    /// 顿帧类型
    /// </summary>
    public byte type = HIT_LAG_DEFINE.TYPE_INSTANCE;
        
    /// <summary>
    /// 顿帧强度
    /// </summary>
    public uint strength;
    public uint strengthmax;

    /// <summary>
    /// 持续时间
    /// </summary>
    public uint duration;
    public uint durationmax;

    /// <summary>
    /// 叠加因子
    /// </summary>
    public uint additivefactor;

    public HitLagData()
    {
        et = FLOW_DEFINE.ET_FLOW_OWNER;
    }
}