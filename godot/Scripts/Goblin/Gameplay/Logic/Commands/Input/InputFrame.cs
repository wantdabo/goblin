namespace Goblin.Gameplay.Logic.Commands.Input;

/// <summary>
/// 帧输入基类 — Gamepad 存储，OnEndTick Reset 后归还池
/// </summary>
public abstract class InputFrame
{
    public void Ready()
    {
        OnReady();
    }

    public void Reset()
    {
        OnReset();
    }

    protected virtual void OnReady()
    {
    }

    protected virtual void OnReset()
    {
    }
}
