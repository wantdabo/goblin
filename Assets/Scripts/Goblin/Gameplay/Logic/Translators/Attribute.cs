using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 属性信息翻译器
    /// </summary>
    public class Attribute : Translator<AttributeInfo>
    {
        protected override void OnRIL(AttributeInfo info)
        {
            stage.rilsync.PushRIL(info.id, new RIL_ATTRIBUTE(info.hp, info.maxhp, info.moveseed, info.attack));
        }
    }
}