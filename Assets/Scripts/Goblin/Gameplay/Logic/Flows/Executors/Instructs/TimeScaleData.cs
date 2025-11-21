using System;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using MessagePack;
using Sirenix.OdinInspector;

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

        [LabelText("时间缩放")]
        public uint timescale = 1000;
    }
}