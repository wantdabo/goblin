# Snapshot / Restore 机制与投影脏标记设计说明

> 状态：`Design`
>
> 文档日期：2026-07-28
> 相关代码：`Stage.Snapshot()` / `Stage.Restore()` (Stage.cs)、`ProjectorSystem` (ProjectorSystem.cs)、`IProjectable` (IProjectable.cs)。具体行为以当前源码为准。

---

## 1. 概述

Snapshot / Restore 是帧同步中快照回滚的核心机制。`Stage.Snapshot()` 在每帧 Tick 结束后对 `StageInfo` 做全量深拷贝（递归 Clone 所有 BehaviorInfo 树），`Stage.Restore()` 在收到服务端追帧指令时用 Clone 出的快照替换当前 info，并重建所有 Behavior 实例。

投影系统（ProjectorSystem）在每帧 EndTick 自检所有 BehaviorInfo 的 `projectdirtymask`，将脏字段打包成 `ProjectorPacket[]` 推送给 Render 层 Component。`projectdirtymask` 在消费后被清零，期待下帧属性变更重新置位。

---

## 2. 全量 Clone 数据流

```
帧 N Tick 结束
    │
    ├── 1️⃣ ProjectorSystem.OnEndTick()
    │       ├── 遍历 behaviorinfodict
    │       ├── 检查 IProjectable.projectdirtymask != 0
    │       ├── 收集脏字段 → ProjectorPacket[]
    │       └── proj.projectdirtymask = 0;  ← 清零！
    │
    └── 2️⃣ Stage.Snapshot()
            └── info.Clone()
                └── StageInfo.SGClone()
                    ├── 拷贝值类型字段（state、frame、elapsed 等）
                    ├── actors.Clone()（GBLList<ulong>，值类型轻量）
                    ├── behaviortypes.Clone()（Type 非 IGBL，浅拷贝轻量）
                    └── behaviorinfos.Clone()  ← 重！
                        └── GBLDict<Type, GBLList<BehaviorInfo>>.Clone()
                            └── 每个 BehaviorInfo.Clone() → SGClone()
                                ├── SpatialInfo（位置、朝向等）
                                ├── StateMachineInfo（states、transitions）
                                ├── FacadeInfo（effectdict、animslots）
                                └── ...
                                └── 所有字段 projectdirtymask == 0

帧 N+5 收到追帧指令
    │
    └── 3️⃣ Stage.Restore()
            ├── info.Reset() + 回池
            ├── info = snapshot.Clone() as StageInfo;  ← 又一次全量深拷贝
            │                                           └── projectdirtymask 仍为 0
            ├── 重建 cache.behaviorinfodict
            ├── AddBehavior 重建 Behavior 实例
            │   └── OnAddBindingInfo → SeekBehaviorInfo 命中已存在 Info
            │       └── 不触发 AddBehaviorInfo → 不调用 MarkProjectableDirty
            │
            └── 🔧 修复点：遍历 info.behaviorinfos，调用 MarkProjectableDirty
                    └── IProjectable.MarkAllDirty() → projectdirtymask 全 1

下帧 ProjectorSystem.OnEndTick()
    └── projectdirtymask != 0 → 全量打包 ProjectorPacket → Component 收到更新
```

---

## 3. projectdirtymask 生命周期

```
                    ┌──────────────────────────────┐
                    │  SG 生成属性 setter 写入位标记  │
                    │  proj.projectdirtymask |= bit  │
                    └──────────┬───────────────────┘
                               │
                               ▼
              ┌────────────────────────────────┐
              │  ProjectorSystem.OnEndTick()   │
              │  消费 mask → ProjectorPacket   │
              │  proj.projectdirtymask = 0     │
              └──────────┬─────────────────────┘
                         │
            ┌────────────┴────────────┐
            ▼                         ▼
    正常下一帧                    快照回滚路径
    属性再次变更               Clone 后 mask 全 0
    setter 重新置位            Restore 不重新置位
            │                         │
            ▼                         ▼
    OnEndTick 正常收集           OnEndTick 静默（❌ 失联）
                                     │
                                     ▼
                              🔧 MarkAllDirty 修复
                              全量投影 Component 更新
```

