namespace Goblin.Gameplay.Projection.Core;

/// <summary>
/// 投影裁剪规则接口 — 接收原始数据包和观察者，返回裁剪后的 fieldmask
/// </summary>
public interface IProjectionRule
{
    /// <summary>
    /// 裁剪过滤
    /// </summary>
    /// <param name="packet">原始投影数据包</param>
    /// <param name="observer">目标观察者</param>
    /// <param name="currentmask">当前字段掩码</param>
    /// <returns>裁剪后的字段掩码，0 表示丢弃</returns>
    ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask);
}
