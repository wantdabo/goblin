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
        public override ushort id => RILDef.ATTRIBUTE_HP;

        private Node node { get; set; }
        private Attribute attribute { get; set; }

        protected override void OnAwake(uint frame, RIL_ATTRIBUTE_HP ril)
        {
            node = actor.EnsureBehavior<Node>();
            attribute = actor.EnsureBehavior<Attribute>();
            attribute.hp = ril.hp;
        }
        
        protected override void OnResolve(uint frame, RIL_ATTRIBUTE_HP ril)
        {
            if (attribute.hp > ril.hp)
            {
                engine.proxy.gameplay.eventor.Tell(new DamageDanceEvent
                {
                    position = node.go.transform.position,
                    crit = false,
                    damage = attribute.hp - ril.hp,
                    from = actor.id,
                    to = actor.id
                });
            }
            else
            {
                engine.proxy.gameplay.eventor.Tell(new CureDanceEvent
                {
                    position = node.go.transform.position,
                    cure = ril.hp - attribute.hp,
                    from = actor.id,
                    to = actor.id
                });
            }
            
            attribute.hp = ril.hp;
        }
    }
}
