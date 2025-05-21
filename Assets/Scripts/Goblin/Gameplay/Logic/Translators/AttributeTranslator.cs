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
    public class AttributeTranslator : Translator<AttributeInfo>
    {
        protected override void OnRIL(AttributeInfo info, int hashcode)
        {
            if (stage.rilsync.Query(info.id, RIL_DEFINE.ATTRIBUTE).Equals(hashcode)) return;
            stage.rilsync.CacheHashCode(info.id, RIL_DEFINE.ATTRIBUTE, hashcode);
            
            var ril = ObjectCache.Get<RIL_ATTRIBUTE>();
            ril.Ready(info.id, hashcode);
            ril.hp = info.hp;
            ril.maxhp = info.maxhp;
            ril.movespeed = info.movespeed;
            ril.attack = info.attack;
            stage.rilsync.Send(ril);
        }
    }
}