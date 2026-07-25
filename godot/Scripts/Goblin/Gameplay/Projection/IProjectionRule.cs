namespace Goblin.Gameplay.Projection;

/// <summary>
/// 投影裁剪规则接口
/// 规则链中每个规则接收上层的 (fieldmask, values)，返回修剪后的 mask
/// </summary>
public interface IProjectionRule
{
    /// <summary>
    /// 裁剪一个 (actor, fieldmask) 组合
    /// </summary>
    /// <param name="packet">原始数据包</param>
    /// <param name="observer">当前观察者</param>
    /// <param name="currentmask">上层规则修剪后的 fieldmask</param>
    /// <returns>修剪后的 fieldmask，0 表示整条丢弃</returns>
    ulong Filter(ProjectorPacket packet, Observer observer, ulong currentmask);
}
