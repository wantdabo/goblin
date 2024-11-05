using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using Goblin.Sys.Gameplay;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 生命值解释器
    /// </summary>
    public class AttributeHP : Resolver<RIL_ATTRIBUTE_HP>
    {
        private Attribute attribute { get; set; }

        protected override void OnAwake(uint frame, RIL_ATTRIBUTE_HP ril)
        {
            attribute = actor.EnsureBehavior<Attribute>();
            attribute.hp = ril.hp;
        }

        protected override void OnResolve(uint frame, RIL_ATTRIBUTE_HP ril)
        {
            attribute.hp = ril.hp;
        }
    }
}
