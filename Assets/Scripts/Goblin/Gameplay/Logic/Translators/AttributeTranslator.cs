using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 属性信息翻译器
    /// </summary>
    public class AttributeTranslator : Translator<AttributeInfo>
    {
        protected override void OnRIL(AttributeInfo info)
        {
            stage.rilsync.Send(RIL_DEFINE.TYPE_RENDER, info.id, new RIL_ATTRIBUTE
            {
                hp = info.hp,
                maxhp = info.maxhp,
                movespeed = info.movespeed,
                attack = info.attack
            });
        }
    }
}