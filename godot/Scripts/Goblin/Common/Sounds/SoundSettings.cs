namespace Goblin.Common.Sounds;

/// <summary>
/// 音效设置（音量/静音偏好）
/// </summary>
public class SoundSettings
{
    /// <summary>
    /// 主音量 0.0 - 1.0
    /// </summary>
    public float mastervolume { get; set; } = 1.0f;
    /// <summary>
    /// BGM 音量 0.0 - 1.0
    /// </summary>
    public float bgmvolume { get; set; } = 1.0f;
    /// <summary>
    /// SFX 音量 0.0 - 1.0
    /// </summary>
    public float sfxvolume { get; set; } = 1.0f;
    /// <summary>
    /// 主静音
    /// </summary>
    public bool mastermuted { get; set; } = false;
    /// <summary>
    /// BGM 静音
    /// </summary>
    public bool bgmmuted { get; set; } = false;
    /// <summary>
    /// SFX 静音
    /// </summary>
    public bool sfxmuted { get; set; } = false;
}
