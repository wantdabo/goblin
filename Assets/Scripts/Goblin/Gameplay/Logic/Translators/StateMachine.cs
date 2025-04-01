using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    public class StateMachine : Translator<StateMachineInfo>
    {
        protected override void OnRIL(ulong id, StateMachineInfo info)
        {
            stage.rilsync.PushRIL(id, new RIL_STATE_MACHINE(info.current, info.frames));
        }
    }
}