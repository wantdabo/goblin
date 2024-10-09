using Goblin.Gameplay.Logic.Translations;
using Goblin.Gameplay.Logic.Translations.Common;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Common;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class SpatialPosition : Resolver<RIL_SPATIAL_POSITION>
    {
        public override ushort id => IRIL.SPATIAL_POSITION;
        
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
