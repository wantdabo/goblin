using Goblin.Gameplay.Logic.Commands.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Flows.Defines;

namespace Goblin.Gameplay.Logic.Commands
{
    /// <summary>
    /// GM 输入指令
    /// </summary>
    public class GMCommand : Command
    {
        public override ushort id => INPUT_DEFINE.GM;

        protected override void OnReset()
        {
        }

        protected override void OnClone(Command clone)
        {
        }
    }
}