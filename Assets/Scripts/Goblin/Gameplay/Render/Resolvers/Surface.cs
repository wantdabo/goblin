using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 外观解釋器
    /// </summary>
    public class Surface : Resolver<RIL_SURFACE>
    {
        /// <summary>
        /// 模型
        /// </summary>
        private Model model { get; set; }

        protected override void OnAwake(uint frame, RIL_SURFACE ril)
        {
            model = actor.EnsureBehavior<Model>();
        }

        protected override void OnResolve(uint frame, RIL_SURFACE ril)
        {
            if (10000 == ril.model) model.Load("Anbi");
        }
    }
}
