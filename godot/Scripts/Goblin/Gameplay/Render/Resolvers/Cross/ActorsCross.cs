using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.DIFF;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Cross
{
    /// <summary>
    /// 渲染指令/单位合并器
    /// </summary>
    public class ActorCross : RILCross<RIL_ACTOR, RIL_DIFF_ACTOR>
    {
        protected override void OnHasDel(RIL_ACTOR ril, RIL_DIFF_ACTOR diff)
        {
            rilbucket.LossRIL(diff.target);

            rilbucket.world.RmvAgent(diff.target);
            ril.actors.Remove(diff.target);
        }

        protected override void OnHasNew(RIL_ACTOR ril, RIL_DIFF_ACTOR diff)
        {
            if (ril.actors.Contains(diff.target)) return;
            
            ril.actors.Add(diff.target);
        }
    }
}