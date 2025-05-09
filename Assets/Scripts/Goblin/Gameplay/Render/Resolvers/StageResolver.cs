using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 场景数据解析器
    /// </summary>
    public class StageResolver : Resolver<RIL_STAGE, StageState>
    {
        protected override StageState OnRIL(RILState<RIL_STAGE> rilstate)
        {
            var state = ObjectCache.Get<StageState>();
            state.frame = rilstate.ril.frame;
            state.actorcnt = rilstate.ril.actorcnt;
            state.behaviorcnt = rilstate.ril.behaviorcnt;
            state.behaviorinfocnt = rilstate.ril.behaviorinfocnt;
            state.hassnapshot = rilstate.ril.hassnapshot;
            state.snapshotframe = rilstate.ril.snapshotframe;

            return state;
        }
    }
}