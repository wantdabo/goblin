using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;

namespace Goblin.Gameplay.Logic.Flows.Executors.Instructs
{
    /// <summary>
    /// 技能中断指令数据
    /// </summary>
    public class SkillBreakData : InstructData
    {
        public override ushort id => INSTR_DEFINE.SKILLBREAK;
    }
}