using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.SkillDatas;
using Goblin.Gameplay.Logic.Actors;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Skills.Action.Cache.Common;
using Goblin.Gameplay.Logic.Skills.Action.Common;
using Goblin.Gameplay.Logic.Skills.Bullets;
using Goblin.Gameplay.Logic.Skills.Bullets.Common;

namespace Goblin.Gameplay.Logic.Skills.Action
{
    /// <summary>
    /// 技能子弹行为
    /// </summary>
    public class BulletEvent : SkillAction<BulletEventData, SkillActionCache>
    {
        public override ushort id => SKILL_ACTION_DEFINE.BULLET_EVENT;
        
        private Spatials.Spatial spatial { get; set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = pipeline.launcher.actor.GetBehavior<Spatials.Spatial>();
        }

        protected override void OnEnter(BulletEventData data, SkillActionCache cache)
        {
            base.OnEnter(data, cache);
            // TODO 子弹测试代码，需要删除
            var bullet = pipeline.launcher.actor.stage.AddActor<Bullet>();
            bullet.Create<BULLET_10001>();
            bullet.eventor.Tell(new BulletFireEvent
            {
                owner = pipeline.launcher.actor.id,
                position = spatial.position + spatial.rotation * data.position.ToVector3(),
                damage = default,
                args = default
            });
        }
    }
}
