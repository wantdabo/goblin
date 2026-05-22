using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators;

/// <summary>
/// 属性信息翻译器（Sa 级，遍历 AttributeBucketInfo 为每个 Actor 产出 RIL_ATTRIBUTE）
/// </summary>
public class AttributeTranslator : Translator<AttributeBucketInfo, RIL_ATTRIBUTE>
{
    public override ushort id => RIL_DEFINE.ATTRIBUTE;

    protected override int OnCalcHashCode(AttributeBucketInfo info) => 0;

    protected override void OnRIL(BehaviorInfo info)
    {
        if (info is not AttributeBucketInfo bucket) return;
        foreach (var kv in bucket.attributes)
        {
            var actor = kv.Key;
            if (false == stage.cache.Valid(actor)) continue;

            int hash = 17;
            foreach (var attr in kv.Value)
            {
                hash = hash * 31 + attr.Key.GetHashCode();
                hash = hash * 31 + attr.Value.GetHashCode();
            }

            if (stage.rilsync.Query(actor, id).Equals(hash)) continue;
            stage.rilsync.CacheHashCode(actor, id, hash);

            var ril = RILCache.Ensure<RIL_ATTRIBUTE>();
            ril.Ready(actor, hash);
            ril.hp = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.HP);
            ril.maxhp = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.MAXHP);
            ril.movespeed = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.MOVESPEED);
            ril.attack = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.ATTACK);
            stage.rilsync.Send(ril);
        }
    }

    protected override void OnRIL(AttributeBucketInfo info, RIL_ATTRIBUTE ril) { }
}
