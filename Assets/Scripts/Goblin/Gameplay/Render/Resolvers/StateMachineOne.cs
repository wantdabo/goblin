using Goblin.Gameplay.Logic.Translations;
using Goblin.Gameplay.Logic.Translations.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StateMachineOne : Resolver<RIL_STATEMACHINE_ONE>
    {
        public override ushort id => IRIL.STATEMACHINE_ONE;
        
        protected override void OnAwake(uint frame, RIL_STATEMACHINE_ONE ril)
        {
        }
        
        protected override void OnResolve(uint frame, RIL_STATEMACHINE_ONE ril)
        {
        }
    }   
}
