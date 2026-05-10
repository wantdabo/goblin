using Goblin.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL;
using Goblin.Gameplay.Render.Core;
using Godot;

namespace Goblin.Gameplay.Render.Agents;

/// <summary>
/// 基元动画代理, 无 AnimationPlayer 依赖
/// 按 RIL_FACADE_ANIMATION.animstate 在 PrimitiveMeshAgent.meshinstance / material 上做程序化动画
/// IDLE(3): 静止; MOVE(4): 上下浮 0.05m @ 2Hz; BEHIT(8): albedo 闪红 0.1s; DEATH(2): scale.y → 0 over 0.5s;
/// CASTING(7): z 轴瞬刺 scale.z 1.0→1.3→1.0, 0.15s; BORN(1): 与 IDLE 一致
/// </summary>
public class PrimitiveAnimAgent : Agent
{
    private PrimitiveMeshAgent meshagent { get; set; }
    private byte animstate { get; set; }
    private float elapsed { get; set; }

    private Vector3 basemeshpos { get; set; }
    private Color basecolor { get; set; }
    private bool hasbase { get; set; }

    protected override void OnReady()
    {
        meshagent = null;
        animstate = STATE_DEFINE.IDLE;
        elapsed = 0f;
        basemeshpos = Vector3.Zero;
        basecolor = new Color(1, 1, 1, 1);
        hasbase = false;
        WatchRIL<RIL_FACADE_ANIMATION>(OnRILAnimation);
    }

    protected override void OnReset()
    {
        RestoreBase();
        meshagent = null;
        animstate = STATE_DEFINE.IDLE;
        elapsed = 0f;
        hasbase = false;
    }

    private void OnRILAnimation(RIL_FACADE_ANIMATION ril)
    {
        if (animstate != ril.animstate)
        {
            RestoreBase();
            animstate = ril.animstate;
            elapsed = ril.animelapsed * Config.Int2Float;
        }
        else
        {
            elapsed = ril.animelapsed * Config.Int2Float;
        }
    }

    protected override void OnChase(float tick, float timescale)
    {
        base.OnChase(tick, timescale);

        if (meshagent == null || meshagent.actor != actor) meshagent = world.GetAgent<PrimitiveMeshAgent>(actor);
        if (null == meshagent || null == meshagent.meshinstance || null == meshagent.material) return;

        if (false == hasbase)
        {
            basemeshpos = meshagent.meshinstance.Position;
            basecolor = meshagent.material.AlbedoColor;
            hasbase = true;
        }

        elapsed += tick * timescale;

        switch (animstate)
        {
            case STATE_DEFINE.MOVE: PlayMove(); break;
            case STATE_DEFINE.BEHIT: PlayBeHit(); break;
            case STATE_DEFINE.DEATH: PlayDeath(); break;
            case STATE_DEFINE.CASTING: PlayCasting(); break;
            case STATE_DEFINE.BORN:
            case STATE_DEFINE.IDLE:
            default: PlayIdle(); break;
        }
    }

    private void PlayIdle()
    {
        meshagent.meshinstance.Position = basemeshpos;
        meshagent.meshinstance.Scale = Vector3.One;
        meshagent.material.AlbedoColor = basecolor;
    }

    private void PlayMove()
    {
        float offset = Mathf.Sin(elapsed * Mathf.Pi * 2f * 2f) * 0.05f;
        meshagent.meshinstance.Position = basemeshpos + new Vector3(0, offset, 0);
        meshagent.meshinstance.Scale = Vector3.One;
        meshagent.material.AlbedoColor = basecolor;
    }

    private void PlayBeHit()
    {
        meshagent.meshinstance.Position = basemeshpos;
        meshagent.meshinstance.Scale = Vector3.One;
        if (elapsed < 0.1f) meshagent.material.AlbedoColor = new Color(1f, 0.2f, 0.2f, basecolor.A);
        else meshagent.material.AlbedoColor = basecolor;
    }

    private void PlayDeath()
    {
        const float duration = 0.5f;
        float t = Mathf.Clamp(elapsed / duration, 0f, 1f);
        meshagent.meshinstance.Position = basemeshpos;
        meshagent.meshinstance.Scale = new Vector3(1f, 1f - t, 1f);
        meshagent.material.AlbedoColor = basecolor;
    }

    private void PlayCasting()
    {
        const float duration = 0.15f;
        float stretch = 1f;
        if (elapsed < duration)
        {
            float t = elapsed / duration;
            // 三角波 0 → 1 → 0, 峰值 0.3
            float tri = (t < 0.5f) ? (t * 2f) : (1f - (t - 0.5f) * 2f);
            stretch = 1f + tri * 0.3f;
        }
        meshagent.meshinstance.Position = basemeshpos;
        meshagent.meshinstance.Scale = new Vector3(1f, 1f, stretch);
        meshagent.material.AlbedoColor = basecolor;
    }

    private void RestoreBase()
    {
        if (false == hasbase) return;
        if (null == meshagent || null == meshagent.meshinstance || null == meshagent.material) return;
        meshagent.meshinstance.Position = basemeshpos;
        meshagent.meshinstance.Scale = Vector3.One;
        meshagent.material.AlbedoColor = basecolor;
    }
}
