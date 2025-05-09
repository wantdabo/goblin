using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 状态机信息翻译器
    /// </summary>
    public class StateMachineTranslator : Translator<StateMachineInfo>
    {
        protected override void OnRIL(StateMachineInfo info)
        {
            stage.rilsync.Send(RIL_DEFINE.TYPE_RENDER, info.id, new RIL_STATE_MACHINE
            {
                current = info.current, 
                last = info.last, 
                frames = info.frames, 
                elapsed = (info.elapsed * stage.cfg.fp2int).AsUInt()
            });
        }
    }
}