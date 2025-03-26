using Goblin.Gameplay.Common.Translations;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Behaviors;
using Goblin.Gameplay.Logic.Core;

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