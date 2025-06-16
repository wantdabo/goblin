using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
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
            if (stage.SeekBehaviorInfo(bullet.flow, out FlowDamageInfo flowdamage))
            {
                while (flowdamage.targets.TryDequeue(out var target))
                {
                    stage.calc.ToDamage(bullet.actor, target.actor, bullet.damage);
                }
            }
            
            // 子弹管线结束检查
            if (false == stage.SeekBehaviorInfo(bullet.flow, out FlowInfo flowinfo) || flowinfo.timeline >= flowinfo.length)
            {
                stage.RmvActor(bullet.actor);
            }
        }
    }
}