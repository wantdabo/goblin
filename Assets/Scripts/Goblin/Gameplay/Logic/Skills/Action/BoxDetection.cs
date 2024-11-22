using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills.Action.Cache;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Spatials;
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
            // var result = pipeline.launcher.actor.stage.phys.OverlapBoxs(spatial.position + spatial.rotation * data.position.ToVector(), data.size.ToVector(), spatial.rotation);
            // if (false == result.hit) return;
            //
            // // TODO 存在性能问题，需要优化
            // List<uint> actors = new();
            // foreach (uint actorId in result.actorIds)
            // {
            //     if (actorId == pipeline.launcher.actor.id) continue;
            //     if (cache.Query(actorId) >= data.detectedcnt) continue;
            //
            //     actors.Add(actorId);
            //     cache.Stamp(actorId);
            // }
            //
            // pipeline.OnHit(actors.ToArray());
        }
    }
}
