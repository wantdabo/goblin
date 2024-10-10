using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using IRIL = Goblin.Gameplay.Common.Translations.Common.IRIL;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SpatialPosition : Resolver<RIL_SPATIAL_POSITION>
    {
        public override ushort id => RILDef.SPATIAL_POSITION;
        
        private Node node { get; set; }

        protected override void OnAwake(uint frame, RIL_SPATIAL_POSITION ril)
        {
            node = actor.EnsureBehavior<Node>();
        }

        protected override void OnResolve(uint frame, RIL_SPATIAL_POSITION ril)
        {
            node.go.transform.position = ril.position.ToVector3();
        }
    }
}
