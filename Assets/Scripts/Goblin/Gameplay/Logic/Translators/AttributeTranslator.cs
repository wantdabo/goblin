using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 属性信息翻译器
    /// </summary>
    public class AttributeTranslator : Translator<AttributeInfo, RIL_ATTRIBUTE>
    {
        public override ushort id => RIL_DEFINE.ATTRIBUTE;

        protected override void OnRIL(AttributeInfo info, RIL_ATTRIBUTE ril)
        {
            ril.hp = info.hp;
            ril.maxhp = info.maxhp;
            ril.movespeed = info.movespeed;
            ril.attack = info.attack;
        }
    }
}