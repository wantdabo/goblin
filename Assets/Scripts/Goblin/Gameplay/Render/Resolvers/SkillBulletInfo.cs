using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using Goblin.Gameplay.Render.Effects;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 子弹解释器
    /// </summary>
    public class SkillBulletInfo : Resolver<RIL_SKILL_BULLET_INFO>
    {
        private Node node { get; set; }
        
        private VFXController eff { get; set; } = default;

        protected override void OnAwake(uint frame, RIL_SKILL_BULLET_INFO ril)
        {
            node = actor.EnsureBehavior<Node>();
        }

        protected override void OnResolve(uint frame, RIL_SKILL_BULLET_INFO ril)
        {
            // TODO 后续，改成配置表
            switch (ril.state)
            {
                case SKILL_BULLET_STATE_DEFINE.FIRE:
                    eff = actor.stage.vfx.LoadVFX("LightningOrbBlue", node.go);
                    eff.Play();
                    break;
                case SKILL_BULLET_STATE_DEFINE.STOP:
                    eff?.Stop();
                    break;
            }
        }
    }
}
