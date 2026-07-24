using Goblin.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Common.Extensions;
using Goblin.Gameplay.Render.Core;
using Godot;
using System;

namespace Goblin.Gameplay.Render.Agents;

public class SpatialAgent : Agent
{
    public Vector3 position { get; set; }
    public Quaternion rotation { get; set; } = Quaternion.Identity;
    public float scale { get; set; } = 1f;

    public Vector3 prevpos { get; set; }
    public Quaternion prevrot { get; set; } = Quaternion.Identity;
    public Vector3 nextpos { get; set; }
    public Quaternion nextrot { get; set; } = Quaternion.Identity;
    public float nextscale { get; set; } = 1f;
    public float accumtime { get; set; }

    protected override void OnReady()
    {
        position = Vector3.Zero;
        rotation = Quaternion.Identity;
        scale = 1f;
        prevpos = Vector3.Zero;
        prevrot = Quaternion.Identity;
        nextpos = Vector3.Zero;
        nextrot = Quaternion.Identity;
        nextscale = 1f;
        accumtime = 0f;
        WatchRIL<RIL_SPATIAL>(OnRILSpatial);
    }

    protected override void OnReset()
    {
        position = Vector3.Zero;
        rotation = Quaternion.Identity;
        scale = 1f;
        prevpos = Vector3.Zero;
        prevrot = Quaternion.Identity;
        nextpos = Vector3.Zero;
        nextrot = Quaternion.Identity;
        nextscale = 1f;
        accumtime = 0f;
    }

    private void OnRILSpatial(RIL_SPATIAL ril)
    {
        prevpos = position;
        prevrot = rotation;
        nextpos = ril.position.ToVector3();
        nextrot = Quaternion.FromEuler(ril.euler.ToVector3() * MathF.PI / 180f);
        nextscale = ril.scale.AsFloat();
        accumtime = 0f;
        ChangeStatus(ChaseStatus.Chasing);
    }

    protected override bool OnArrived()
    {
        return accumtime >= GAME_DEFINE.LOGIC_TICK.AsFloat();
    }

    protected override void OnFlash()
    {
        base.OnFlash();
        if (false == world.rilbucket.SeekRIL<RIL_SPATIAL>(actor, out var ril)) return;
        var pos = ril.position.ToVector3();
        var rot = Quaternion.FromEuler(ril.euler.ToVector3() * MathF.PI / 180f);
        var scl = ril.scale.AsFloat();
        position = pos; rotation = rot; scale = scl;
        prevpos = pos; prevrot = rot;
        nextpos = pos; nextrot = rot; nextscale = scl;
        accumtime = GAME_DEFINE.LOGIC_TICK.AsFloat();
    }
}
