namespace Goblin.Gameplay.Logic.Commands.Input;

/// <summary>
/// 按键操作
/// </summary>
public enum KeyAction
{
    /// <summary>
    /// 按下
    /// </summary>
    Press,
    /// <summary>
    /// 释放
    /// </summary>
    Release,
}

/// <summary>
/// 按键帧输入
/// </summary>
public class KeyFrame : InputFrame
{
    /// <summary>
    /// 按键 ID（INPUT_DEFINE.BA / BB / BC / BD）
    /// </summary>
    public ushort key { get; set; }
    /// <summary>
    /// 操作类型
    /// </summary>
    public KeyAction action { get; set; }

    protected override void OnReady()
    {
        key = 0;
        action = default;
    }

    protected override void OnReset()
    {
        key = 0;
        action = default;
    }
}
