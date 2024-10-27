using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Skills.ActionCache.Common;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 圆柱体碰撞检测行为
    /// </summary>
    public class CylinderDetection : SkillAction<CylinderDetectionData, SkillActionCache>
    {
        public override ushort id => SkillActionDef.CYLINDER_DETECTION;

        private Spatials.Spatial spatial { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = pipeline.launcher.actor.GetBehavior<Spatials.Spatial>();
        }

        protected override void OnExecute(CylinderDetectionData data, SkillActionCache cache, uint frame, FP tick)
        {
            var result = pipeline.launcher.actor.stage.phys.OverlapCylinders(pipeline.launcher.actor.id, spatial.position + spatial.rotation * data.position.ToVector(), data.radius * FP.EN3, data.height * FP.EN3, spatial.rotation);
            if (false == result.hit) return;
            pipeline.OnHit(result.actorIds);
        }
    }
}
