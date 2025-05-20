using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 空间信息翻译器
    /// </summary>
    public class SpatialTranslator : Translator<SpatialInfo>
    {
        protected override void OnRIL(SpatialInfo info)
        {
            var ril = ObjectCache.Get<RIL_SPATIAL>();
            ril.Ready(info.id);
            ril.position = info.position;
            ril.euler = info.euler;
            ril.scale = info.scale;
            stage.rilsync.Send(ril);
        }
    }
}