# Projection-Render 流程分析

> 分析日期：2026-07-26（修订：2026-07-27）
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
   │     observerpackets = Crop.Process(packets, observers)
   │        foreach packet × observer → obs.crop.Project(packet, obs)
   │           foreach rule in crop.rules: mask = rule.Filter(...)
   │           mask != 0 → new ObserverPacket（含 TrimValues 裁剪后的 values）
   │  （GameplayProxy 不设 transport，留待主线程消费）
   ▼
GameplayView.OnLateTick（主线程）
   │  proxy.ApplyProjection()
   │     mirror.ApplyPackets(pipeline.observerpackets)
   ▼
Mirror.ApplyPackets
   │  逐条 Apply(actor, infotype, fieldmask, values)
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

---

## 二、设计评估

### 优点

1. **Logic/Render 零耦合**：Logic 只产 `ProjectorPacket[]`，不接触 Godot；Render 通过 `Mirror` 拿纯数据。
2. **SG 零反射**：脏标记、`TakeProjectValues`、`ApplyTo` 全部源生成器生成，运行时无反射开销。
3. **脏标记位图**：`projectdirtymask` 用 `ulong` 位图，单字段变更只同步该字段，节省带宽。
4. **规则链可组合**：`Crop` + `IProjectionRule` 是经典责任链，新增规则不影响旧规则。
5. **多传输层抽象**：`LocalTransport` / `NetworkTransport` / `RemoteTransport` 覆盖单机/帧同步/网络游戏。

### 设计缺陷

1. **`Observer.crop` 设计意图未完全实现**。`Crop.Process` 现已改用 `obs.crop.Project(p, obs)`（已修复），但 `ObserverFactory.CreateRuleChain` 仍无调用点，生产路径 `GameplayProxy` 手工构造 Observer 时直接把 `pipeline.crop`（空 Crop）赋给 `obs.crop`。Phase 1 等价 GodRule 无功能影响，但「每个 Observer 独立裁剪链」的设计意图未体现。

2. **`ProjectionPipeline.crop` 字段冗余且语义混淆**。`Crop.Process` 用 `obs.crop`，完全无视 `pipeline.crop`。后者仅被 `GameplayProxy` 当作共享空 Crop 塞给 Observer。两种模式（共享 crop vs ObserverFactory 每 Observer 独立 crop）并存却都没走通。建议删除 `pipeline.crop`。

3. **`ProjectorPacket.values` 不入对象池**，每帧 `TakeProjectValues` 内 `new List + ToArray`，`Crop.TrimValues` 又 `new List + ToArray`，长期运行有 GC 压力。

4. **`ProjectorSystem` 遍历所有 BehaviorInfo**。注释说「数千 Actor μs 级可忽略」，但万级 MMO 必须空间索引。当前架构没有「脏 Actor 索引」这一层，扩规模时需要重构。

5. **`IComponentApply<T>` 接口的 `static abstract`** 依赖 .NET 7+，跨平台/旧 runtime 不可用。

---

## 三、已修复 Bug（2026-07-27 复核）

### Bug 1【已修复】SG 生成的 `ApplyTo` 位掩码全部硬编码为 bit0

**位置**：`SpatialComponentApply.g.cs`、`HUDComponentApply.g.cs`

```csharp
// 旧代码：所有字段都使用 (1UL << 0)，即 bit0
if (0 != (fieldmask & (1UL << 0))) c.position = (FPVector3)values[vi++];
```

**修复状态**：SG `ExtractApplyToData` 现已按 `field.index` 正确生成 `1UL << {field.index}`，见 `GoblinSourceGenerator.cs:1437-1449`。

---

### Bug 2【已修复】`ObserverPacket.values` 未随裁剪 mask 同步裁剪

**位置**：`Crop.cs:79-87`（旧版）

**修复状态**：`Crop.Process` 现已调用 `TrimValues(p.values, p.fieldmask, mask)`，按裁剪后 mask 从原始 values 提取子集，见 `Crop.cs:99-110`。字段错位风险已排除。

