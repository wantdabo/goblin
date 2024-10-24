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
    public class StateMachineZero : Resolver<RIL_STATEMACHINE_ZERO>
    {
        public override ushort id => RILDef.STATEMACHINE_ZERO;

        private AnimancerAnimation animation { get; set; }

        protected override void OnAwake(uint frame, RIL_STATEMACHINE_ZERO ril)
        {
            animation = actor.EnsureBehavior<AnimancerAnimation>();
        }
        
        protected override void OnResolve(uint frame, RIL_STATEMACHINE_ZERO ril)
        {
            // TODO 后续要改为配置读取解析
            if (StateDef.PLAYER_IDLE == ril.state && StateDef.PLAYER_RUN == ril.laststate)
            {
                animation.EmptySequeue();
                animation.Play("Anbi_Run_End", ril.layer);
            }
            else if (StateDef.PLAYER_RUN == ril.state && StateDef.PLAYER_IDLE == ril.laststate)
            {
                animation.EmptySequeue();
                animation.Play("Anbi_Run_Start", ril.layer);
            }
            else
            {
                switch (ril.state)
                {
                    case StateDef.PLAYER_IDLE:
                        animation.Play("Anbi_Idle", ril.layer);
                        break;
                    case StateDef.PLAYER_RUN:
                        animation.Play("Anbi_Run", ril.layer);
                        break;
                }
                
                return;
            }

            switch (ril.state)
            {
                case StateDef.PLAYER_IDLE:
                    animation.PlaySequeue("Anbi_Idle", ril.layer);
                    break;
                case StateDef.PLAYER_RUN:
                    animation.PlaySequeue("Anbi_Run", ril.layer);
                    break;
            }
        }
    }
}
