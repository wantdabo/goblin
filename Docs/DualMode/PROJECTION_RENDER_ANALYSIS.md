# Projection-Render 流程分析

> 分析日期：2026-07-26
> 范围：`godot/Scripts/Goblin/Gameplay/Projection/`、`godot/Scripts/Goblin/Gameplay/Render/`

---

## 一、整体流程

### 数据流（一帧）

```
Logic 层 BehaviorInfo（SG 生成 backing field + 脏标记）
   │  属性 setter 写入 projectdirtymask 位
   ▼
ProjectorSystem.OnEndTick（SA 上挂载）
   │  遍历 stage.cache.behaviorinfodict
   │  仅 IProjectable 且 mask != 0 的 Info
   │  → 取 mask、TakeProjectValues(mask) → ProjectorPacket[]
   ▼
GameplayProxy.OnStep → pipeline.Process(ps.packets)
   │  ProjectionPipeline.Process:
   │     observerpackets = Crop.Process(packets, observers, crop)
   │        foreach packet × observer → crop.Project(packet, obs)
   │           foreach rule in crop.rules: mask = rule.Filter(...)
   │           mask != 0 → new ObserverPacket { values = p.values, fieldmask = mask }
   │     if (transport != null) transport.Send(observerpackets)
   │  （GameplayProxy 不设 transport，留待主线程消费）
   ▼
GameplayView.OnLateTick（主线程）
   │  proxy.ApplyProjection()
   │     mirror.ApplyPackets(pipeline.observerpackets)
   ▼
Mirror.ApplyPackets
   │  eventframecache 去重 "{actor}_{infotype.Name}"
   │  Apply(actor, infotype, fieldmask, values)
   │     infotocomp[infotype] → comptype
   │     Activator.CreateInstance(comptype)（首次）
   │     applymap[comptype](comp, fieldmask, values)
   ▼
SG 生成的 {Component}Apply.ApplyTo
   │  按 bit 检查 fieldmask，从 values[vi++] 还原字段
   ▼
Render 层 Component（SpatialComponent / HUDComponent / FacadeComponent）
```

### 完整时序

```
[逻辑线程]                          [主线程]
OnStep                              OnLateTick
  │                                    │
  ├─ TickBehavior()                    ├─ ApplyProjection()
  ├─ pipeline.Process()                │     mirror.ApplyPackets()
  │     Crop + Rules                   │
  ├─ EnqueueCommand(input)             │
  │                                    ├─ Render（Godot 渲染）
```

### 关键抽象

| 角色 | 职责 |
|------|------|
| `IProjectable` | Logic 侧接口：脏标记 + TakeProjectValues/SetProjectValues/MarkAllDirty |
| `ProjectorAttribute` | 类级注解，标记需同步字段，SG 据此生成 backing field |
| `ProjectorTargetAttribute` | Render 侧 Component 标记，SG 据此生成 ApplyTo |
| `ProjectorPacket` | 一条 Info 的脏数据（mask + values） |
| `Observer` | 一个数据消费端（GM/Player/Spectator/AI 等） |
| `Crop` + `IProjectionRule` | 裁剪规则链，逐步修剪 mask |
| `IPropertyTransport` | 传输层（Local 直接写 Mirror / Network 走 MessagePack） |
| `Mirror` | Render 侧纯数据镜像，actor → (comptype → comp) |
| `ProjectorSnapshot` | 回滚快照 |

---

## 二、设计评估

### 优点

1. **Logic/Render 零耦合**：Logic 只产 `ProjectorPacket[]`，不接触 Godot；Render 通过 `Mirror` 拿纯数据。
2. **SG 零反射**：脏标记、`TakeProjectValues`、`ApplyTo` 全部源生成器生成，运行时无反射开销。
3. **脏标记位图**：`projectdirtymask` 用 `ulong` 位图，单字段变更只同步该字段，节省带宽。
4. **规则链可组合**：`Crop` + `IProjectionRule` 是经典责任链，新增规则不影响旧规则。
5. **多传输层抽象**：`LocalTransport` / `NetworkTransport` / `RemoteTransport` 覆盖单机/帧同步/网络游戏。
6. **快照回滚**：`ProjectorSnapshot` 支持全量快照与恢复，支撑帧同步回溯。

