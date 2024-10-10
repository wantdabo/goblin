using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Logic.Attributes.Surfaces
{
    public class Translator : Translator<Surface>
    {
        private uint model { get; set; }

        protected override void OnRIL()
        {
            if (model != behavior.model)
            {
                model = behavior.model;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_ATTR_SURFACE(model));
            }
        }
    }
}
