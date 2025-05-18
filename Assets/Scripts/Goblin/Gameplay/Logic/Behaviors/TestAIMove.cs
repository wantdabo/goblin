using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    public class TestAIMoveInfo : BehaviorInfo
    {
        public FP elapsed { get; set; }
        public bool moveleft { get; set; }
        
        protected override void OnReady()
        {
            elapsed = 0;
            moveleft = false;
        }

        protected override void OnReset()
        {
            elapsed = 0;
            moveleft = false;
        }

        protected override BehaviorInfo OnClone()
        {
            var clone = ObjectCache.Get<TestAIMoveInfo>();
            clone.Ready(id);
            clone.elapsed = elapsed;
            clone.moveleft = moveleft;
            
            return clone;
        }
    }

    public class TestAIMove : Behavior<TestAIMoveInfo>
    {
        protected override void OnTick(FP tick)
        {
            base.OnTick(tick);
            info.elapsed += tick;
            if (false == actor.SeekBehavior(out Movement movement)) return;
            
            movement.Move((info.moveleft ? FPVector3.left : FPVector3.right), tick);

            if (info.elapsed >= 3)
            {
                info.elapsed = 0;
                info.moveleft = false == info.moveleft;
            }
        }
    }
}