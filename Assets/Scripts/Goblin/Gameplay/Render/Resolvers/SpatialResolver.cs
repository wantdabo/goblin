using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 空间数据解析器
    /// </summary>
    public class SpatialResolver : Resolver<RIL_SPATIAL, SpatialState>
    {
        protected override SpatialState OnRIL(RILState<RIL_SPATIAL> rilstate)
        {
            var state = ObjectCache.Get<SpatialState>();
            state.position = rilstate.ril.position.ToVector3();
            state.euler = rilstate.ril.euler.ToVector3();
            state.scale = rilstate.ril.scale.ToVector3();

            return state;
        }
    }
}