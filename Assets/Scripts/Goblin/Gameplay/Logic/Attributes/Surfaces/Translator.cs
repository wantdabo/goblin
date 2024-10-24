using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;

namespace Goblin.Gameplay.Logic.Attributes.Surfaces
{
    /// <summary>
    /// 外观翻译
    /// </summary>
    public class Translator : Translator<Surface>
    {
        /// <summary>
        /// 模型 ID
        /// </summary>
        private uint model { get; set; }

        protected override void OnRIL()
        {
            if (model != behavior.model)
            {
                model = behavior.model;
                behavior.actor.stage.rilsync.PushRIL(behavior.actor.id, new RIL_SURFACE(model));
            }
        }
    }
}
