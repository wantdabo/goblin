using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills.Action.Cache;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 立方体碰撞检测行为
    /// </summary>
    public class BoxDetection : SkillAction<BoxDetectionData, DetectionCache>
    {
        public override ushort id => SKILL_ACTION_DEFINE.BOX_DETECTION;

        private Spatials.Spatial spatial { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = pipeline.launcher.actor.GetBehavior<Spatials.Spatial>();
        }

        protected override void OnExecute(BoxDetectionData data, DetectionCache cache, uint frame, FP tick)
        {
            var result = pipeline.launcher.actor.stage.phys.OverlapBox(spatial.position + spatial.rotation * data.position.ToFPVector3(), FPQuaternion.identity, data.size.ToFPVector3());
            if (false == result.hit) return;
            
            List<uint> actors = new();
            foreach (var target in result.targets)
            {
                if (target.actorId == pipeline.launcher.actor.id) continue;
                if (cache.Query(target.actorId) >= data.detectedcnt) continue;
            
                actors.Add(target.actorId);
                cache.Stamp(target.actorId);
            }
            
            pipeline.OnHit(actors.ToArray());
        }
    }
}
