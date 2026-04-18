using System;
using Kowtow.Math;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs;

/// <summary>
/// POSITION 变化指令数据
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class SpatialPositionData : InstructData
{
    public override ushort id => INSTR_DEFINE.SPATIAL_POSITION;

    public byte type;

    public IntVector3 position;
}