using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators;

/// <summary>
/// 外观动画翻译器
/// </summary>
public class FacadeAnimationTranslator : Translator<FacadeInfo, RIL_FACADE_ANIMATION>
{
    public override ushort id => RIL_DEFINE.FACADE_ANIMATION;

    protected override int OnCalcHashCode(FacadeInfo info)
    {
        int hash = 17;
        hash = hash * 31 + info.actor.GetHashCode();

        // 逐层哈希（animslots 已按优先级降序排列，每层取首个 active）
        for (byte l = 0; l < ANIM_DEFINE.LAYER_MAX; l++)
        {
            var winner = FindLayerWinner(info, l);
            if (null == winner && 0 != l) continue;

            byte ws = (null != winner) ? winner.animstate : info.animstate;
            uint wh = (null != winner) ? winner.animhash : info.animhash;
            hash = hash * 31 + ws.GetHashCode();
            hash = hash * 31 + unchecked((int)wh);
        }

        hash = hash * 31 + info.animelapsed.GetHashCode();
        hash = hash * 31 + info.effectincrement.GetHashCode();

        return hash;
    }

    protected override void OnRIL(FacadeInfo info, RIL_FACADE_ANIMATION ril)
    {
        // 逐层取最高优先级活跃槽位
        ril.layercount = 0;
        for (byte l = 0; l < ANIM_DEFINE.LAYER_MAX; l++)
        {
            var winner = FindLayerWinner(info, l);
            if (null == winner && 0 != l) continue;

            ril.layeranims[ril.layercount].layer = l;
            if (null != winner)
            {
                ril.layeranims[ril.layercount].animstate = winner.animstate;
                ril.layeranims[ril.layercount].animhash = winner.animhash;
            }
            else
            {
                ril.layeranims[ril.layercount].animstate = info.animstate;
                ril.layeranims[ril.layercount].animhash = info.animhash;
            }
            ril.layercount++;
        }

        // 兼容旧字段（镜像 layer 0）
        if (0 < ril.layercount)
        {
            ril.animstate = ril.layeranims[0].animstate;
            ril.animhash = ril.layeranims[0].animhash;
        }

        ril.animelapsed = (info.animelapsed * stage.cfg.fp2int).AsUInt();
    }

    /// <summary>
    /// 查找指定层的最高优先级活跃槽位
    /// </summary>
    private static AnimationSlot FindLayerWinner(FacadeInfo info, byte layer)
    {
        foreach (var slot in info.animslots)
        {
            if (false == slot.active || slot.layer != layer) continue;
            return slot;
        }
        return null;
    }
}