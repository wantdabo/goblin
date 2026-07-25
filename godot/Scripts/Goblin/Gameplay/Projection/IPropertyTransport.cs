namespace Goblin.Gameplay.Projection;

/// <summary>
/// 属性传输接口 — 裁剪后的 ObserverPacket 发送目标
/// Phase 1：LocalTransport 直接写入 RenderWorld
/// Phase 2+：NetworkTransport 序列化后走网络
/// </summary>
public interface IPropertyTransport
{
    /// <summary>
    /// 发送裁剪后的观察者数据包
    /// </summary>
    /// <param name="packets">按 Observer 裁剪后的数据包数组</param>
    void Send(ObserverPacket[] packets);
}
