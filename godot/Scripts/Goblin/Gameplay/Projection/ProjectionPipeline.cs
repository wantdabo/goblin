using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Projection.Rules;
using Goblin.Gameplay.Projection.Transport;

namespace Goblin.Gameplay.Projection;

/// <summary>
/// 投影管线 — 接收 ProjectorPacket[]，按 Observer 分叉执行 Crop 裁剪
/// ProjectorSystem 出包后，由外部（Director/Game）将原始包交给 Pipeline 处理
/// 管线持有 Observers、传输层，在 Process 中一次串联
/// </summary>
public partial class ProjectionPipeline : IGBL
{
    /// <summary>
    /// 当前激活的观察者列表（Phase 1 默认单 Player）
    /// </summary>
    public List<Observer> observers { get; private set; }

    /// <summary>
    /// 传输层（T1.8 接入 LocalTransport）
    /// </summary>
    public IPropertyTransport transport { get; set; }

    /// <summary>
    /// 本帧裁剪后的 ObserverPacket 数组
    /// 注意：volatile 仅保证引用读写原子性，不保护数组元素内容
    /// 多线程场景下读取数组元素需额外同步（Interlocked.Exchange 或 lock）
    /// </summary>
    private volatile ObserverPacket[] packetcache;
    public ObserverPacket[] observerpackets { get => packetcache; private set => packetcache = value; }

    public ProjectionPipeline()
    {
        observers = new List<Observer>();
        packetcache = Array.Empty<ObserverPacket>();
    }

    /// <summary>
    /// 执行投影管线：原始包 → 按 Observer 裁剪 → 传输
    /// 无数据时将 observerpackets 置空，避免主线程重复消费上帧脏数据
    /// </summary>
    /// <param name="packets">ProjectorSystem 产出的原始投影包</param>
    public void Process(ProjectorPacket[] packets)
    {
        if (null == packets || 0 == packets.Length)
        {
            RecyclePacketCache();
            observerpackets = Array.Empty<ObserverPacket>();
            return;
        }

        // 回收上帧 ObserverPacket 实例到对象池
        RecyclePacketCache();

        packetcache = Crop.Process(packets, observers);

        // 裁剪后的数据包交给 Transport 发送
        if (null != transport && 0 < observerpackets.Length)
        {
            transport.Send(observerpackets);
            // 回收 Transport 已消费的 ObserverPacket 实例，防止对象池泄露
            for (var i = 0; i < observerpackets.Length; i++)
            {
                if (null == observerpackets[i]) continue;
                observerpackets[i].Reset();
                ObjectPool.Set(observerpackets[i], ObserverPacket.POOL_KEY);
            }
            // 清空防止 ApplyProjection 双重 apply
            observerpackets = Array.Empty<ObserverPacket>();
        }
    }

    /// <summary>
    /// 回收当前帧缓存的 ObserverPacket 实例到对象池
    /// </summary>
    private void RecyclePacketCache()
    {
        if (null == packetcache) return;
        foreach (var p in packetcache)
        {
            if (null == p) continue;
            p.Reset();
            ObjectPool.Set(p, ObserverPacket.POOL_KEY);
        }
        packetcache = Array.Empty<ObserverPacket>();
    }

    /// <summary>
    /// 清理所有 Observer 的频率规则过期条目
    /// 定期调用以释放不再需要的实体记录，防止 lastpushtable 无限增长
    /// </summary>
    public void CleanupFrequencyRules(long minFrame)
    {
        foreach (var obs in observers)
        {
            obs.crop?.CleanupFrequencyRules(minFrame);
        }
    }

    /// <summary>
    /// 浅拷贝 — observers List 是共享引用，克隆后修改列表影响双方
    /// Clone 产物仅供池化回收使用，业务侧不应修改克隆实例
    /// </summary>
    public IGBL Clone()
    {
        return (IGBL)MemberwiseClone();
    }

    /// <summary>
    /// 重置为空状态（对象池回收用）
    /// </summary>
    public void Reset()
    {
        RecyclePacketCache();
        observers.Clear();
        packetcache = Array.Empty<ObserverPacket>();
    }
}
