using Goblin.Gameplay.Logic.Commands.Common;

namespace Goblin.Gameplay.Logic.Commands.Soliders
{
    /// <summary>
    /// 时间缩放指令处理
    /// </summary>
    public class TimeScaleSolider : Solider<TimeScaleCommand>
    {
        protected override void OnExecute(TimeScaleCommand command)
        {
            stage.timescale = command.timescale * stage.cfg.int2fp;
        }
    }
}