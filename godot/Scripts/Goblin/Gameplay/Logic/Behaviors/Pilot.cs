using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 驾驶员/玩家意图源, 把 Gamepad 输入翻译成 MovementInfo
/// </summary>
public class Pilot : Behavior
{
    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        if (false == stage.SeekBehavior(actor, out Gamepad gamepad)) return;
        if (false == stage.SeekBehaviorInfo(actor, out MovementInfo movement)) return;

        var joystick = gamepad.GetInput(INPUT_DEFINE.JOYSTICK);
        if (joystick.press)
        {
            movement.dire = new FPVector3(joystick.dire.x, 0, joystick.dire.y);
            movement.wantmove = true;
        }
        else
        {
            movement.wantmove = false;
        }
    }
}
