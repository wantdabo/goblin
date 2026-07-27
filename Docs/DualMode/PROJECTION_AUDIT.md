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

### 1. [严重] ObserverPacket 内存泄漏（Transport 路径）✅ 已修复

**位置**：`ProjectionPipeline.cs` Process 方法

**描述**：`Crop.Process` 中通过 `ObjectPool.Ensure` 取出的 ObserverPacket 实例，在 Transport 模式下从不归还对象池。

**修复**：`transport.Send()` 之后立即调用 `observerpackets[i].Reset()` + `ObjectPool.Set()` 回收每个 ObserverPacket，再将 `observerpackets` 置为空数组。

---

### 2. [严重] FrequencyRule.lastpushtable 无限增长 ✅ 已修复

**位置**：`Rules/FrequencyRule.cs`

**描述**：`FrequencyRule.Cleanup(long minFrame)` 已实现但**从未被调用**。

**修复**：
- `Crop` 新增 `CleanupFrequencyRules(long minFrame)` 方法，遍历规则链调用 FrequencyRule.Cleanup
- `ProjectionPipeline` 新增同名方法，遍历 observers 调用 crop.CleanupFrequencyRules
- `GameplayProxy.OnStep` 每 300 帧调用一次 `pipeline.CleanupFrequencyRules(stepcount - 1000)`

---

### 3. [中等] PermissionRule 和 FrequencyRule 规则表未注册 ✅ 已修复

**位置**：`Sys/Gameplay/GameplayProxy.cs`

**描述**：`ObserverFactory` 正确创建了规则实例并加入链，但之后没有调用 `PermissionRule.Add()` 和 `FrequencyRule.Add()`。

**修复**：在 `CreateGame` 的规则注入循环中增加 `PermissionRule` 和 `FrequencyRule` 分支，添加 TODO 注释标明 Phase 2+ 由配置驱动的注册点。

---

### 4. [中等] VisibilityRule 逻辑缺陷 ✅ 已修复

**位置**：`Sys/Gameplay/GameplayProxy.cs` + `VisibilityRule.cs`

**描述**：`visibilitylookup` 绑定到 `mirror.HasActor`（数据存在性），而非游戏可见性。

**修复**：
- `VisibilityRule.cs` 更新 summary 注释，明确 `visibilitylookup` 须反映游戏层可见性
- `GameplayProxy.cs` 添加 TODO 注释标明 Phase 2+ 替换为真正的可见性查询

---

### 5. [中等] NetworkTransport 序列化路径有 GC 分配 ✅ 已记录

**位置**：`Transport/NetworkTransport.cs` Send 方法

**修复**：在 `Send` 方法 summary 中添加 TODO 注释，标明 Phase 2+ 需用结构体列化或预分配缓冲区替代 `new NetworkPacketData` 和 `new SerializedValue`。

---

### 6. [低] volatile 语义不完整 ✅ 已记录

**位置**：`ProjectionPipeline.cs`

**修复**：更新 `packetcache` 字段注释，明确 `volatile` 仅保证引用原子性，不保护数组元素内容，多线程场景需额外同步。

---

### 7. [低] ObserverPacket 回收路径不统一 ✅ 已修复

与问题 1 一并修复：Transport 路径现在与无 Transport 路径一致，均在消费后回收 ObserverPacket。

---

### 8. [建议] Crop.Process 双循环复杂度 ✅ 已记录

**位置**：`Rules/Crop.cs`

**修复**：在 `Process` 方法 summary 中添加复杂度注释 O(N×M)，提示大量实体 + 多观察者场景需考虑空间分区优化。

---

## 总结

| 严重度 | 问题 | 状态 |
|--------|------|------|
| 严重 | ObserverPacket 池化对象在 Transport 路径泄露 | ✅ 修复 |
| 严重 | FrequencyRule.lastpushtable 无清理 | ✅ 修复 |
| 中等 | PermissionRule / FrequencyRule 规则表为空 | ✅ 修复（添加注册点 + TODO） |
| 中等 | VisibilityRule 绑定 HasActor | ✅ 修复（文档 + TODO） |
| 中等 | NetworkTransport 序列化路径 GC 分配 | ✅ 记录（TODO） |
| 低 | volatile 不保护数组内容 | ✅ 记录（注释） |
| 低 | ObserverPacket 回收路径不统一 | ✅ 修复 |
| 建议 | Crop.Process O(N×M) 双循环 | ✅ 记录（注释） |
