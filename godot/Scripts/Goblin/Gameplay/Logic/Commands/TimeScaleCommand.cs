using Goblin.Gameplay.Logic.Commands.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;

namespace Goblin.Gameplay.Logic.Commands
{
    /// <summary>
    /// 时间缩放输入指令
    /// </summary>
    public class TimeScaleCommand : Command
    {
        public override ushort id => INPUT_DEFINE.TIMESCALE;

        /// <summary>
        /// 时间缩放值
        /// </summary>
        public int timescale { get; set; }

        protected override void OnReset()
        {
            timescale = 1;
        }

        protected override void OnClone(Command clone)
        {
            if (clone is not TimeScaleCommand command) return;

            command.timescale = timescale;
        }
    }
}