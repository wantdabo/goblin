using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.Flows.BehaviorInfos;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    /// <summary>
    /// 子弹行为
    /// </summary>
    public class Bullet : Behavior<BulletInfo>
    {
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == stage.SeekBehaviorInfo(info.flow, out CollisionInfo collisioninfo)) return;
            
            while (collisioninfo.collisions.TryDequeue(out var target))
            {
                stage.calc.ToDamage(actor.id, target, info.damage);
            }
        }
    }
}