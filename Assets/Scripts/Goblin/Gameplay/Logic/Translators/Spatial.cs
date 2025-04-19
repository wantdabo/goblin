using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Logic.Translators.Common;

namespace Goblin.Gameplay.Logic.Translators
{
    /// <summary>
    /// 空间信息翻译器
    /// </summary>
    public class Spatial : Translator<SpatialInfo>
    {
        protected override void OnRIL(SpatialInfo info)
        {
            stage.rilsync.Send(RIL_DEFINE.TYPE_RENDER, info.id, new RIL_SPATIAL
            {
                position = info.position, 
                euler = info.euler, 
                scale = info.scale
            });
        }
    }
}