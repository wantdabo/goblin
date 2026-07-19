namespace Goblin.Common.Sounds;

/// <summary>
/// 音效分类
/// </summary>
public enum SoundCategory : byte
{
    /// <summary>
    /// 玩法音效（3D 空间）
    /// </summary>
    SFX = 0,
    /// <summary>
    /// 背景音乐（2D）
    /// </summary>
    BGM = 1,
    /// <summary>
    /// UI 音效（2D）
    /// </summary>
    UI = 2,
}

/// <summary>
/// 音效配置（soundid → 资源映射）
///
/// soundid 分段（纯配置管理约定，代码不校验）：
///   0000000 - 9999999   SFX
///   10000000 - 19999999 BGM
///   20000000 - 29999999 UI
/// </summary>
public class SoundConfig
{
    /// <summary>
    /// 唯一 ID（按分段分配）
    /// </summary>
    public uint soundid { get; set; }
    /// <summary>
    /// 资源路径（相对于 Location.soundpath）
    /// </summary>
    public string res { get; set; }
    /// <summary>
    /// 分类
    /// </summary>
    public SoundCategory category { get; set; }
    /// <summary>
    /// 默认音量（备用）
    /// </summary>
    public float defaultvolume { get; set; } = 1.0f;
}
