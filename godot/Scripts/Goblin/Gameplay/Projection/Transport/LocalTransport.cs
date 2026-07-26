using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Render.Core;

namespace Goblin.Gameplay.Projection.Transport;

/// <summary>
/// 本地传输 — 帧同步/单机模式，将裁剪后的数据包推送到 Mirror
/// 不序列化、不走网络，直接写入表现层
/// </summary>
public class LocalTransport : IPropertyTransport
{
    /// <summary>
    /// 数据镜像，Send 时直接 ApplyPackets
    /// </summary>
    public Mirror? mirror { get; set; }

    /// <summary>
    /// 发送裁剪后的观察者数据包到 Mirror
    /// </summary>
    /// <param name="packets">裁剪后的数据包数组</param>
    public void Send(ObserverPacket[] packets)
    {
        mirror?.ApplyPackets(packets);
    }
}
