using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
{
    /// <summary>
    /// 子弹行为
    /// </summary>
    public class Bullet : Behavior
    {
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == stage.SeekBehaviorInfos(out List<BulletInfo> bullets)) return;
            foreach (var bullet in bullets) Execute(bullet);
        }

        /// <summary>
        /// 执行子弹行为
        /// </summary>
        /// <param name="bullet">子弹信息</param>
        private void Execute(BulletInfo bullet)
        {
            if (stage.SeekBehaviorInfo(bullet.flow, out FlowCollisionHurtInfo collisionhurt))
            {
                foreach (var target in collisionhurt.targets)
                {
                    if (bullet.owner == target.actor) continue;
                    stage.attrc.ToDamage(bullet.actor, target.actor, bullet.damage);
                }
                collisionhurt.targets.Clear();
            }
            
            // 子弹管线结束检查
            if (false == stage.flow.CheckFlowActive(bullet.flow))
            {
                stage.RmvActor(bullet.actor);
            }
        }
    }
}