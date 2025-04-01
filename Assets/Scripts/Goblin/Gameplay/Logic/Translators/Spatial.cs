using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    public class Spatial : Translator<SpatialInfo>
    {
        protected override void OnRIL(ulong id, SpatialInfo info)
        {
            stage.rilsync.PushRIL(id, new RIL_SPATIAL(info.position, info.euler, info.scale));
        }
    }
}