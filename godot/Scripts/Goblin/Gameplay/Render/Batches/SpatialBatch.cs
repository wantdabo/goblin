using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Godot;
using System;
using System.Threading.Tasks;

namespace Goblin.Gameplay.Render.Batches;

public class SpatialBatch : Batch
{
    private float lerpt { get; set; }
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

        lerpt = Mathf.Clamp(e.tick, 0, GAME_DEFINE.MAX_TICK) / GAME_DEFINE.LOGIC_TICK.AsFloat();
        lerpt = Mathf.Clamp(lerpt, 0f, 1f);

        Parallel.ForEach(rils, processril);

        rils.Clear();
        ObjectPool.Set(rils);
    }

    private void ProcessRIL(RIL_SPATIAL ril)
    {
        var spatialnode = world.GetAgent<SpatialNode>(ril.actor);
        if (null == spatialnode || ChaseStatus.Arrived == spatialnode.status || !spatialnode.ready) return;

        var timescale = 1f;
        if (world.rilbucket.SeekRIL<RIL_TICKER>(ril.actor, out var ticker))
            timescale = Mathf.Clamp(ticker.timescale * Config.Int2Float, 0f, 1f);

        var tarpos = ril.position.ToVector3();
        var tarrot = Quaternion.FromEuler(ril.euler.ToVector3() * MathF.PI / 180f);

        spatialnode.position = spatialnode.position.Lerp(tarpos, lerpt * timescale);
        spatialnode.rotation = spatialnode.rotation.Normalized().Slerp(tarrot, lerpt * timescale);
        spatialnode.scale = ril.scale.AsFloat();
    }
}
