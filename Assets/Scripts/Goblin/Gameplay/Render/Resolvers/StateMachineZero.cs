using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Logic.Common.StateMachine;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 状态机零层解释器
    /// </summary>
    public class StateMachineZero : Resolver<RIL_STATE_MACHINE_ZERO>
    {
        private AnimancerAnimation animation { get; set; }

        protected override void OnAwake(uint frame, RIL_STATE_MACHINE_ZERO ril)
        {
            animation = actor.EnsureBehavior<AnimancerAnimation>();
        }
        
        protected override void OnResolve(uint frame, RIL_STATE_MACHINE_ZERO ril)
        {
            animation.EmptySequeue();
            switch (ril.state)
            {
                case STATE_DEFINE.PLAYER_IDLE:
                    animation.Play("idle");
                    break;
                case STATE_DEFINE.PLAYER_RUN:
                    animation.Play("run");
                    break;
            }
        }
    }
}