## 4. 根因：Restore 后 projectdirtymask 不置位

Restore 流程中，三步导致失联：

| 步骤 | 代码位置 | 行为 | 后果 |
|------|---------|------|------|
| Clone 快照 | `Stage.cs:301` | Snapshot 时 info 的 projectdirtymask 已被 ProjectorSystem 清零 | 快照内 mask 全 0 |
| Clone 恢复 | `Stage.cs:320` | `info = snapshot.Clone()` 再 Clone 一份 | 新 info 的 mask 仍全 0 |
| AddBehavior 重建 | `Stage.cs:340` | 因 BehaviorInfo 已由 322-332 行写入 behaviorinfodict，`SeekBehaviorInfo` 命中、不触发 `AddBehaviorInfo` | `MarkProjectableDirty` 不被调用 |

**Result**：下帧 ProjectorSystem 自检 `if (0 == mask)` 全部跳过，不发送任何 ProjectorPacket，Component 不更新。

## 5. 修复方案

在 `Stage.Restore()` 第 332 行（重建 behaviorinfodict 完成）与第 334 行（AddBehavior 重建）之间，新增遍历：

```csharp
// 快照回滚后标记全部投影字段为脏（首帧全量同步）
foreach (var behaviorinfos in info.behaviorinfos.Values)
{
    foreach (var behaviorinfo in behaviorinfos) MarkProjectableDirty(behaviorinfo);
}
```

修复点选择依据：
- `behaviorinfodict` 已重建完毕，`MarkProjectableDirty` 仅操作 info 对象本身，不依赖索引；
- 放在 `AddBehavior` 之前语义清晰——先备好数据（Info），再建行为（Behavior）；
- 复用既有 `MarkProjectableDirty(BehaviorInfo)`（内部 `if (info is IProjectable proj) proj.MarkAllDirty()`），与 `AddBehaviorInfo` 首帧行为一致。

## 6. MarkAllDirty 在快照回滚中的语义

`IProjectable.MarkAllDirty()` 注释明确："标记全部投影字段为脏（新对象首帧全量同步）"。在快照回滚上下文中：

- **不是**"新对象"，而是"恢复后的快照副本"；
- 语义等价：当前 info 已是全新的 Clone 对象，对投影系统而言就是"新对象"，需要全量同步；
- `SetProjectValues()` 注释标为"Phase 4 快照回滚时使用"，与 `MarkAllDirty()` 配合：前者从 object[] 静默设置值（不触发脏标记），后者标记全脏通知全量投影；
- 本修复属于 Phase 4 快照回滚投影链路的一部分。

## 7. 帧同步 vs 状态同步

| 维度 | 帧同步（Lockstep） | 状态同步（State Sync） |
|------|-------------------|----------------------|
| 全量 Clone 开销 | 可接受（仅回滚时触发，罕见） | 爆炸（频繁状态广播） |
| 快照回滚需求 | 需要 Snapshot/Restore | 不需要快照回滚 |
| 当前结论 | 全量 Clone 保留 | 无影响（不触发 Restore） |

当前架构焦点在帧同步，状态同步不在短期路线图中，故全量 Clone 不做优化。后续若接入状态同步，需将 Clone 替换为差量序列化。

## 8. 修复边界与已知局限

- **仅在 Restore 路径生效**：正常帧 Tick 中的属性变更通过 SG 生成 setter 的 `projectdirtymask |= bit` 机制正常工作，不受本修复影响。
- **全量投影开销**：Restore 后下一帧所有 IProjectable 的 BehaviorInfo 全量打包为 ProjectorPacket，与 Actor 首次 AddBehaviorInfo 行为一致。数百 Actor 场景下 μs 级，可接受。
- **不解决 Clone 本身的开销**：本修复不碰 Snapshot 的 Clone 逻辑，Clone 仍然是深拷贝整棵 BehaviorInfo 树。
- **与 SetProjectValues 的关系**：本修复使用 MarkAllDirty（标记脏 + 下帧 ProjectorSystem 自检收集），未使用 SetProjectValues（静默设置值 + 不触发投影）。SetProjectValues 设计用于 Phase 4 快照恢复时通过投影反向写值，当前链路未使用，留作后续接入点。
