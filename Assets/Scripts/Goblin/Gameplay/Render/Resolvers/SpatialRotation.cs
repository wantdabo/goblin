using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SpatialRotation : Resolver<RIL_SPATIAL_ROTATION>
    {
        public override ushort id => RILDef.SPATIAL_ROTATION;
        
        protected override void OnAwake(uint frame, RIL_SPATIAL_ROTATION ril)
        {
        }
        
        protected override void OnResolve(uint frame, RIL_SPATIAL_ROTATION ril)
        {
        }
    }
}
