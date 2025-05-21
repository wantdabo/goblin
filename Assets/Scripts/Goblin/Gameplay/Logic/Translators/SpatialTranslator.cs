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
        protected override void OnRIL(SpatialInfo info, int hashcode)
        {
            if (stage.rilsync.Query(info.id, RIL_DEFINE.SPATIAL).Equals(hashcode)) return;
            stage.rilsync.CacheHashCode(info.id, RIL_DEFINE.SPATIAL, hashcode);

            var ril = ObjectCache.Get<RIL_SPATIAL>();
            ril.Ready(info.id, hashcode);
            ril.position = info.position;
            ril.euler = info.euler;
            ril.scale = info.scale;
            stage.rilsync.Send(ril);
        }
    }
}