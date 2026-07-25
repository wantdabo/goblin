using Goblin.Gameplay.Logic.Common;
using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.Core;
using Kowtow.Math;

namespace Goblin.Gameplay.Logic.BehaviorInfos;

/// <summary>
/// 驱动信息
/// </summary>
public partial class TickerInfo : BehaviorInfo
{
    private FP mtimescale = FP.One;
    /// <summary>
    /// 时间缩放
    /// </summary>
    public FP timescale {
        get
        {
            return mtimescale;
        }
        set
        {
            mtimescale = FPMath.Clamp(value, 0, FP.MaxValue);
        }
    }

    protected override void OnReady()
    {
        OnReset();
    }

    // timescale Reset 值为 FP.One（非 default），SG Reset 设 default 后由此覆盖
    protected override void OnReset()
    {
        timescale = FP.One;
    }
}
