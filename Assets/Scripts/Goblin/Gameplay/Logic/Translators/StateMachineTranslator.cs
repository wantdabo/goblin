using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Common;
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
        protected override void OnRIL(StateMachineInfo info, int hashcode)
        {
            if (stage.rilsync.Query(info.id, RIL_DEFINE.STATE_MACHINE).Equals(hashcode)) return;
            stage.rilsync.CacheHashCode(info.id, RIL_DEFINE.STATE_MACHINE, hashcode);

            var ril = ObjectCache.Get<RIL_STATE_MACHINE>();
            ril.Ready(info.id, hashcode);
            ril.current = info.current;
            ril.last = info.last;
            ril.frames = info.frames;
            ril.elapsed = (info.elapsed * stage.cfg.fp2int).AsUInt();
            stage.rilsync.Send(ril);
        }
    }
}