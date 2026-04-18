using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// 火花指令数据
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class SparkData : InstructData
{
    public override ushort id => INSTR_DEFINE.SPARK;

    /// <summary>
    /// 火花触发范围
    /// </summary>
    public sbyte influence = SPARK_INSTR_DEFINE.FLOW;
        
    /// <summary>
    /// 火花令牌
    /// </summary>
    public string token;
}