---

### Bug 3【已过时】`ProjectorSnapshot.Reset` 只清空数组第一个元素

**位置**：旧版 `ProjectorSystem.cs:267-281`

**状态**：`ProjectorSystem.cs` 已在重构中缩减至 116 行，`ProjectorSnapshot` 类不再存在。此 Bug 随代码删除而过时。

---

### Bug 4【已修复】`Mirror.ApplyPackets` 跨 Observer 去重导致数据丢失

**位置**：`Mirror.cs:90-96`（旧版）

```csharp
// 旧代码：eventframecache 按 "{actor}_{infotype.Name}" 去重
var key = $"{p.actor}_{p.behaviorinfotype.Name}";
if (false == eventframecache.Add(key)) continue;
```

**修复状态**：`eventframecache` 已移除，`ApplyPackets` 改为逐条 `Apply`，不再跨 Observer 去重，见 `Mirror.cs:86-93`。

---

### Bug 6【已修复】`InputSystem.GetInput` 返回 struct 副本

**位置**：`InputSystem.cs:42-50`（旧版）

```csharp
// 旧代码：InputState 是 struct，GetInput 返回副本
public InputState GetInput(ushort key) { ... return state; }
```

**修复状态**：`InputState` 已改为 class 引用类型，并提供 `SetInput` 写入方法。玩家输入可正常写入。

---

### Bug 7【已修复】`AOIRule` 在 `observer.radius` 默认 `FP.Zero` 时裁掉一切

**位置**：`AOIRule.cs:31-35`（旧版）

**修复状态**：`AOIRule.Filter` 现已在 `rad <= FP.Zero` 时直接 `return currentmask`（直通），见 `AOIRule.cs:31-32`。

---

### Bug 5【半修复】`Observer.crop` 死代码 + `ObserverFactory` 完全未接入

**位置**：`Observer.cs:48`、`ObserverFactory.cs`、`Crop.cs:67-91`、`GameplayProxy.cs:100-111`

**当前状态**：
- `Crop.Process` 已改用 `obs.crop`（**已修复**），见 `Crop.cs:76-77`
- `ObserverFactory.CreateRuleChain` 仍无调用点（**未接入**）
- `GameplayProxy` 手工构造 Observer，`crop = pipeline.crop`（空 Crop），见 `GameplayProxy.cs:110`

**遗留问题**：不同 ObserverType 仍无法走不同裁剪链。见下文「集成缺口」章节。

---

## 四、当前未修复 Bug

### Bug 13【严重】`FrequencyRule` 抑制字段时仍滑动 `lastpushtable`，导致脏字段永不重发

**位置**：`FrequencyRule.cs:43-68`

```csharp
if (lastpushtable.TryGetValue(stateKey, out var lastFrame))
{
    if (packet.frame - lastFrame < interval)
    {
        result &= ~bit;          // 抑制（本次不推送）
    }
}
lastpushtable[stateKey] = packet.frame;   // ← 无条件更新！即便抑制也记录为"已推送"
```

**问题**：`lastpushtable` 语义应为「记录上次**实际推送**帧号」。当前实现：字段持续每帧脏时，首帧推送后 `lastpush` 被滑动到当前帧，`frame - lastFrame` 永远 = 1 < interval，**该字段再也不会被推送**，直到它先停止脏若干帧再变脏。这违背「按间隔推送」的初衷。

**影响**：Phase 1 未接入此规则暂不触发；Phase 2 一旦接入 `FrequencyRule`，持续变化的字段（如连续移动的 position）将只推送第一帧，之后永久静默。

**修复方向**：仅在实际推送（未走抑制分支）时更新 `lastpushtable[stateKey]`：

```csharp
if (lastpushtable.TryGetValue(stateKey, out var lastFrame))
{
    if (packet.frame - lastFrame < interval)
    {
        result &= ~bit;
        continue;   // 跳过 lastpushtable 更新
    }
}
lastpushtable[stateKey] = packet.frame;
```

