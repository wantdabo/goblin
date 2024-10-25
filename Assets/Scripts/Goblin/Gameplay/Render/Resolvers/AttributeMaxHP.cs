using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Render.Behaviors;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 最大生命值解释器
    /// </summary>
    public class AttributeMaxHP : Resolver<RIL_ATTRIBUTE_MAXHP>
    {
        public override ushort id => RILDef.ATTRIBUTE_MAXHP;
        
        private Attribute attribute { get; set; }

        protected override void OnAwake(uint frame, RIL_ATTRIBUTE_MAXHP ril)
        {
            attribute = actor.EnsureBehavior<Attribute>();
            attribute.maxhp = ril.maxhp;
        }
        
        protected override void OnResolve(uint frame, RIL_ATTRIBUTE_MAXHP ril)
        {
            attribute.maxhp = ril.maxhp;
        }
    }
}
