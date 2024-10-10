using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using IRIL = Goblin.Gameplay.Common.Translations.Common.IRIL;

namespace Goblin.Gameplay.Render.Resolvers
{
    public class AttrSurface : Resolver<RIL_ATTR_SURFACE>
    {
        public override ushort id => RILDef.ATTR_SURFACE;

        private Model model { get; set; }

        protected override void OnAwake(uint frame, RIL_ATTR_SURFACE ril)
        {
            model = actor.EnsureBehavior<Model>();
        }

        protected override void OnResolve(uint frame, RIL_ATTR_SURFACE ril)
        {
            if (10000 == ril.model) model.Load("Anbi");
        }
    }
}
