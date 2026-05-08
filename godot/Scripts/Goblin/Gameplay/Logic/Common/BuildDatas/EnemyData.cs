using Kowtow.Math;
using MessagePack;

namespace Goblin.Gameplay.Logic.Common.BuildDatas;

/// <summary>
/// 敌人数据
/// </summary>
[MessagePackObject(true)]
public struct EnemyData
{
    /// <summary>
    /// 敌人配置 ID（对应 EnemyInfo.Id）
    /// </summary>
    public int enemy { get; set; }
    /// <summary>
    /// 位置
    /// </summary>
    public IntVector3 position { get; set; }
    /// <summary>
    /// 旋转
    /// </summary>
    public IntVector3 euler { get; set; }
    /// <summary>
    /// 缩放
    /// </summary>
    public int scale { get; set; }
}
