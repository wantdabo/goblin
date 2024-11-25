using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills.Action.Cache.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 球体碰撞检测行为
    /// </summary>
    public class SphereDetection : SkillAction<SphereDetectionData, SkillActionCache>
    {
        public override ushort id => SKILL_ACTION_DEFINE.SPHERE_DETECTION;

        private Spatials.Spatial spatial { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = pipeline.launcher.actor.GetBehavior<Spatials.Spatial>();
        }
    }
}
