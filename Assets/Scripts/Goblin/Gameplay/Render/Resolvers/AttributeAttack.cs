using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 攻击力解释器
    /// </summary>
    public class AttributeAttack : Resolver<RIL_ATTRIBUTE_ATTACK>
    {
        public override ushort id => RILDef.ATTRIBUTE_ATTACK;
        
        private Attribute attribute { get; set; }

        protected override void OnAwake(uint frame, RIL_ATTRIBUTE_ATTACK ril)
        {
            attribute = actor.EnsureBehavior<Attribute>();
            attribute.attack = ril.attack;
        }
        
        protected override void OnResolve(uint frame, RIL_ATTRIBUTE_ATTACK ril)
        {
            attribute.attack = ril.attack;
        }
    }
}
