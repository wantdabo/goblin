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
    public Quaternion rotation { get; set; }
    public float scale { get; set; }
    public bool ready { get; private set; }

    protected override void OnReady()
    {
        position = Vector3.Zero;
        rotation = Quaternion.Identity;
        scale = 1f;
        ready = false;
        WatchRIL<RIL_SPATIAL>(OnRILSpatial);
    }

    protected override void OnReset()
    {
        position = Vector3.Zero;
        rotation = Quaternion.Identity;
        scale = 1f;
        ready = false;
    }

    private void OnRILSpatial(RIL_SPATIAL ril) => ChangeStatus(ChaseStatus.Chasing);

    protected override bool OnArrived()
    {
        if (false == world.rilbucket.SeekRIL<RIL_SPATIAL>(actor, out var ril)) return true;
        if (!ready)
        {
            position = ril.position.ToVector3();
            rotation = Quaternion.FromEuler(ril.euler.ToVector3() * MathF.PI / 180f);
            scale = ril.scale.AsFloat();
            ready = true;
            return true;
        }
        var tarpos = ril.position.ToVector3();
        var tarrot = Quaternion.FromEuler(ril.euler.ToVector3() * MathF.PI / 180f);
        var tarscale = ril.scale.AsFloat();
        return position.IsEqualApprox(tarpos) && rotation.IsEqualApprox(tarrot) && Mathf.IsEqualApprox(scale, tarscale);
    }

    protected override void OnFlash()
    {
        base.OnFlash();
        if (false == world.rilbucket.SeekRIL<RIL_SPATIAL>(actor, out var ril)) return;
        position = ril.position.ToVector3();
        rotation = Quaternion.FromEuler(ril.euler.ToVector3() * MathF.PI / 180f);
        scale = ril.scale.AsFloat();
        ready = true;
    }
}
