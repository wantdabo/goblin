using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    public class TestAIMove : Behavior
    {
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            if (false == actor.SeekBehavior(out Movement movement)) return;
            movement.Move(new FPVector3(stage.random.Range(-FP.One, FP.One), 0, stage.random.Range(-FP.One, FP.One)), tick);
        }
    }
}