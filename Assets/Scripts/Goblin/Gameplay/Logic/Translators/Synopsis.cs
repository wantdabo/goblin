using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 梗概翻译器
    /// </summary>
    public class Synopsis : Translator<StageInfo>
    {
        protected override void OnRIL(StageInfo info)
        {
            uint behaviorcnt = 0;
            foreach (var types in info.behaviortypes.Values) behaviorcnt += (uint)types.Count;
            uint behaviorinfocnt = 0;
            foreach (var behaviors in info.behaviorinfos.Values) behaviorinfocnt += (uint)behaviors.Count;
            RIL_SYNOPSIS ril = new RIL_SYNOPSIS
            {
                frame = info.frame,
                actorcnt = (uint)info.actors.Count,
                behaviorcnt = behaviorcnt,
                behaviorinfocnt = behaviorinfocnt,
                hassnapshot = stage.hassnapshot,
                snapshotframe = stage.snapshotframe,
            };
            stage.rilsync.Push(info.id, ril);
        }
    }
}