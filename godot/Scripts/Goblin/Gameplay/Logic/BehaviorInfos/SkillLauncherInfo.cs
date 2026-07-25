using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 技能释放器信息
/// </summary>
public partial class SkillLauncherInfo : BehaviorInfo
{
    /// <summary>
    /// 正在进行的技能 ID
    /// </summary>
    public uint skill { get; set; }
    /// <summary>
    /// 当前技能生成的 Magic ActorID（0 表示无）
    /// </summary>
    public ulong magicid { get; set; }
    /// <summary>
    /// 是否有技能在释放中
    /// </summary>
    public bool casting { get; set; }
}
