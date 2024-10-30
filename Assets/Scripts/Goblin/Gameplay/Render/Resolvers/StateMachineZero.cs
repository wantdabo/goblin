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
            // TODO 后续要改为配置读取解析
            if (STATE_DEFINE.PLAYER_HURT == ril.state)
            {
                animation.EmptySequeue();
                animation.Play("Anbi_Hit_H_Back", true);
            }

            if (STATE_DEFINE.PLAYER_IDLE == ril.state && STATE_DEFINE.PLAYER_RUN == ril.laststate)
            {
                animation.EmptySequeue();
                animation.Play("Anbi_Run_End");
            }
            else if (STATE_DEFINE.PLAYER_RUN == ril.state && STATE_DEFINE.PLAYER_IDLE == ril.laststate)
            {
                animation.EmptySequeue();
                animation.Play("Anbi_Run_Start");
            }
            else
            {
                switch (ril.state)
                {
                    case STATE_DEFINE.PLAYER_IDLE:
                        animation.Play("Anbi_Idle");
                        break;
                    case STATE_DEFINE.PLAYER_RUN:
                        animation.Play("Anbi_Run");
                        break;
                }
                
                return;
            }

            switch (ril.state)
            {
                case STATE_DEFINE.PLAYER_IDLE:
                    animation.PlaySequeue("Anbi_Idle");
                    break;
                case STATE_DEFINE.PLAYER_RUN:
                    animation.PlaySequeue("Anbi_Run");
                    break;
            }
        }
    }
}
