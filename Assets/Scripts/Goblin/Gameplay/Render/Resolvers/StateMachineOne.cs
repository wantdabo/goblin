using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StateMachineOne : Resolver<RIL_STATEMACHINE_ONE>
    {
        public override ushort id => RILDef.STATEMACHINE_ONE;
        
        protected override void OnAwake(uint frame, RIL_STATEMACHINE_ONE ril)
        {
        }
        
        protected override void OnResolve(uint frame, RIL_STATEMACHINE_ONE ril)
        {
        }
    }   
}