---

### Bug 14【严重】`NetworkTransport` / `RemoteTransport` 用 `Type.GetType(FullName)` 反序列化，非 corelib 类型必返 null

**位置**：`NetworkTransport.cs:33`（序列化）、`NetworkTransport.cs:109`（反序列化）

```csharp
// 序列化端
behaviorinfotype = p.behaviorinfotype?.FullName ?? string.Empty,

// 反序列化端
behaviorinfotype = Type.GetType(d.behaviorinfotype),   // ← 对用户类型返回 null
```

**问题**：`Type.GetType(string)` 仅对 `System.Private.CoreLib` 内类型（`System.String` 等）可按 FullName 解析；对 `Goblin.Gameplay.Logic.BehaviorInfos.SpatialInfo` 这类用户定义类型，**必须带程序集限定名**（AssemblyQualifiedName）才返回非 null。

**影响**：`behaviorinfotype` 恒为 null → `Mirror.Apply` 在 `infotocomp.TryGetValue(infotype, ...)` 返回 false → 静默 return。**网络模式下所有投影数据被完全丢弃，无任何报错**。

**修复方向**：

方案一（最小改动）：序列化 `behaviorinfotype.AssemblyQualifiedName`，反序列化 `Type.GetType(assemblyQualifiedName)`。

方案二（推荐）：改用类型注册表 `Dictionary<string, Type>`，序列化短名/ID，反序列化查表。避免 AssemblyQualifiedName 的版本耦合问题。

---

### Bug 15【严重】多线程模式下引用类型投影字段存在数据竞争（容器并发修改）

**位置**：`GameplayProxy.cs:81,140,264-265`、`GameplayProxy.cs:55-58`

**触发前提**：`LobbyView` 以 `CreateGame(data, true)` 启动多线程模式。

**问题链**：

1. 逻辑线程 `OnStep` 调用 `pipeline.Process` → `Crop.Process` → `ObserverPacket.values` 引用 SG 生成的 `TakeProjectValues` 返回值。

2. SG 生成的 `TakeProjectValues` 对引用类型字段返回 **backing field 的活引用**（非 FP 类型直接 `return backing`，见 `GoblinSourceGenerator.cs:1154-1165`）。`FacadeInfo` 的 `effectdict`（`GBLDict`）、`animslots`（`GBLList<AnimationSlot>`）进入 `ObserverPacket.values` 后，逻辑线程下一帧仍会原地变更这些容器。

3. 主线程 `ApplyProjection` 调用 `mirror.ApplyPackets` → `FacadeComponent.ApplyTo`，其中 `new Dictionary<uint, EffectInfo>((GBLDict)values[vi++])` 正在迭代该容器 → **并发修改导致快照不一致**，极端情况抛 `InvalidOperationException`。

**与旧 Bug 8 的区别**：旧 Bug 8 只分析了「引用可见性」（`pipeline.observerpackets` 字段的 `volatile`），漏掉了 `ObserverPacket.values` 内部容器内容的引用别名问题。`volatile` 只能保证 `packetcache` 引用本身的可见性，挡不住容器内部并发修改。

**影响**：生产路径已开多线程（`LobbyView.cs:58`），`FacadeInfo` 的 `effectdict`/`animslots` 等引用类型字段存在实际数据竞争。

**修复方向**：

方案一（推荐）：SG 生成的 `TakeProjectValues` 对引用类型（非 FP/stuct）做深拷贝，确保产出的 values 脱离 Info 内部状态。

方案二：Pipeline 在产出 `ObserverPacket` 时对容器做快照拷贝。

方案三：多线程下对 `packetcache` 使用 `Interlocked.Exchange` + 双缓冲，确保主线程拿到的是逻辑线程已完成的一帧完整快照。

---

