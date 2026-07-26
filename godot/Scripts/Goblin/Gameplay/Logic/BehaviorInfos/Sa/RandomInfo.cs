using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.BehaviorInfos.Sa;

/// <summary>
/// 随机信息
/// </summary>
public partial class RandomInfo : BehaviorInfo
{
    /// <summary>
    /// 乘数
    /// </summary>
    public long a { get; private set; }
    /// <summary>
    /// 增量
    /// </summary>
    public long c { get; private set; }
    /// <summary>
    /// 模数
    /// </summary>
    public long m { get; private set; }
    /// <summary>
    /// 随机种子
    /// </summary>
    public long seed { get; set; }
    /// <summary>
    /// 最新随机数
    /// </summary>
    public long current { get; set; }

    // a/c/m 默认值为 LCG 常量（非 default），首次创建时由此初始化
    // SG Reset 设 default 后由 OnReset 覆盖，但 OnReady 不会设置值类型字段
    protected override void OnReady()
    {
        base.OnReady();
        a = 1664525;
        c = 1013904223;
        m = 4294967296;
        seed = 0;
        current = 0;
    }

    // a/c/m Reset 值非 default（LCG 常量），SG Reset 设 default 后由此覆盖
    protected override void OnReset()
    {
        base.OnReset();
        a = 1664525;
        c = 1013904223;
        m = 4294967296;
        seed = 0;
        current = 0;
    }
}
