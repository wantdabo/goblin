using System;
using System.Collections.Generic;
using System.IO;
using MessagePack;

namespace Goblin.Gameplay.Projection;

/// <summary>
/// 网络传输 — 序列化 ObserverPacket 并通过网络发送
/// </summary>
public class NetworkTransport : IPropertyTransport
{
    /// <summary>
    /// 发送回调（注入网络层）
    /// </summary>
    public Action<byte[]> onsend { get; set; }

    /// <summary>
    /// 序列化并发送 ObserverPacket 数组
    /// </summary>
    public void Send(ObserverPacket[] packets)
    {
        if (null == packets || 0 == packets.Length) return;

        var list = new List<NetworkPacketData>();
        foreach (var p in packets)
        {
            list.Add(new NetworkPacketData
            {
                actor = p.actor,
                behaviorinfotype = p.behaviorinfotype?.FullName ?? string.Empty,
                fieldmask = p.fieldmask,
                frame = p.frame,
                values = p.values,
            });
        }

        var bytes = MessagePackSerializer.Serialize(list);
        onsend?.Invoke(bytes);
    }
}

/// <summary>
/// 可序列化的数据包结构
/// </summary>
[MessagePackObject]
public class NetworkPacketData
{
    /// <summary>
    /// ActorID
    /// </summary>
    [Key(0)]
    public ulong actor { get; set; }

    /// <summary>
    /// BehaviorInfo 类型名（用于反序列化端映射 Component）
    /// </summary>
    [Key(1)]
    public string behaviorinfotype { get; set; }

    /// <summary>
    /// 字段掩码
    /// </summary>
    [Key(2)]
    public ulong fieldmask { get; set; }

    /// <summary>
    /// Logic 帧号
    /// </summary>
    [Key(3)]
    public long frame { get; set; }

    /// <summary>
    /// 字段值数组
    /// </summary>
    [Key(4)]
    public object[] values { get; set; }
}

/// <summary>
/// 远程传输 — 反序列化接收端，将网络数据包推入 RenderWorld
/// </summary>
public class RemoteTransport
{
    /// <summary>
    /// 表现世界
    /// </summary>
    public RenderWorld renderworld { get; set; }

    /// <summary>
    /// 接收并反序列化网络数据
    /// </summary>
    public void Receive(byte[] data)
    {
        if (null == renderworld || null == data || 0 == data.Length) return;

        var list = MessagePackSerializer.Deserialize<List<NetworkPacketData>>(data);
        if (null == list) return;

        var packets = new ObserverPacket[list.Count];
        for (var i = 0; i < list.Count; i++)
        {
            var d = list[i];
            packets[i] = new ObserverPacket
            {
                actor = d.actor,
                behaviorinfotype = Type.GetType(d.behaviorinfotype),
                fieldmask = d.fieldmask,
                frame = d.frame,
                values = d.values,
            };
        }

        renderworld.ApplyPackets(packets);
    }
}
