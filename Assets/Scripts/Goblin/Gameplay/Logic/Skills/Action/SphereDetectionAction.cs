using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas.Action;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    public class SphereDetectionAction : SkillAction<SphereDetectionActionData>
    {
        public override ushort id => SkillActionDef.SPHERE_DETECTION;
        private Spatial spatial { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = pipeline.launcher.actor.GetBehavior<Spatial>();
        }

        protected override void OnExecute(SphereDetectionActionData data, FP tick)
        {
            var result = pipeline.launcher.actor.stage.phys.OverlapSpheres(pipeline.launcher.actor.id, spatial.position + spatial.rotation * data.position.ToVector(), data.radius * FP.EN3);
            if (false == result.hit) return;
            pipeline.launcher.actor.eventor.Tell(new SkillCollisionEvent { id = pipeline.id, actorIds = result.actorIds });
        }
    }
}