### 设计缺陷

1. **`Observer.crop` 字段是死代码**。`Crop.Process` 用全局 `pipeline.crop`，而不是 `obs.crop`。`ObserverFactory.CreateRuleChain` 创建的链从未被接入。设计意图是「每个 Observer 独立裁剪链」（Player 走 AOI+Permission+Visibility+Frequency，GM 走 GodRule），但当前实现是「所有 Observer 共用一条链」。
2. **`values` 数组与 `fieldmask` 解耦不彻底**。`TakeProjectValues(mask)` 按 bit 顺序收集，但裁剪后 mask 变小、values 数组不变，索引对齐就崩了（见 Bug 2）。
3. **`Mirror.ApplyPackets` 跨 Observer 去重逻辑可疑**（见 Bug 4）。
4. **`ProjectorPacket.values` 不入对象池**，每帧 `TakeProjectValues` 都 `new List + ToArray`，长期运行有 GC 压力。
5. **`ProjectorSystem` 遍历所有 BehaviorInfo**。注释说「数千 Actor μs 级可忽略」，但万级 MMO 必须空间索引。当前架构没有「脏 Actor 索引」这一层，扩规模时需要重构。
6. **`IComponentApply<T>` 接口的 `static abstract`** 依赖 .NET 7+，跨平台/旧 runtime 不可用。

---

## 三、Bug 清单

### Bug 1【严重】SG 生成的 `ApplyTo` 位掩码全部硬编码为 bit0

**位置**：`SpatialComponentApply.g.cs`、`HUDComponentApply.g.cs`

```csharp
// 所有字段都使用 (1UL << 0)，即 bit0
if (0 != (fieldmask & (1UL << 0))) c.position = (FPVector3)values[vi++];
if (0 != (fieldmask & (1UL << 0))) c.euler = (FPVector3)values[vi++];
if (0 != (fieldmask & (1UL << 0))) c.scale = (FP)values[vi++];
```

同一份 SG 生成的 `SpatialInfo.projector.g.cs` 是正确的 `1ul << 0/1/2`。说明 `ExtractProjectorData`（管线 2）的 index 计算对，`ExtractApplyToData`（管线 3）的 index 计算错，或 SG DLL 未重新编译。

**后果**：
- bit0 脏时所有字段尝试 `values[vi++]`，但 `values` 只含 bit0 一个值 → **IndexOutOfRangeException**
- bit0 不脏但 bit1 脏时所有字段被跳过 → **更新丢失**

当前能跑只因 `SpatialInfo`/`HUDInfo` 首帧 `MarkAllDirty` 后所有字段一起脏，但 `FacadeInfo` 有 9 个字段、含引用类型，错位会立刻崩。

**修复方向**：检查 `GoblinSourceGenerator.cs:ExtractApplyToData` 中的 `nextAutoIndex` 递增逻辑；重新编译 SG。

---

### Bug 2【严重】`ObserverPacket.values` 未随裁剪 mask 同步裁剪

**位置**：`Crop.cs:79-87`

```csharp
results.Add(new ObserverPacket
{
    fieldmask = mask,         // ← 裁剪后
    values = p.values,        // ← 仍然是按【原始 mask】收集的数组
});
```

**示例**：`SpatialInfo` 三字段 bit0/bit1/bit2。
- Logic mask = `0b111`，values = `[v0, v1, v2]`
- FrequencyRule mask 掉 bit1，`ObserverPacket.fieldmask = 0b101`
- ApplyTo 按 `0b101` 消费：`c.position = values[0]` ✓ → `c.scale = values[1]` ❌（其实是 euler）

**后果**：一旦接入 `FrequencyRule` / `PermissionRule`，字段错位、类型转换异常。

**修复方向**：`Crop.Project` 在产出最终 mask 后，按裁剪后 mask 重算一份 values。

---

### Bug 3【严重】`ProjectorSnapshot.Reset` 只清空数组第一个元素

**位置**：`ProjectorSystem.cs:267-281`

