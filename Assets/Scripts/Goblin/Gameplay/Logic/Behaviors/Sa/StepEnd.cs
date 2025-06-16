using System.Collections.Generic;
using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Behaviors.Sa
{
    /// <summary>
    /// Stage 驱动末尾
    /// </summary>
    public class StepEnd : Behavior
    {
        {
            base.OnTick(tick);
            if (false == stage.SeekBehaviorInfos(out List<SpatialInfo> spatials)) return;
            foreach (var spatial in spatials) spatial.preframe = (spatial.position, spatial.euler, spatial.scale);
        }
    }
}