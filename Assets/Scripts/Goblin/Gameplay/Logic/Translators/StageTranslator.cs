using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 场景翻译器
    /// </summary>
    public class StageTranslator : Translator<StageInfo, RIL_STAGE>
    {
        public override ushort id => RIL_DEFINE.STAGE;

        protected override void OnRIL(StageInfo info, RIL_STAGE ril)
        {
            uint behaviorcnt = 0;
            foreach (var types in info.behaviortypes.Values) behaviorcnt += (uint)types.Count;
            uint behaviorinfocnt = 0;
            foreach (var behaviors in info.behaviorinfos.Values) behaviorinfocnt += (uint)behaviors.Count;
            
            ril.frame = stage.frame;
            ril.actorcnt = (uint)info.actors.Count;
            ril.behaviorcnt = behaviorcnt;
            ril.behaviorinfocnt = behaviorinfocnt;
            ril.hassnapshot = stage.hassnapshot;
            ril.snapshotframe = stage.snapshotframe;
        }
    }
}