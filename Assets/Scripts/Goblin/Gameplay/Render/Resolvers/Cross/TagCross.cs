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
        protected override void OnHasDels(RIL_TAG ril, RIL_DIFF_TAG diff)
        {
            foreach (var tag in diff.tags)
            {
                ril.tags.Remove(tag.Key);
            }
        }

        protected override void OnHasNews(RIL_TAG ril, RIL_DIFF_TAG diff)
        {
            foreach (var tag in diff.tags)
            {
                if (ril.tags.ContainsKey(tag.Key)) ril.tags.Remove(tag.Key);
                ril.tags.Add(tag.Key, tag.Value);
            }
        }
    }
}