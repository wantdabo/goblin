using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;
using UnityEngine;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 生命值解释器
    /// </summary>
    public class AttributeHP : Resolver<RIL_ATTRIBUTE_HP>
    {
        public override ushort id => RILDef.ATTRIBUTE_HP;

        private Attribute attribute { get; set; }

        protected override void OnAwake(uint frame, RIL_ATTRIBUTE_HP ril)
        {
            attribute = actor.EnsureBehavior<Attribute>();
            attribute.hp = ril.hp;
        }
        
        protected override void OnResolve(uint frame, RIL_ATTRIBUTE_HP ril)
        {
            if (attribute.hp > ril.hp)
            {
                Debug.Log($"OnHurt ID -> {actor.id}, {attribute.hp - ril.hp}");
            }
            else
            {
                Debug.Log($"OnCure ID -> {actor.id}, {ril.hp - attribute.hp}");
            }
            
            attribute.hp = ril.hp;
        }
    }
}
