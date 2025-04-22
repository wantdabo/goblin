using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SeatResolver : Resolver<RIL_SEAT>
    {
        public override ushort id => RIL_DEFINE.SEAT;
        
        protected override void OnRIL(RIL_SEAT ril)
        {
        }
    }
}