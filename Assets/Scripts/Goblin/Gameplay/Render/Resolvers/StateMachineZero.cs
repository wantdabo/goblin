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
            switch (ril.state)
            {
                case STATE_DEFINE.PLAYER_IDLE:
                    if (STATE_DEFINE.PLAYER_FALLING2GROUND == ril.laststate)
                    {
                        animation.PlaySequeue("idle");
                        break;
                    }
                    
                    animation.EmptySequeue();
                    animation.Play("idle");
                    break;
                case STATE_DEFINE.PLAYER_RUN:
                    animation.EmptySequeue();
                    animation.Play("run");
                    break;
                case STATE_DEFINE.PLAYER_FALLING:
                    animation.EmptySequeue();
                    animation.Play("jump_down");
                    break;
                case STATE_DEFINE.PLAYER_FALLING2GROUND:
                    animation.EmptySequeue();
                    animation.Play("jump_end");
                    break;
                case STATE_DEFINE.PLAYER_JUMP_START:
                    animation.EmptySequeue();
                    animation.Play("jump_start");
                    break;
                case STATE_DEFINE.PLAYER_JUMPING:
                    animation.PlaySequeue("jump_up");
                    break;
                case STATE_DEFINE.PLAYER_ROLL:
                    animation.EmptySequeue();
                    animation.Play("roll");
                    break;
            }
        }
    }
}
