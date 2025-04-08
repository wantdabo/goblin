using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 状态机信息翻译器
    /// </summary>
    public class StateMachine : Translator<StateMachineInfo>
    {
        protected override void OnRIL(StateMachineInfo info)
        {
            stage.rilsync.PushRIL(info.id, new RIL_STATE_MACHINE(info.current, info.last, info.frames));
        }
    }
}