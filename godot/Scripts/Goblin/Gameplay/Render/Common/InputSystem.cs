using System.Collections.Generic;
using Goblin.Common;
using Goblin.Core;
using Goblin.Gameplay.Logic.Commands.Common;
using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Render.Core;
using Godot;
using Kowtow.Math;
using Config = Goblin.Common.Config;

namespace Goblin.Gameplay.Render.Common;

/// <summary>
/// 输入系统
/// </summary>
public class InputSystem : Comp
{
    private World world { get; set; }
    private readonly object @lock = new();
    private Dictionary<ushort, (bool press, IntVector2 dire)> inputdict { get; set; }
    private Queue<Command> cmdqueue { get; set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        world.ticker.eventor.Listen<TickEvent>(OnTick);
        inputdict = ObjectPool.Ensure<Dictionary<ushort, (bool, IntVector2)>>();
        cmdqueue = ObjectPool.Ensure<Queue<Command>>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        world.ticker.eventor.UnListen<TickEvent>(OnTick);
        inputdict.Clear(); ObjectPool.Set(inputdict);
        foreach (var cmd in cmdqueue) { cmd.Reset(); ObjectPool.Set(cmd); }
        cmdqueue.Clear(); ObjectPool.Set(cmdqueue);
    }

    public InputSystem Initialize(World world) { this.world = world; return this; }

    public (bool press, IntVector2 dire) GetInput(ushort type)
    {
        lock (@lock)
        {
            inputdict.TryGetValue(type, out var input);
            return input;
        }
    }

    public void SetInput(ushort type, bool press, IntVector2 dire)
    {
        lock (@lock)
        {
            inputdict.Remove(type);
            inputdict.Add(type, (press, dire));
        }
    }

    public bool TryDequeueCommand(out Command command) => cmdqueue.TryDequeue(out command);

    public void EnqueueCommand(Command command)
    {
        if (null != command) cmdqueue.Enqueue(command);
    }

    private void OnTick(TickEvent e)
    {
        var move = Input.GetVector("move_left", "move_right", "move_up", "move_down");

        var joystick = new Godot.Vector2(move.X, move.Y);
        if (joystick != Godot.Vector2.Zero && null != world.eyes?.camera)
        {
            var cam = world.eyes.camera;
            var forward = -cam.GlobalTransform.Basis.Z;
            var right = cam.GlobalTransform.Basis.X;
            forward.Y = 0; right.Y = 0;
            forward = forward.Normalized(); right = right.Normalized();
            var world3d = joystick.X * right - joystick.Y * forward;
            joystick = new Godot.Vector2(world3d.X, world3d.Z);
        }

        SetInput(INPUT_DEFINE.JOYSTICK, joystick != Godot.Vector2.Zero,
            new IntVector2((int)(joystick.X * Config.Float2Int), (int)(joystick.Y * Config.Float2Int)));

        var fire = Input.IsActionPressed("fire");
        SetInput(INPUT_DEFINE.BA, fire, new IntVector2(0, 0));
    }
}