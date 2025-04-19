using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 场景翻译器
    /// </summary>
    public class Stage : Translator<StageInfo>
    {
        protected override void OnRIL(StageInfo info)
        {
            uint behaviorcnt = 0;
            foreach (var types in info.behaviortypes.Values) behaviorcnt += (uint)types.Count;
            uint behaviorinfocnt = 0;
            foreach (var behaviors in info.behaviorinfos.Values) behaviorinfocnt += (uint)behaviors.Count;
            RIL_STAGE ril = new RIL_STAGE
            {
                frame = info.frame,
                actorcnt = (uint)info.actors.Count,
                behaviorcnt = behaviorcnt,
                behaviorinfocnt = behaviorinfocnt,
                hassnapshot = stage.hassnapshot,
                snapshotframe = stage.snapshotframe,
            };
            stage.rilsync.Send(RIL_DEFINE.TYPE_RENDER, info.id, ril);
        }
    }
}