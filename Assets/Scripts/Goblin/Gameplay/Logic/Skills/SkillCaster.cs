using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Inputs;
using Goblin.Gameplay.Logic.Spatials;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills
{
    /// <summary>
    /// 技能释放解析
    /// </summary>
    public class SkillCaster : Comp
    {
        public SkillLauncher launcher { get; set; }
        private Spatial spatial { get; set; }
        private Gamepad gamepad { get; set; }

        // TODO 后续改为读取配置
        private Dictionary<uint, uint> skillcomboDict = new()
        {
            { 10001, 10002 },
            { 10002, 10003 },
            { 10003, 10004 },
            { 10004, 10001 },
        };

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = launcher.actor.GetBehavior<Spatial>();
            gamepad = launcher.actor.GetBehavior<Gamepad>();
        }

        private void JoystickForawrd()
        {
            var joystick = gamepad.GetInput(InputType.Joystick);
            if (joystick.press)
            {
                if (TSVector2.zero != joystick.dire)
                {
                    FP angle = TSMath.Atan2(joystick.dire.x, joystick.dire.y) * TSMath.Rad2Deg;
                    spatial.eulerAngle = TSVector.up * angle;
                }
            }
        }

        public void OnFPTick(FP tick)
        {
            // TODO 后面改为配置
            if (null == gamepad) return;
            var ba = gamepad.GetInput(InputType.BA);
            var bb = gamepad.GetInput(InputType.BB);
            var bc = gamepad.GetInput(InputType.BC);
            uint lastskill = 0;
            if (launcher.launching.playing && ba.press || bb.press || bc.press)
            {
                if (launcher.launching.playing)
                {
                    var pipeline = launcher.Get(launcher.launching.skill);
                    if (BreakTokenDef.SKILL_CAST != (BreakTokenDef.SKILL_CAST & pipeline.breaktoken)) return;
                    lastskill = pipeline.id;
                    pipeline.Break();
                }
            }

            // 连招循环
            if (ba.press)
            {
                skillcomboDict.TryGetValue(lastskill, out var comboskill);
                foreach (uint skill in launcher.skills)
                {
                    if (comboskill > 0 && skill != comboskill) continue;
                    if (launcher.Launch(skill))
                    {
                        JoystickForawrd();
                        return;
                    }
                }
            }

            // 技能 A
            if (bb.press)
            {
                if (launcher.Launch(10011))
                {
                    JoystickForawrd();
                    return;
                }
            }

            // 技能 B
            if (bc.press)
            {
                if (launcher.Launch(10012))
                {
                    JoystickForawrd();
                }
            }
        }
    }
}