### Bug 16【低】`ApplyProjection` 每帧重放上一帧脏数据

**位置**：`GameplayProxy.cs:55-58`、`ProjectionPipeline.cs:52`

```csharp
// ProjectionPipeline.Process：无新数据时直接 return，不覆盖 packetcache
if (null == packets || 0 == packets.Length) return;

// GameplayProxy.ApplyProjection：首帧有数据后 observerpackets.Length 永远 > 0
if (null == pipeline || 0 == pipeline.observerpackets.Length) return;
mirror?.ApplyPackets(pipeline.observerpackets);
```

**问题**：首帧有脏数据后，`observerpackets` 数组 Length 永远 > 0，后续每个 render 帧（没有新逻辑帧数据时）都会重复 `ApplyPackets`。值类型字段幂等无所谓，但 `FacadeComponent.ApplyTo` 对 `effectdict` 每帧 `new Dictionary(GBLDict)` → 持续 GC 压力。

**修复方向**：Pipeline.Process 在无新 packets 时把 `observerpackets` 置为 `Array.Empty<ObserverPacket>()`，或 `ApplyProjection` 增加帧号去重。

---

### Bug 17【低】`Mirror.Apply` 静默跳过未注册的 InfoType

**位置**：`Mirror.cs:60-63`

```csharp
if (false == infotocomp.TryGetValue(infotype, out var comptype))
{
    return;   // ← 静默丢掉
}
```

**问题**：如果 `BehaviorInfo` 标了 `[Projector]` 但 `CreateGame` 忘了 `mirror.Register<>()`，数据被静默丢弃，无日志，难排查。

**修复方向**：未注册时打 Warning 日志。

---

### Bug 18【低】`SetProjectValues` 与 `TakeProjectValues` 索引语义不一致

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

如果 `[Projector]` 注解指定非连续 index（如 `[Projector("a", 0)]` + `[Projector("b", 5)]`），`SetProjectValues` 会越界。快照用 `ulong.MaxValue` 收集对齐正常，但两套不可互换的契约是隐患。

**修复方向**：`SetProjectValues` 也按 bit 顺序写，或改为 `Dictionary<int, object>` 按 index 取。

---

### Bug 19【低】`NetworkTransport` 序列化 `values` 用 `object[]`

**位置**：`NetworkTransport.cs:30-42, 99-114`

`MessagePackSerializer.Serialize(list)` 序列化 `object[]` 时，FP/FPVector3 等自定义 struct 不会自动装箱为可序列化形式，反序列化后 `(FPVector3)values[vi++]` 会抛 `InvalidCastException`。

**注**：Bug 14 已导致反序列化端 `behaviorinfotype` 为 null 从而静默跳过，此 Bug 的实际效果被 Bug 14 掩盖。修复 Bug 14 后，此 Bug 将暴露。

**修复方向**：网络传输为每个字段类型生成显式序列化器（SG 生成），不依赖 `object[]` 自动序列化。

---

## 五、集成缺口

### 缺口 1：`ObserverFactory` 在运行时完全未接入

全工程搜索 `ObserverFactory.` 仅命中 Tests 的 csproj 链接与文档，无任何调用点。`GameplayProxy.CreateGame` 手工构造 Observer 并赋 `crop = pipeline.crop`（空 Crop），绕过 `ObserverFactory.CreateRuleChain(type, id)`。

**Phase 2 待办**：接入 `ObserverFactory`，为不同 `ObserverType` 生成不同规则链（Player 走 AOI+Permission+Visibility+Frequency，GM 走 GodRule）。

### 缺口 2：`AOIRule.positionlookup` / `VisibilityRule.visibilitylookup` 委托无人注入

**位置**：`AOIRule.cs:15`、`VisibilityRule.cs:14`

```csharp
public Func<ulong, FPVector3> positionlookup { get; set; }
public Func<ulong, ulong, bool> visibilitylookup { get; set; }
```

