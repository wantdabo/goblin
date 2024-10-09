using Goblin.Gameplay.Logic.Translations;
using Goblin.Gameplay.Logic.Translations.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StateMachineZero : Resolver<RIL_STATEMACHINE_ZERO>
    {
        public override ushort id => IRIL.STATEMACHINE_ZERO;
        
        protected override void OnAwake(uint frame, RIL_STATEMACHINE_ZERO ril)
        {
            
        }
        
        protected override void OnResolve(uint frame, RIL_STATEMACHINE_ZERO ril)
        {
        }
    }
}
