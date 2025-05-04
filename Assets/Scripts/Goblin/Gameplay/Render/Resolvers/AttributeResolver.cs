using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.RIL.Common;
using Goblin.Gameplay.Render.Resolvers.Common;
using Goblin.Gameplay.Render.Resolvers.States;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 属性数据解析器
    /// </summary>
    public class AttributeResolver : Resolver<RIL_ATTRIBUTE, AttributeState>
    {
        protected override AttributeState OnRIL(RILState<RIL_ATTRIBUTE> rilstate)
        {
            var state = ObjectCache.Get<AttributeState>();
            state.hp = rilstate.ril.hp;
            state.maxhp = rilstate.ril.maxhp;
            state.movespeed = rilstate.ril.movespeed;
            state.attack = rilstate.ril.attack;

            return state;
        }
    }
}