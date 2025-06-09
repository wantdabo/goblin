using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Sa;
using Goblin.Gameplay.Logic.Common.Extensions;
using Goblin.Gameplay.Logic.Flows.Defines;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// POSITION 变化指令
    /// </summary>
    public class SpatialPositionExecutor : Executor<SpatialPositionData>
    {
        protected override void OnExecute(SpatialPositionData data, FlowInfo flowinfo)
        {
            base.OnExecute(data, flowinfo);
            if (false == stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo spatial)) return;
            var motion = data.position.ToFPVector3();
            switch (data.type)
            {
                case SPATIAL_DEFINE.POSITION_WORLD:
                    spatial.position += motion;
                    break;
                case SPATIAL_DEFINE.POSITION_SELF:
                    var rotation = FPQuaternion.Euler(spatial.euler);
                    motion = rotation * motion;
                    spatial.position += motion;
                    break;
            }
        }
    }
}