using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors
{
    [Ticking]
    public class Movement : Behavior<MovementInfo>
    {
        public void Move(FPVector3 motion)
        {
            var machine = actor.GetBehavior<StateMachine>();
            if (false == machine.ChangeState(STATE_DEFINE.MOVE)) return;

            var spatial = actor.GetBehavior<Spatial>();
            spatial.info.position += motion;
        }
    }
}