```csharp
public void Reset()
{
    if (null != data)
    {
        foreach (var arr in data.Values)
        {
            arr.SetValue(null, 0);   // ← 只清 index 0，其他元素残留引用
        }
        data.Clear();
        ObjectCache.Set(data);
        data = null;
    }
}
```

**后果**：快照对象池复用时，`object[]` 中 index ≥ 1 的旧引用不释放，引用类型实例被 dangling 引用住 → **内存泄漏**，且影响下次 Clone 语义。

**修复方向**：循环清空：`for (int i = 0; i < arr.Length; i++) arr[i] = null;`，或将数组本身也入池。

---

### Bug 4【中】`Mirror.ApplyPackets` 跨 Observer 去重导致数据丢失

**位置**：`Mirror.cs:90-96`

```csharp
eventframecache.Clear();
foreach (var p in packets)
{
    var key = $"{p.actor}_{p.behaviorinfotype.Name}";
    if (false == eventframecache.Add(key)) continue;   // 同 (actor, infotype) 只 Apply 第一份
    Apply(p.actor, p.behaviorinfotype, p.fieldmask, p.values);
}
```

**问题**：`Crop.Process` 对每个 Observer 产出一份 `ObserverPacket`，它们 `(actor, behaviorinfotype)` 相同。当存在 2 个以上 Observer 时，只有第一个 Observer 的裁剪结果被 Apply。

当前 `GameplayProxy` 只注册 1 个 Player Observer 不触发，但扩展到多 Observer 即丢数据。

**修复方向**：Mirror 改为「每个 Observer 一份独立镜像」`observer.id → (actor → comp)`，或 Pipeline 在 Send 前合并 mask/values。

---

### Bug 5【中】`Observer.crop` 死代码 + `ObserverFactory` 完全未接入

**位置**：`Observer.cs:48`、`ObserverFactory.cs`、`Crop.cs:67-91`、`GameplayProxy.cs:100-111`

- `ObserverFactory.CreateRuleChain` 返回的 Crop 无人调用
- `Crop.Process` 用的是 `pipeline.crop`（全局共用），不是 `obs.crop`
- `GameplayProxy` 注册 Observer 时没设 `crop`

**后果**：不同 ObserverType 无法走不同裁剪链。「GM 全通过、Player 走 AOI+权限+可见性+频率」的设计意图完全没生效。

**修复方向**：`Crop.Process` 改用 `obs.crop.Project(p, obs)`；`GameplayProxy` 用 `ObserverFactory.CreateRuleChain(type, seat)` 创建 Observer 的 crop；删除 `pipeline.crop` 字段。

---

### Bug 6【中】`InputSystem.GetInput` 返回 struct 副本，玩家输入永远写不进去

**位置**：`InputSystem.cs:42-50`

```csharp
public InputState GetInput(ushort key)
{
    if (false == inputs.TryGetValue(key, out var state))
    {
        state = new InputState();
        inputs[key] = state;
    }
    return state;   // ← InputState 是 struct，返回副本
}
```

全工程没有任何代码写入 `InputState.press`/`dire` 字段。`GameplayProxy.OnStep` 拿到默认值副本 → `stage.PushInput` 永远收「无输入」。

**后果**：玩家键盘/鼠标输入完全无效。当前只能靠 `EnemyAutopoilot` 和 `SimulatedInput`（Debug HTTP）注入。

**修复方向**：`InputState` 改 class（引用语义），或提供 `SetInput(key, press, dire)` 写入方法。

---

### Bug 7【中】`AOIRule` 在 `observer.radius` 默认 `FP.Zero` 时裁掉一切

**位置**：`AOIRule.cs:31-35`、`Observer.cs:43`

```csharp
public FP radius { get; set; }   // 默认 FP.Zero

var rad = observer.radius;
if (sqr <= rad * rad) return currentmask;   // rad = 0 时只有 sqr == 0 才通过
return 0;
```

`GameplayProxy` 创建 Observer 时没设 `radius`。一旦接入 `AOIRule`，距离 > 0 的所有 Actor 被裁掉。

**修复方向**：`Observer` 构造给 `radius` 合理默认值，或在 `AOIRule.Filter` 中 `rad <= FP.Zero` 时直接 `return currentmask`。

