using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Core;
using Godot;
using System;

namespace Goblin.Gameplay.Render.Cameras;

/// <summary>
/// 眼睛/镜头，替代 Unity Cinemachine FreeLook
/// </summary>
public class Eyes : Comp
{
    private World world;
    public Camera3D camera { get; private set; }

    private float yaw;
    private float pitch = 20f;
    private float distance = 5f;
    private const float pitchMin = -10f, pitchMax = 60f;
    private const float distMin = 2f, distMax = 10f;

    protected override void OnCreate()
    {
        base.OnCreate();
        world.ticker.eventor.Listen<TickEvent>(OnTick);

        camera = new Camera3D { Name = "Camera3D" };
        var scene = Godot.Engine.GetMainLoop() as SceneTree;
        scene?.Root.AddChild(camera);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        world.ticker.eventor.UnListen<TickEvent>(OnTick);
        camera?.QueueFree();
    }

    public Eyes Initialize(World world)
    {
        this.world = world;
        return this;
    }

    private void OnTick(TickEvent e)
    {
        var node = world.GetAgent<NodeAgent>(world.self);
        if (null == node?.node) return;

        var target = node.node.Position + Vector3.Up * 1.7f;

        // 鼠标/摇杆旋转
        var look = engine.gdkit.GetLookInput();
        yaw += look.X * 120f * e.tick;
        pitch = Mathf.Clamp(pitch - look.Y * 80f * e.tick, pitchMin, pitchMax);

        // 滚轮缩放
        var scroll = engine.gdkit.GetScrollInput();
        distance = Mathf.Clamp(distance - scroll * 3f * e.tick, distMin, distMax);

        var yawRad = yaw * MathF.PI / 180f;
        var pitchRad = pitch * MathF.PI / 180f;
        var offset = new Vector3(
            MathF.Sin(yawRad) * MathF.Cos(pitchRad),
            MathF.Sin(pitchRad),
            MathF.Cos(yawRad) * MathF.Cos(pitchRad)
        ) * distance;

        camera.Position = target + offset;
        camera.LookAt(target, Vector3.Up);
    }
}