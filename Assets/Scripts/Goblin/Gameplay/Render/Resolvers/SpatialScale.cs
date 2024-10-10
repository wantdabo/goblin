using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SpatialScale : Resolver<RIL_SPATIAL_SCALE>
    {
        public override ushort id => RILDef.SPATIAL_SCALE;
        
        protected override void OnAwake(uint frame, RIL_SPATIAL_SCALE ril)
        {
        }
        
        protected override void OnResolve(uint frame, RIL_SPATIAL_SCALE ril)
        {
        }
    }
}
