using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 立方体碰撞检测行为
    /// </summary>
    public class BoxDetectionAction : SkillAction<BoxDetectionActionData>
    {
        public override ushort id => SkillActionDef.BOX_DETECTION;
        
        private Spatial spatial { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = pipeline.launcher.actor.GetBehavior<Spatial>();
        }

        protected override void OnExecute(BoxDetectionActionData data, uint frame, FP tick)
        {
            var result = pipeline.launcher.actor.stage.phys.OverlapBoxs(pipeline.launcher.actor.id, spatial.position + spatial.rotation * data.position.ToVector(), data.size.ToVector(), spatial.rotation);
            if (false == result.hit) return;
            pipeline.OnHit(result.actorIds);
        }
    }
}
