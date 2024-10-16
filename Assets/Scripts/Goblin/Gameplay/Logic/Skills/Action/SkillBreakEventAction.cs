using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    public class SkillBreakEventAction : SkillAction<SkillBreakEventActionData>
    {
        public override ushort id => SkillActionDef.SKILL_BREAK_EVENT;
        protected override void OnExecute(SkillBreakEventActionData data, FP tick)
        {
            if (BreakTokenType.NONE == data.token) return;

            if (BreakTokenType.JOYSTICK == (BreakTokenType.JOYSTICK & data.token))
            {
                
            }
            else if (BreakTokenType.RECV_HURT == (BreakTokenType.RECV_HURT & data.token))
            {
                
            }
            else if (BreakTokenType.RECV_CONTROL == (BreakTokenType.RECV_CONTROL & data.token))
            {
                
            }
            else if (BreakTokenType.SKILL_CAST == (BreakTokenType.SKILL_CAST & data.token))
            {
                
            }
            else if (BreakTokenType.COMBO_SKILL_CAST == (BreakTokenType.COMBO_SKILL_CAST & data.token))
            {
                
            }
        }
    }
}
