using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;

namespace Goblin.Gameplay.Logic.Flows.Checkers
{
    /// <summary>
    /// 输入检查器
    /// </summary>
    public class InputChecker : Checker<InputCondition>
    {
        protected override bool OnCheck(InputCondition condition, FlowInfo flowinfo)
        {
            if (false == stage.SeekBehavior(flowinfo.owner, out Gamepad gamepad)) return false;
            var input = gamepad.GetInput(condition.type);
            
            if (condition.press && input.press) return true;
            else if (condition.release && input.release) return true;
            
            return false;
        }
    }
}