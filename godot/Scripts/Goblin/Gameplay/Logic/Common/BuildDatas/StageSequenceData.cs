using MessagePack;

namespace Goblin.Gameplay.Logic.Common.BuildDatas;

/// <summary>
/// 关卡序列条件类型
/// </summary>
public enum StageSequenceCondition : byte
{
    /// <summary>
    /// 所有敌人死亡 → 胜利
    /// </summary>
    AllEnemiesDead = 1,
    /// <summary>
    /// 英雄死亡 → 失败
    /// </summary>
    HeroDead = 2,
}

/// <summary>
/// 关卡序列数据，随 StageData 传入，由 StageSequence Behavior 解析执行
/// </summary>
[MessagePackObject(true)]
public struct StageSequenceData
{
    /// <summary>
    /// 胜利条件
    /// </summary>
    public StageSequenceCondition win { get; set; }
    /// <summary>
    /// 失败条件
    /// </summary>
    public StageSequenceCondition lose { get; set; }
}
