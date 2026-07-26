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
/// 管线持有 Observers、Crop 规则链、Transport，在 Process 中一次串联
/// </summary>
public partial class ProjectionPipeline : IGBL
{
    /// <summary>
    /// 当前激活的观察者列表（Phase 1 默认单 Player）
    /// </summary>
    public List<Observer> observers { get; private set; }

    /// <summary>
    /// 裁剪规则链（Phase 1 默认挂 GodRule）
    /// </summary>
    public Crop crop { get; private set; }

    /// <summary>
    /// 传输层（T1.8 接入 LocalTransport）
    /// </summary>
    public IPropertyTransport transport { get; set; }

    /// <summary>
    /// 本帧裁剪后的 ObserverPacket 数组（volatile 保证多线程可见性）
    /// </summary>
    private volatile ObserverPacket[] packetcache;
    public ObserverPacket[] observerpackets { get => packetcache; private set => packetcache = value; }

    public ProjectionPipeline()
    {
        observers = new List<Observer>();
        crop = new Crop();
        packetcache = Array.Empty<ObserverPacket>();
    }

    /// <summary>
    /// 执行投影管线：原始包 → 按 Observer 裁剪 → 传输
    /// 空包直接返回，不覆盖上一帧的有效 observerpackets
    /// </summary>
    /// <param name="packets">ProjectorSystem 产出的原始投影包</param>
    public void Process(ProjectorPacket[] packets)
    {
        if (0 == packets.Length) return;

        packetcache = Crop.Process(packets, observers);

        // T1.8：裁剪后的数据包交给 Transport 发送
        if (null != transport && 0 < observerpackets.Length)
        {
            transport.Send(observerpackets);
        }
    }
}
