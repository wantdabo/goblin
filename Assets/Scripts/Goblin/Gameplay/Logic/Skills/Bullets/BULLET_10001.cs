using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Logic.Attributes;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Skills.Bullets.Common;
using Goblin.Gameplay.Logic.Spatials;
using TrueSync;

namespace Goblin.Gameplay.Logic.Skills.Bullets
{
    /// <summary>
    /// 子弹 10001
    /// </summary>
    public class BULLET_10001 : SkillBullet
    {
        public override uint id => BULLET_DEFINE.BULLET_10001;

        private Spatial spatial { get; set; }
        private TSVector dire { get; set; }
        private uint speed { get; set; } = 75;

        protected override void OnCreate()
        {
            base.OnCreate();
            spatial = actor.GetBehavior<Spatial>();
        }

        protected override void OnFire()
        {
            spatial.position = position;

            var ospatial = actor.stage.GetActor(owner).GetBehavior<Spatial>();
            dire = ospatial.eulerAngle.normalized;
            actor.ticker.Timing((t) => actor.eventor.Tell<BulletStopEvent>(), 5, 1);
        }

        protected override void OnFlying(FP tick)
        {
            speed += 2;
            spatial.position += new TSVector(dire.y, dire.x, FP.Zero) * speed * FP.EN1 * tick;
        }

        protected override void OnEnter(Actor target)
        {
            // TODO 后面修改为读取配置, 使用配置中的技能伤害强度
            var hurt = new HurtEvent()
            {
                damage = actor.stage.calc.ChargeDamage(actor.id, 1),
                from = owner
            };
            target.eventor.Tell(hurt);
        }
    }
}
