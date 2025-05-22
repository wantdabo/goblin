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
            throw new System.NotImplementedException();
        }

        protected override void OnHasNews(RIL_TAG ril, RIL_DIFF_TAG diff)
        {
            throw new System.NotImplementedException();
        }
    }
}