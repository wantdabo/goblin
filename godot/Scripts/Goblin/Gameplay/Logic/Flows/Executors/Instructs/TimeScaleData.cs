using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// POSITION 变化指令数据
    /// </summary>
    [Serializable]
    [MessagePackObject(true)]
    public class TimeScaleData : InstructData
    {
        public override ushort id => INSTR_DEFINE.TIMESCALE;

        public uint timescale = 1000;
    }
}