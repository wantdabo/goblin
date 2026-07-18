using Kowtow.Math;

namespace Goblin.Gameplay.Logic.Commands.Input;

/// <summary>
/// 移动帧输入 — 摇杆方向
/// </summary>
public class MoveFrame : InputFrame
{
    public FPVector2 dire { get; set; }

    protected override void OnReady()
    {
        dire = default;
    }

    protected override void OnReset()
    {
        dire = default;
    }
}
