using System;
using System.Collections.Generic;
using System.IO;
using Goblin.Common;
using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Render.Core;
using MessagePack;

namespace Goblin.Gameplay.Projection.Transport;

/// <summary>
/// 网络传输 — 序列化 ObserverPacket 并通过网络发送
/// </summary>
public class NetworkTransport : IPropertyTransport
{
    /// <summary>
    /// 对象池 key（唯一常量）
    /// </summary>
    private const string PACKETDATA_LIST_KEY = "NETWORK_PACKETDATA_LIST";

    /// <summary>
    /// 类型注册表，序列化用 FullName 为键，反序列化端通过此表解析
    /// </summary>
    internal static readonly Dictionary<string, Type> typeregistry = new();

    /// <summary>
    /// 注册 BehaviorInfo 类型（启动时调用）
    /// </summary>
    public static void RegisterType(Type type)
    {
        if (null == type) return;
        typeregistry[type.FullName!] = type;
    }

    /// <summary>
    /// 发送回调（注入网络层）
    /// </summary>
    public Action<byte[]>? onsend { get; set; }

    /// <summary>
    /// 序列化并发送 ObserverPacket 数组
    /// TODO: Phase 2+ 每帧 new NetworkPacketData 和 SerializedValue 产生 GC 压力
    /// 高频同步场景需用结构体列化或预分配缓冲区替代 new 分配
    /// </summary>
    public void Send(ObserverPacket[] packets)
    {
        if (null == packets || 0 == packets.Length) return;

        // 从对象池取 List，清空复用
        var list = ObjectPool.Ensure<List<NetworkPacketData>>(PACKETDATA_LIST_KEY);
        list.Clear();
        foreach (var p in packets)
        {
            list.Add(new NetworkPacketData
            {
                actor = p.actor,
                behaviorinfotype = p.behaviorinfotype?.FullName ?? string.Empty,
                fieldmask = p.fieldmask,
                frame = p.frame,
                values = ValueSerializer.SerializeValues(p.values ?? Array.Empty<object>()),
            });
        }

        var bytes = MessagePackSerializer.Serialize(list);
        onsend?.Invoke(bytes);

        // 归还 List 到对象池
        ObjectPool.Set(list, PACKETDATA_LIST_KEY);
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
    public string behaviorinfotype { get; set; } = string.Empty;

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
    public List<SerializedValue> values { get; set; } = new List<SerializedValue>();
}

/// <summary>
/// 远程传输 — 反序列化接收端，将网络数据包推入 Mirror
/// </summary>
public class RemoteTransport
{
    /// <summary>
    /// 数据镜像
    /// </summary>
    public Mirror? mirror { get; set; }

    /// <summary>
    /// 接收并反序列化网络数据
    /// </summary>
    public void Receive(byte[] data)
    {
        if (null == mirror || null == data || 0 == data.Length) return;

        var list = MessagePackSerializer.Deserialize<List<NetworkPacketData>>(data);
        if (null == list) return;

        var packets = new ObserverPacket[list.Count];
        for (var i = 0; i < list.Count; i++)
        {
            var d = list[i];
            // 从对象池取 ObserverPacket 实例
            var p = ObjectPool.Ensure<ObserverPacket>(ObserverPacket.POOL_KEY);
            p.actor = d.actor;
            p.behaviorinfotype = NetworkTransport.typeregistry.TryGetValue(d.behaviorinfotype, out var t) ? t : null;
            p.fieldmask = d.fieldmask;
            p.frame = d.frame;
            p.values = ValueSerializer.DeserializeValues(d.values);
            packets[i] = p;
        }

        mirror.ApplyPackets(packets);

        // 归还 ObserverPacket 实例到对象池
        foreach (var p in packets)
        {
            if (null == p) continue;
            p.Reset();
            ObjectPool.Set(p, ObserverPacket.POOL_KEY);
        }
    }
}
