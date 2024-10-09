using Goblin.Gameplay.Logic.Translations;
using Goblin.Gameplay.Logic.Translations.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SpatialRotation : Resolver<RIL_SPATIAL_ROTATION>
    {
        public override ushort id => IRIL.SPATIAL_ROTATION;
        
        protected override void OnAwake(uint frame, RIL_SPATIAL_ROTATION ril)
        {
        }
        
        protected override void OnResolve(uint frame, RIL_SPATIAL_ROTATION ril)
        {
        }
    }
}
