using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Commands.Input;
using Goblin.Gameplay.Logic.Flows.Checkers.Common;
using Goblin.Gameplay.Logic.Flows.Checkers.Conditions;

namespace Goblin.Gameplay.Logic.Flows.Checkers;

/// <summary>
/// 输入检查器 — 在 Flow 管线中检查输入指令
/// </summary>
public class InputChecker : Checker<InputCondition>
{
    protected override bool OnCheck(InputCondition condition, FlowInfo flowinfo, ulong target)
    {
        if (false == stage.SeekBehavior(target, out Gamepad gamepad)) return false;

        foreach (var key in gamepad.keys)
        {
            if (key.key == condition.type)
            {
                if (condition.press && key.action == KeyAction.Press) return true;
                if (condition.release && key.action == KeyAction.Release) return true;
            }
        }

        return false;
    }
}
