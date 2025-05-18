using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    public class TestInstr : InstructData
    {
        public override ushort id => INSTR_DEFINE.TEST;
        
        public int hero { get; set; }
    }
}