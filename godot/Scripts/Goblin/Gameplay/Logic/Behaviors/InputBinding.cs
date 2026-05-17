using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors;

/// <summary>
/// 输入绑定，把 Gamepad 输入翻译成移动意图和技能触发
/// </summary>
public class InputBinding : Behavior
{
    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        if (false == stage.SeekBehaviors(out List<SkillLauncher> launchers)) return;

        foreach (var launcher in launchers)
        {
            if (false == stage.SeekBehavior(launcher.actor, out Gamepad gamepad)) continue;

            // 移动意图
            if (stage.SeekBehaviorInfo(launcher.actor, out MovementInfo movement))
            {
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

            // 技能触发
            foreach (var skill in launcher.info.loadedskills)
            {
                if (false == launcher.info.loadedskilldict.TryGetValue(skill, out var skillinfo)) continue;
                if (skillinfo.key == 0) continue;
                if (false == gamepad.GetInput(skillinfo.key).press) continue;

                launcher.Launch(skill);
            }
        }
    }
}
