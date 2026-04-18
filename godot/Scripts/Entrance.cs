using Goblin.Core;
using Godot;

/// <summary>
/// Entrance/游戏入口
/// </summary>
public partial class Entrance : Node
{
	public override void _Ready()
	{
		Godot.Engine.MaxFps = 0;
		CallDeferred(MethodName.Init);
	}

	private void Init()
	{
		Export.Init();
	}

	public override void _Process(double delta)
	{
		Export.Tick((float)delta);
	}

	public override void _PhysicsProcess(double delta)
	{
		Export.FixedTick((float)delta);
	}
}
