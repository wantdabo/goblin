using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    public class Movement : Behavior<MovementInfo>
    {
        public void Move(FPVector3 motion)
        {
            if (false == actor.SeekBehavior(out StateMachine machine) || false == machine.TryChangeState(STATE_DEFINE.MOVE)) return;
            if (actor.SeekBehaviorInfo(out SpatialInfo spatial))
            {
                spatial.position += motion;
            }
            MarkMotion();
        }

        public void MarkMotion()
        {
            info.motion = true;
        }

        protected override void OnTickEnd()
        {
            base.OnTickEnd();
            if (false == info.motion && actor.SeekBehavior(out StateMachine machine))
            {
                machine.TryChangeState(STATE_DEFINE.IDLE);
            }

            info.motion = false;
        }
    }
}