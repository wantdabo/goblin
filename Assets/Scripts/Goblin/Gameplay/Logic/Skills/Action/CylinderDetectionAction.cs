using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    public class CylinderDetectionAction : SkillAction<CylinderDetectionActionData>
    {
        public override ushort id => SkillActionDef.CYLINDER_DETECTION;
        
        protected override void OnExecute(CylinderDetectionActionData data, FP tick)
        {
        }
    }
}