---

### Bug 8【低】多线程模式下 `pipeline.observerpackets` 跨线程读写无内存屏障

**位置**：`GameplayProxy.cs:50,55-59,262-267`

`OnStep` 在逻辑线程写 `pipeline.observerpackets`，`ApplyProjection` 在主线程读，无同步。

**后果**：多线程模式下主线程可能读到旧值。当前未崩溃因 .NET 引用赋值是原子的，但可见性不保证，可能漏帧。

**修复方向**：`Interlocked.Exchange` 写入 / `Interlocked.CompareExchange` 读取，或用 `ConcurrentQueue<ObserverPacket[]>`。

---

### Bug 9【低】`SetProjectValues` 与 `TakeProjectValues` 索引语义不一致

**位置**：SG 生成 `SpatialInfo.projector.g.cs:72-93`

```csharp
// TakeProjectValues：按 mask 的 bit 收集（只收集脏位）
public object[] TakeProjectValues(ulong mask) { ... }

// SetProjectValues：按位置 0/1/2 取（与 mask 无关）
public void SetProjectValues(object[] values) {
    field_0 = (FPVector3)values[0];
    field_1 = (FPVector3)values[1];
    field_2 = (FP)values[2];
}
```

如果 [Projector] 注解指定非连续 index（如 `[Projector("a", 0)]` + `[Projector("b", 5)]`），`SetProjectValues` 会越界。快照用 `ulong.MaxValue` 收集对齐正常，但两套不可互换的契约是定时炸弹。

**修复方向**：`SetProjectValues` 也按 bit 顺序写，或改为 `Dictionary<int, object>` 按 index 取。

---

### Bug 10【低】`GameplayProxy.OnStep` 多线程下无锁访问共享状态

**位置**：`GameplayProxy.cs:217-268`

`OnStep` 在后台线程读 `selfseat`、`stage`、`input`，主线程 `SwitchSeat`/`DestroyGame` 会改这些字段，无任何同步。`DestroyGame` 已 Join 线程 OK，但 `SwitchSeat` 仍有竞态。

**修复方向**：关键状态加 `volatile` 或 `Interlocked`。

---

### Bug 11【低】`Mirror.Apply` 静默跳过未注册的 InfoType

**位置**：`Mirror.cs:64-82`

```csharp
if (false == infotocomp.TryGetValue(infotype, out var comptype)) return;   // 静默丢掉
```

如果 `BehaviorInfo` 标了 `[Projector]` 但 `CreateGame` 忘了 `mirror.Register<>()`，数据被静默丢弃，无日志，难排查。

**修复方向**：未注册时打 Warning 日志。

---

### Bug 12【低】`NetworkTransport` 序列化 `values` 用 `object[]`

**位置**：`NetworkTransport.cs:30-42, 99-114`

`MessagePackSerializer.Serialize(list)` 序列化 `object[]` 时，FP/FPVector3 等自定义 struct 不会自动装箱为可序列化形式，反序列化后 `(FPVector3)values[vi++]` 会抛 `InvalidCastException`。

**修复方向**：网络传输为每个字段类型生成显式序列化器（SG 生成），不依赖 `object[]` 自动序列化。

---

## 四、优先修复建议

| 优先级 | Bug | 影响 |
|--------|-----|------|
| P0 | Bug 1 (ApplyTo bit0) | 一旦接入 FacadeInfo 必崩 |
| P0 | Bug 2 (values 未裁剪) | 接入任何非 GodRule 即字段错位 |
| P0 | Bug 6 (InputState struct) | 玩家无法操作 |
| P1 | Bug 3 (快照 Reset) | 内存泄漏 |
| P1 | Bug 4 (Mirror 去重) | 多 Observer 数据丢失 |
| P1 | Bug 5 (Observer.crop 死代码) | 设计意图未实现 |
| P2 | Bug 7 (AOI radius 默认 0) | 接入 AOI 即裁光 |
| P2 | Bug 8/10 (线程同步) | 多线程模式偶发问题 |
| P3 | Bug 9/11/12 | 隐患/可维护性 |
