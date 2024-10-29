using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 移动速度解释器
    /// </summary>
    public class AttributeMoveSpeed : Resolver<RIL_ATTRIBUTE_MOVESPEED>
    {
        private Attribute attribute { get; set; }

        protected override void OnAwake(uint frame, RIL_ATTRIBUTE_MOVESPEED ril)
        {
            attribute = actor.EnsureBehavior<Attribute>();
            attribute.movespeed = ril.movespeed.AsFloat();
        }
        
        protected override void OnResolve(uint frame, RIL_ATTRIBUTE_MOVESPEED ril)
        {
            attribute.movespeed = ril.movespeed.AsFloat();
        }
    }
}
