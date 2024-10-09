using Goblin.Gameplay.Logic.Translations;
using Goblin.Gameplay.Logic.Translations.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SpatialScale : Resolver<RIL_SPATIAL_SCALE>
    {
        public override ushort id => IRIL.SPATIAL_SCALE;
        
        protected override void OnAwake(uint frame, RIL_SPATIAL_SCALE ril)
        {
        }
        
        protected override void OnResolve(uint frame, RIL_SPATIAL_SCALE ril)
        {
        }
    }
}
