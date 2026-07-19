namespace Goblin.Gameplay.Logic.Common.Defines;

/// <summary>
/// 音效播放模式
/// </summary>
public enum SoundMode : byte
{
    /// <summary>
    /// 一次性播放，播完自动回收
    /// </summary>
    OneShot = 0,
    /// <summary>
    /// 循环播放，需要显式 Stop
    /// </summary>
    Loop = 1,
    /// <summary>
    /// 停止当前 Actor 上指定 soundid 的循环音效（per-instance，非全局同名）
    /// </summary>
    Stop = 2,
}
