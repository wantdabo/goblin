using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class AttributeResolver : Resolver<RIL_ATTRIBUTE>
    {
        public override ushort id => RIL_DEFINE.ATTRIBUTE;
        
        protected override void OnRIL(RIL_ATTRIBUTE ril)
        {
        }
    }
}