全工程无赋值点。即便 `ObserverFactory` 接入规则链，也会因 `null == positionlookup` 走 fail-open 直通，AOI/可见性裁剪实质无效。

**Phase 2 待办**：在 `Stage` 或 `Mirror` 侧提供位置/可见性查询实现，通过 `ObserverFactory` 注入到规则中。

---

## 六、性能问题

### 1. `Crop.Process` 每帧大量短命对象分配

**位置**：`Crop.cs:66-94, 99-110`

每个存活的 (packet × observer) 对都执行：
- `new List<ObserverPacket>()`（`Process` 内部 `results`）
- `new List<object>()`（`TrimValues` 内部 `trimmed`）
- `results.ToArray()`（最终返回）
- `trimmed.ToArray()`（每个裁剪后的 values）

**建议**：引入对象池（`ObjectPool<List<ObserverPacket>>` 等），或预分配容量。

### 2. `Mirror.Apply` 用 `Activator.CreateInstance(comptype)` 反射创建 Component

**位置**：`Mirror.cs:73`

```csharp
comp = Activator.CreateInstance(comptype);
```

`Register<TInfo,TComp>` 已知 `TComp`，可同时注册 `() => new TComp()` 工厂委托，零反射创建。

### 3. `ApplyProjection` 每帧重放（见 Bug 16）

`FacadeComponent.ApplyTo` 每帧对 `effectdict` 做 `new Dictionary(GBLDict)` → 持续 GC 压力。

### 4. `RmvActor` 直接 `Clear()` 丢弃 Component，池化口径不一

**位置**：`Mirror.cs:98-105`

`FacadeComponent` 已实现 `IGBL`（可池化），但 `SpatialComponent`/`HUDComponent` 未实现。建议统一池化口径。

---

## 七、风格 / 小问题

1. **`ProjectorPacket.Clone` 浅拷贝违背接口契约**：`ProjectorPacket.cs:71-74` 用 `MemberwiseClone` 浅拷贝，但 `IGBL.Clone` 文档约定「深拷贝」。注释已承认，建议明确标注或补深拷贝。

2. **`RenderWorld.cs` 为空文件**：仅含注释 + 空 file-scoped namespace，无任何类型。要么删除，要么填实 Phase 3-4 入口。

3. **`GodRule` 定义在 `Crop.cs` 末尾**（`Crop.cs:116-125`），与「一类一文件」惯例不符。建议移到独立 `GodRule.cs`。

4. **大量非空自动属性触发 CS8618**：`Observer.crop`、`ProjectorPacket.behaviorinfotype/values/addedkeys/removedkeys`、`ObserverPacket.*`、`LocalTransport.mirror`、`NetworkTransport.onsend/mirror` 等未在构造函数赋值。建议按项目风格用 `?` 标可空或补 `required`。

5. **Lint 的 IDE1006 对 `type`/`id`/`crop` 等是误报**：项目规范明确属性全小写，这些是正确写法，无需改。

---

## 八、优先修复建议

| 优先级 | Bug | 影响 |
|--------|-----|------|
| P0 | Bug 14（`Type.GetType` 网络模式静默丢数据） | 网络模式下所有投影数据被丢弃，无报错 |
| P0 | Bug 15（多线程引用字段数据竞争） | 生产已开多线程，容器并发修改 |
| P1 | Bug 13（FrequencyRule lastpushtable 滑动） | Phase 2 一接入即字段永不重发 |
| P1 | 集成缺口 1（`ObserverFactory` 未接入） | 不同 ObserverType 裁剪链设计未实现 |
| P1 | 集成缺口 2（positionlookup/visibilitylookup 未注入） | AOI/可见性裁剪实质无效 |
| P2 | Bug 16（`ApplyProjection` 重放） | 持续 GC 压力 |
| P2 | 性能 1-3（`Crop` 分配、`Activator`、池化） | GC 累积 |
| P3 | Bug 17/18/19 + 风格问题 | 隐患/可维护性 |
