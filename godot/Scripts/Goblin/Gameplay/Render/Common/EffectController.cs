using Godot;

namespace Goblin.Gameplay.Render.Common;

/// <summary>
/// 特效控制器，挂在特效 Node3D 上
/// </summary>
public class EffectController
{
    public CpuParticles3D[] particles;
    public AnimationPlayer[] animplayers;
    public Node3D node;
    public float time { get; private set; }

    public void Reset() => time = 0;

    public void Simulate(float t)
    {
        time = t;
        if (particles != null)
            foreach (var p in particles)
                if (p != null) { p.Restart(); p.Emitting = true; }

        if (animplayers != null)
            foreach (var ap in animplayers)
                if (ap != null && ap.HasAnimation(ap.CurrentAnimation))
                {
                    var len = ap.CurrentAnimationLength;
                    if (len > 0) ap.Seek(t % len, true);
                }
    }
}