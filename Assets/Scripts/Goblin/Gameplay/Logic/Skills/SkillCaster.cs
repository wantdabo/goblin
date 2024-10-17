using Goblin.Core;
using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Inputs;
using System.Collections.Generic;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills
{
    public class SkillCaster : Comp
    {
        public SkillLauncher launcher { get; set; }
        private Gamepad gamepad { get; set; }
        
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
            gamepad = launcher.actor.GetBehavior<Gamepad>();
        }

        public void OnFPTick(FP tick)
        {
            // TODO 后面改为配置
            if (null == gamepad) return;
            var ba = gamepad.GetInput(InputType.BA);
            var bb = gamepad.GetInput(InputType.BB);
            var bc = gamepad.GetInput(InputType.BC);

            uint lastskill = 0;
            if (launcher.launchskill.playing && ba.press || bb.press || bc.press)
            {
                if (launcher.launchskill.playing)
                {
                    var pipeline = launcher.Get(launcher.launchskill.skill);
                    if (false == (BreakTokenDef.SKILL_CAST == (BreakTokenDef.SKILL_CAST & pipeline.breaktoken))) return;
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
                    if (launcher.Launch(skill)) return;
                }
            }
            
            // 技能 A
            if (bb.press) if (launcher.Launch(10011)) return;
            
            // 技能 B
            if (bc.press) launcher.Launch(10012);
        }
    }
}
