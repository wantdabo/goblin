# 投影系统审计报告

> 审计范围：`godot/Scripts/Goblin/Gameplay/Projection` + `godot/Scripts/Goblin/Gameplay/Render`
> 审计日期：2026-07-27

## 架构概览

```
Logic层 ProjectorSystem
        ↓ (ProjectorPacket[])
ProjectionPipeline
        ↓ (按 Observer × Crop规则链裁剪)
        ↓ (ObserverPacket[])
   Transport (LocalTransport → Mirror)
        ↓
   Render层 Mirror → Component 属性更新 → Godot渲染
```

整体设计遵循 **数据推送 + 规则链裁剪 + 传输抽象** 的模式。
其中 `IProjectionRule.IsActive` 决定是否需要规则裁剪，`IsActive=false` 时跳过性能开销。

---

## 发现的问题

### 1. [严重] ObserverPacket 内存泄漏（Transport 路径）

**位置**：`ProjectionPipeline.cs` Process 方法

**描述**：`Crop.Process` 中通过 `ObjectPool.Ensure` 取出的 ObserverPacket 实例，在 Transport 模式下从不归还对象池。

`transport.Send()` 之后引用了 `packetcache`（通过 `observerpackets` getter），但紧接着 `observerpackets = Array.Empty<ObserverPacket>()` 将 `packetcache` 重定向到一个空数组，**原来的 ObserverPacket 数组引用丢失，无法回收**。

`RecyclePacketCache()` 在下帧回收，但它看到的是已被替换的空数组。`LocalTransport.Send` 和 `NetworkTransport.Send` 也没有回收 ObserverPacket。

**修复方向**：在 `transport.Send()` 之后立即回收，或在 Transport 接口中约定消费方负责回收。

---

### 2. [严重] FrequencyRule.lastpushtable 无限增长

**位置**：`Rules/FrequencyRule.cs`

**描述**：`FrequencyRule.Cleanup(long minFrame)` 已实现但**从未被调用**。一旦 `FrequencyRule.Add()` 被调用注册了推送间隔，`lastpushtable` 会按 `(actor, type, fieldIndex)` 三元组无限增长。需要定期调用 `Cleanup` 清理已销毁实体的条目。

---

### 3. [中等] PermissionRule 和 FrequencyRule 规则表未注册

**位置**：`Sys/Gameplay/GameplayProxy.cs`

**描述**：`ObserverFactory` 正确创建了规则实例并加入链，但之后没有调用 `PermissionRule.Add()` 和 `FrequencyRule.Add()` 来填充权限表和频率表。当前二者都是空表 → fail-open 全放行，这两个规则实际上没生效。

---

### 4. [中等] VisibilityRule 逻辑缺陷

**位置**：`Sys/Gameplay/GameplayProxy.cs` 第 112-113 行

**描述**：`visibilitylookup` 被绑定到 `mirror.HasActor`：

```csharp
else if (rule is VisibilityRule vis)
    vis.visibilitylookup = mirror.HasActor;
```

`HasActor` 检查的是"Mirror 中是否存在该 Actor 的数据"，**不是真正的可见性概念**（如隐身、战争迷雾等）。当前逻辑：
- Actor 不在 Mirror → `HasActor` 返回 `false` → VisibilityRule 放行（fail-open，首帧数据流入）
- Actor 在 Mirror → `HasActor` 返回 `true` → VisibilityRule 也放行

这意味着 **VisibilityRule 永远不会裁剪任何数据**。可见性查询需要一个独立的、反映游戏可见性状态（而非数据存在性）的查询函数。

---

### 5. [中等] NetworkTransport 序列化路径有 GC 分配

**位置**：`Transport/NetworkTransport.cs` Send 方法

**描述**：每条 ObserverPacket 都 new 一个新对象：

```csharp
list.Add(new NetworkPacketData
{
    actor = p.actor,
    behaviorinfotype = p.behaviorinfotype?.FullName ?? string.Empty,
    fieldmask = p.fieldmask,
    frame = p.frame,
    values = ValueSerializer.SerializeValues(p.values ?? Array.Empty<object>()),
});
```

`ValueSerializer.SerializeValues` 中每条值 `new SerializedValue` + `new long[]` 也有分配。高频网络同步场景下这些分配会触发频繁 GC。建议使用对象池或结构体列化。

---

### 6. [低] volatile 语义不完整

**位置**：`ProjectionPipeline.cs`

```csharp
private volatile ObserverPacket[] packetcache;
public ObserverPacket[] observerpackets { get => packetcache; private set => packetcache = value; }
```

`volatile` 只保证引用读写的原子可见性，**不保护数组元素**。如果 Process 在逻辑线程、ApplyProjection 在渲染线程，跨线程读取数组元素存在数据竞争。当前单线程模式下无害。

---

### 7. [低] ObserverPacket 回收路径不统一

- **无 Transport 路径**：`ApplyProjection()` 读取 → 下帧 `RecyclePacketCache()` 回收
- **有 Transport 路径**：`transport.Send()` 消费 → 引用丢失（同问题 1）

两个消费路径的生命周期管理不一致，容易在未来引入 bug。

---

### 8. [建议] Crop.Process 双循环复杂度

**位置**：`Rules/Crop.cs`

N 个 Packet × M 个 Observer = O(N×M)。在大量实体 + 多观察者场景下是性能瓶颈，但当前单 Player 模式下无影响。

---

## 总结

| 严重度 | 问题 | 影响 |
|--------|------|------|
| 严重 | ObserverPacket 池化对象在 Transport 路径泄露 | 每帧内存增长 |
| 严重 | FrequencyRule.lastpushtable 无清理 | 潜在内存泄漏 |
| 中等 | PermissionRule / FrequencyRule 规则表为空 | 规则未生效 |
| 中等 | VisibilityRule 绑定 HasActor | 可见性裁剪无效 |
| 中等 | NetworkTransport 序列化路径 GC 分配 | 网络模式性能 |
| 低 | volatile 不保护数组内容 | 多线程数据竞争 |
| 低 | ObserverPacket 回收路径不统一 | 维护风险 |
