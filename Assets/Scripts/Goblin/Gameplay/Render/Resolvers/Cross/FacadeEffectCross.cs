using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.DIFF;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Cross
{
    /// <summary>
    /// 渲染指令/外观特效合并器
    /// </summary>
    public class FacadeEffectCross : RILCross<RIL_FACADE_EFFECT, RIL_DIFF_FACADE_EFFECT>
    {
        protected override void OnHasDel(RIL_FACADE_EFFECT ril, RIL_DIFF_FACADE_EFFECT diff)
        {
            if (ril.effectdict.ContainsKey(diff.effect.id)) ril.effectdict.Remove(diff.effect.id);
        }

        protected override void OnHasNew(RIL_FACADE_EFFECT ril, RIL_DIFF_FACADE_EFFECT diff)
        {
            if (ril.effectdict.ContainsKey(diff.effect.id)) ril.effectdict.Remove(diff.effect.id);
            ril.effectdict.Add(diff.effect.id, diff.effect);
        }
    }
}