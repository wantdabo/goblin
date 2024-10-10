using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Core;
using IRIL = Goblin.Gameplay.Common.Translations.Common.IRIL;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StateMachineZero : Resolver<RIL_STATEMACHINE_ZERO>
    {
        public override ushort id => RILDef.STATEMACHINE_ZERO;
        
        protected override void OnAwake(uint frame, RIL_STATEMACHINE_ZERO ril)
        {
            
        }
        
        protected override void OnResolve(uint frame, RIL_STATEMACHINE_ZERO ril)
        {
        }
    }
}
