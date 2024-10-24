using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    public class SphereDetectionAction : SkillAction<SphereDetectionActionData>
    {
        public override ushort id => SkillActionDef.SPHERE_DETECTION;

        protected override void OnExecute(SphereDetectionActionData data, FP tick)
        {
        }
    }
}
