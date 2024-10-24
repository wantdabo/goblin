using Goblin.Gameplay.Common.Defines;
using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Common.Translations.Common;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Render.Resolvers
{
    /// <summary>
    /// 状态机一层解释器
    /// </summary>
    public class StateMachineOne : Resolver<RIL_STATEMACHINE_ONE>
    {
        public override ushort id => RILDef.STATEMACHINE_ONE;
        
        protected override void OnAwake(uint frame, RIL_STATEMACHINE_ONE ril)
        {
        }
        
        protected override void OnResolve(uint frame, RIL_STATEMACHINE_ONE ril)
        {
        }
    }   
}
