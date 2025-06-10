using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Logic.RIL.DIFF;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Cross
{
    /// <summary>
    /// 渲染指令/标签合并器
    /// </summary>
    public class TagCross : RILCross<RIL_TAG, RIL_DIFF_TAG>
    {
        protected override void OnHasDel(RIL_TAG ril, RIL_DIFF_TAG diff)
        {
            ril.tags.Remove(diff.key);
        }

        protected override void OnHasNew(RIL_TAG ril, RIL_DIFF_TAG diff)
        {
            if (ril.tags.ContainsKey(diff.key)) ril.tags.Remove(diff.key);
            ril.tags.Add(diff.key, diff.tag);
        }
    }
}