using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Skills.ActionCache;
using Goblin.Gameplay.Logic.Skills.ActionCache.Common;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 立方体碰撞检测行为
    /// </summary>
    public class BoxDetection : SkillAction<BoxDetectionData, DetectionCache>
    {
        public override ushort id => SkillActionDef.BOX_DETECTION;

        private Spatials.Spatial spatial { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = pipeline.launcher.actor.GetBehavior<Spatials.Spatial>();
        }

        protected override void OnExecute(BoxDetectionData data, DetectionCache cache, uint frame, FP tick)
        {
            var result = pipeline.launcher.actor.stage.phys.OverlapBoxs(pipeline.launcher.actor.id, spatial.position + spatial.rotation * data.position.ToVector(), data.size.ToVector(), spatial.rotation);
            if (false == result.hit) return;
            pipeline.OnHit(result.actorIds);
        }
    }
}
