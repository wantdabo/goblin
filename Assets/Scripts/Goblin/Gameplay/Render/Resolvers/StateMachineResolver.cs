using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StateMachineResolver : Resolver<RIL_STATE_MACHINE>
    {
        public override ushort id => RIL_DEFINE.STATE_MACHINE;
        
        protected override void OnRIL(RILState rilstate, RIL_STATE_MACHINE ril)
        {
        }
    }
}