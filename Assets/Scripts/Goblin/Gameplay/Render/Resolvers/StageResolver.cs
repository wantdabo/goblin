using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class StageResolver : Resolver<RIL_STAGE>
    {
        public override ushort id => RIL_DEFINE.STAGE;
        
        protected override void OnRIL(RIL_STAGE ril)
        {
        }
    }
}