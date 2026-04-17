using Goblin.Gameplay.Logic.BehaviorInfos;
using Goblin.Gameplay.Logic.BehaviorInfos.Flows;
using Goblin.Gameplay.Logic.Flows.Executors.Common;
using Goblin.Gameplay.Logic.Flows.Executors.Instructs;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Flows.Executors
{
    /// <summary>
    /// 时间缩放执行器
    /// </summary>
    public class TimeScaleExecutor : Executor<TimeScaleData>
    {
        protected override void OnEnter((uint pipelineid, uint index) identity, TimeScaleData data, FlowInfo flowinfo, ulong target)
        {
            base.OnEnter(identity, data, flowinfo, target);
            if (false == stage.SeekBehaviorInfo(target, out TickerInfo ticker)) return;
            ticker.timescale = data.timescale * FP.EN3;
        }
    }
}