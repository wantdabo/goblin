using System.Collections.Generic;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 标签数据解析器
    /// </summary>
    public class TagResolver : Resolver<RIL_TAG, TagState>
    {
        protected override TagState OnRIL(RILState<RIL_TAG> rilstate)
        {
            var state = ObjectCache.Get<TagState>();
            state.tags = rilstate.ril.tags;

            return state;
        }
    }
}