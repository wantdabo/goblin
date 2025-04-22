using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class TagResolver : Resolver<RIL_TAG>
    {
        public override ushort id => RIL_DEFINE.TAG;
        
        protected override void OnRIL(RIL_TAG ril)
        {
        }
    }
}