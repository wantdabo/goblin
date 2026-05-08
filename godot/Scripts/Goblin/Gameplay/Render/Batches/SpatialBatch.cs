using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using Godot;
using System;
using System.Threading.Tasks;

namespace Goblin.Gameplay.Render.Batches;

public class SpatialBatch : Batch
{
    private float dt { get; set; }
    private Action<RIL_SPATIAL> processril { get; set; } = null!;

    protected override void OnCreate()
    {
        base.OnCreate();
        processril = ProcessRIL;
    }

    protected override void OnTick(TickEvent e)
    {
        base.OnTick(e);
        if (false == world.rilbucket.SeekRILS<RIL_SPATIAL>(out var rils)) return;

        dt = e.tick;

        if (rils.Count >= 32)
            Parallel.ForEach(rils, processril);
        else
            foreach (var ril in rils) ProcessRIL(ril);

        rils.Clear();
        ObjectPool.Set(rils);
    }

    private void ProcessRIL(RIL_SPATIAL ril)
    {
        var spatialnode = world.GetAgent<SpatialAgent>(ril.actor);
        if (null == spatialnode || ChaseStatus.Arrived == spatialnode.status) return;

        var timescale = 1f;
        if (world.rilbucket.SeekRIL<RIL_TICKER>(ril.actor, out var ticker))
            timescale = Mathf.Clamp(ticker.timescale * Config.Int2Float, 0f, 1f);

        spatialnode.accumtime += dt * timescale;
        var t = Mathf.Clamp(spatialnode.accumtime / GAME_DEFINE.LOGIC_TICK.AsFloat(), 0f, 1f);
        spatialnode.position = spatialnode.prevpos.Lerp(spatialnode.nextpos, t);
        spatialnode.rotation = spatialnode.prevrot.Normalized().Slerp(spatialnode.nextrot.Normalized(), t);
        spatialnode.scale = spatialnode.nextscale;
    }
}
