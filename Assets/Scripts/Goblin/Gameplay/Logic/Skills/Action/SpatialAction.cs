using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Skills.Actions.Common;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    public class SpatialAction : SkillPipelineAction
    {
        public override ushort id => SkillPipelineActionDef.SPATIAL;
        
        protected override void OnEnter()
        {
        }
        
        protected override void OnExecute(FP tick)
        {
        }
        
        protected override void OnExit()
        {
        }
    }
}
