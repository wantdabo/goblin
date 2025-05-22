using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.DIFF;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Cross
{
    /// <summary>
    /// 渲染指令/单位合并器
    /// </summary>
    public class ActorsCross : RILCross<RIL_ACTORS, RIL_DIFF_ACTOR>
    {
        protected override void OnHasDel(RIL_ACTORS ril, RIL_DIFF_ACTOR diff)
        {
            rilbucket.world.RmvAgent(diff.target);
            ril.actors.Remove(diff.target);
        }

        protected override void OnHasNew(RIL_ACTORS ril, RIL_DIFF_ACTOR diff)
        {
            ril.actors.Add(diff.target);
        }
    }
}