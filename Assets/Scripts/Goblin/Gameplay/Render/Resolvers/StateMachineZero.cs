using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
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
            switch (ril.sid)
            {
                case StateDef.PLAYER_IDLE:
                    animation.Play("Anbi_Idle", ril.layer);
                    break;
                case StateDef.PLAYER_RUN:
                    animation.Play("Anbi_Run", ril.layer);
                    break;
            }
        }
    }
}
