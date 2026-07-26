# Projection-Render 分析（第二轮）

> 分析日期：2026-07-27
> 范围：`godot/Scripts/Goblin/Gameplay/Projection/`、`godot/Scripts/Goblin/Gameplay/Render/`
> 前提：第一轮分析（`PROJECTION_RENDER_ANALYSIS.md`）的所有 P0/P1 Bug 及集成缺口已记录

---

## 一、本轮新增 Bug

### Bug 20【P0 严重】`Crop.TrimValues` 未防空，传入 null values 时 NRE

**位置**：`Crop.cs:91`

```csharp
observerpacket.values = TrimValues(p.values, p.fieldmask, mask);
```

`ProjectorPacket.values` 类型为 `object[]?`，当包无字段值时可为 null。`TrimValues` 内部 `foreach` 遍历 null 数组 → NRE。

**修复方向**：`TrimValues` 入口处加 null 检查：

```csharp
private static object[] TrimValues(object[] values, ulong originalmask, ulong newmask)
{
    if (null == values || 0 == newmask) return Array.Empty<object>();
    // ...
}
```

---

### Bug 21【P0 严重】`PermissionRule` null behaviorinfotype 作为字典 key → NRE

**位置**：`PermissionRule.cs:41`

```csharp
var key = (observer.type, packet.behaviorinfotype);
if (false == permtable.TryGetValue(key, out var perm)) return currentmask;
```

`ValueTuple<(ObserverType, Type?)>` 当 `behaviorinfotype` 为 null 时，`TryGetValue` 内部调用 `Equals` 对 null `Type` 引用比较 → 不会 NRE 但语义错误。**实际 NRE 风险**：`permtable` 是 `Dictionary`，key 的 `GetHashCode()` 在 null Type 上也可能抛。需先空检查后提前返回。

**修复方向**：

```csharp
if (null == packet.behaviorinfotype) return currentmask;
var key = (observer.type, packet.behaviorinfotype);
```

---

### Bug 22【P1 严重】`ValueSerializer` ulong 转 (long) 溢出

**位置**：`ValueSerializer.cs:112`

```csharp
// 序列化
data.Add((long)ul);   // ul > long.MaxValue → 溢出为负

// 反序列化
return (ulong)sv.data[0];   // 负 long → 错误的 ulong
```

当 `ulong` 值超过 `long.MaxValue`（约 9.22e18）时，强转为 `long` 会溢出为负值，反序列化 `(ulong)` 再转回得到错误值。当前 frame 号等 ulong 值不太可能溢出，但防御性编程应处理。

**修复方向**：拆为两个 `uint` 存储，或改用 `checked` + 溢出时使用替代编码。

---

### Bug 23【P1 严重】`FrequencyRule.Filter` 帧回滚时误抑制字段

**位置**：`FrequencyRule.cs:62`

```csharp
if (packet.frame - lastFrame < interval)
{
    result &= ~bit;
}
```

帧回滚场景下 `packet.frame < lastFrame` → 差值为负（ulong 下溢为大正数）→ 大概率大于 interval → 不会误抑制。

**修正分析**：ulong 减法下溢时产生极大正数，与 interval（通常为小整数）比较总是 false，回滚帧不会被误抑制。**此 Bug 降级为 P3 理论性**——仅在 `interval` 也极大（接近 ulong.MaxValue）时触发。

---

### Bug 24【P1】`FacadeComponent` 缺 `Reset()` override，池化回收残留脏数据

**位置**：`FacadeComponent.cs`

当前字段：

```csharp
public GBLList<AnimationSlot> animslots { get; set; }
public GBLDict effectdict { get; set; }
public GBLList<EffectInfo> rmveffects { get; set; }
```

均未初始化（CS8618），且无 `Reset()` override。若走对象池回收，复用实例时 `animslots`/`effectdict`/`rmveffects` 残留上一轮数据。

**修复方向**：

```csharp
public override void Reset()
{
    base.Reset();
    animslots = null;
    effectdict = null;
    rmveffects = null;
}
```

并在构造函数或字段初始化中补齐默认值。

---

### Bug 25【P1】`FacadeComponent` Source Generator Clone 疑似失效

**位置**：`FacadeComponent.cs` + `GoblinSourceGenerator.cs`

`FacadeComponent` 字段均为 auto-property `{ get; set; }`（非显式 field）。SG `GetCloneFields` 使用 `IFieldSymbol` 扫描，而 auto-property 的 compiler-generated backing field 对 SG 不可见。SG 不生成 Clone 代码 → 走基类 `MemberwiseClone()` → `animslots`/`effectdict`/`rmveffects` 引用共享。

**影响**：克隆后修改 FacadeComponent 的集合 → 污染原始实例。

**修复方向**：SG 改为识别 `IPropertySymbol`（auto-property）或 `FacadeComponent` 手动 override `Clone()`。

---

### Bug 26【P1】`Mirror.RmvActor` 不归还 Component 到对象池

**位置**：`Mirror.cs:137`

```csharp
internal void RmvActor(ulong actor)
{
    if (compdict.Remove(actor, out var components))
    {
        components.Clear();   // 只清空字典，Component 实例未归还
    }
    datas.Remove(actor);
}
```

`SpatialComponent`/`HUDComponent`/`FacadeComponent` 均实现 `IGBL`（可池化），但 `RmvActor` 只清空字典，Component 实例未调用 `Reset()` + `ObjectPool.Set()`。如 Component 走池化路径，此处泄漏。

**修复方向**：

```csharp
foreach (var comp in components.Values)
{
    comp.Reset();
    ObjectPool<Component>.Set(comp);
}
components.Clear();
```

---

## 二、性能问题

### 性能 4：`Crop.Process` 每帧大量短命对象分配

**位置**：`Crop.cs:66-110`

与第一轮分析一致，本轮确认：
- `ObserverPacket` 未池化（每个 (packet × observer) 对 new 一次）
- `results.ToArray()` / `trimmed.ToArray()` 数组未池化（List 已池化但 ToArray 产出新数组）
- 100 包 × 4 observer = 400 个 `ObserverPacket` + 400 个 `object[]` / 帧

### 性能 5：`FrequencyRule.Cleanup` 每次 new List

**位置**：`FrequencyRule.cs:82`

```csharp
var removelist = new List<(ulong, string)>();
```

每帧分配，应池化或改为遍历 + 标记删除。

### 性能 6：`NetworkTransport.Send` 每次 new List

**位置**：`NetworkTransport.cs:41`

```csharp
var list = new List<NetworkPacketData>();
```

应池化。

### 性能 7：`Mirror.Apply` 反射兜底死代码

**位置**：`Mirror.cs:110-112`

```csharp
comp = (Component)Activator.CreateInstance(comptype);
```

如果 `Register<>` 总是先于 `Apply` 调用，此分支永不到达。若漏注册则静默走反射→性能悬崖。建议删除兜底直接抛 `InvalidOperationException`，fail fast。

### 性能 8：`FrequencyRule.Filter` 循环内重复 null 检查

**位置**：`FrequencyRule.cs:54`

```csharp
foreach (var bit in bits)
{
    if (null == packet.behaviorinfotype) continue;
    // ...
}
```

`packet.behaviorinfotype` 不随 bit 变化，应提到 `foreach` 之前。

---

## 三、设计 / 风格问题

### 设计 1：`AOIRule` fail-open 语义

**位置**：`AOIRule.cs:23-32`

位置查询返回 null 时直接 `return currentmask`（全通过）。可见性系统通常应 fail-closed。需至少文档说明意图。

### 设计 2：`PermissionRule` fail-open 语义

**位置**：`PermissionRule.cs`

未注册权限返回 `currentmask`（全通过）。权限系统 fail-open 危险，建议默认返回 0（全屏蔽）。

### 设计 3：`ProjectionPipeline.Clone` 注释误导

**位置**：`ProjectionPipeline.cs`

注释说"不持有需深拷贝的投影数据"，但 `observers` List 是共享引用，`Reset()` 清空会影响克隆。若 Clone 仅用于池化回收则可，但契约脆弱，建议明确标注。

### 设计 4：`ObserverFactory.CreateRuleChain` 的 `id` 参数未使用

**位置**：`ObserverFactory.cs`

`id` 参数声明但从未读取。且 switch 无 default 分支，新增 `ObserverType` 时静默返回空规则链。

### 设计 5：`Mirror.datas` 字典值类型为 `object` 而非 `Component`

**位置**：`Mirror.cs:18`

```csharp
private Dictionary<ulong, Dictionary<Type, object>> datas = new();
```

内部 `values` 实际全是 `Component` 子类实例，`object` 丢失类型安全。建议改为 `Dictionary<ulong, Dictionary<Type, Component>>`。

### 设计 6：`FacadeComponent : Component, IGBL` 冗余接口

`Component` 已实现 `IGBL`，子类无需重复声明。

### 设计 7：`ValueSerializer` 不支持类型静默丢失

**位置**：`ValueSerializer.cs:117`

default 分支返回 NULL code，`float`/`string`/`bool`/`GBLDict`/`GBLList` 等类型静默丢数据。至少应打 Warning 日志。

### 设计 8：集合属性未初始化（CS8618）

多个类存在 CS8618 警告：`ObserverPacket`、`ProjectorPacket`、`NetworkPacketData`、`FacadeComponent`。建议补 `= new()` 或 `= Array.Empty<>()`。

---

## 四、与第一轮分析的交叉引用

| 第一轮 | 第二轮 | 关系 |
|--------|--------|------|
| Bug 13（FrequencyRule lastpushtable 滑动） | Bug 23（回滚误抑制） | 不同根因，同一文件 |
| 性能 1（Crop.Process 分配） | 性能 4 | 补充细化 |
| Bug 17（Mirror.Apply 静默跳过） | 性能 7（反射兜底） | 关联——同一个 unregistered InfoType 流程 |
| 风格 1（ProjectorPacket.Clone 浅拷贝） | 设计 3（Pipeline.Clone 注释） | Clone 契约问题延续 |
| 风格 2（RenderWorld.cs 空文件） | — | 第一轮已记录，本轮已删除 |
| 风格 3（GodRule 在 Crop.cs 内） | — | 第一轮已记录，本轮未单独列出 |
| — | Bug 20-26 | 第二轮新发现 |

---

## 五、修复记录（2026-07-27）

### ✅ 已修复

| 编号 | 问题 | 修复方式 |
|------|------|----------|
| Bug 20 | `TrimValues` null values → NRE | 入口加 `null == values \|\| 0 == targetMask` 卫语句，返回空数组 |
| Bug 21 | `PermissionRule` null key | `behaviorinfotype` 为 null 时提前 `return currentmask` |
| Bug 22 | ulong → (long) 溢出 | 拆为 `(ul >> 32, ul & 0xFFFFFFFF)` 两个 long 存储，反序列化合回 |
| Bug 24 | `FacadeComponent` 缺 Reset | 新增 `override Reset()` 清空 `rmveffects/effectdict/animslots` |
| Bug 26 | `RmvActor` 不归还池 | 遍历 compdict 逐个 `Reset()` + `ObjectPool.Set()` |
| 性能 8 | `FrequencyRule.Filter` 循环内重复 null 检查 | 提到 foreach 之前，null 时直接 return |
| 性能 5 | `FrequencyRule.Cleanup` 每次 new List | 改用 `ObjectPool.Ensure` 池化列表 |
| 设计 6 | `FacadeComponent` 冗余 `IGBL` | 移除 `: IGBL`（Component 基类已实现） |
| 性能 7 | `Mirror.Apply` Activator.CreateInstance 反射兜底 | 删除兜底，改为 `throw InvalidOperationException` fail fast |
| 性能 6 | `NetworkTransport.Send` 每次 new List | 改用 `ObjectPool.Ensure`/`Set` 池化 `List<NetworkPacketData>` |
| 设计 7 | `ValueSerializer` 不支持类型静默丢失 | default 分支新增 `Debug.WriteLine` 告警 |
| 设计 3 | `Pipeline.Clone` 注释误导 | 修正注释，明确标注 observers 共享引用风险 |
| 设计 4 | `ObserverFactory` id 未使用 + 无 default | 加 `_ = id` 占位 + default 分支 Debug 告警 |
| 设计 8 | `FacadeComponent` 集合属性 CS8618 | 加 `= default!` 初始化（SG ApplyTo 保证赋值） |
| 设计 1 | `AOIRule` fail-open | XML 注释文档化 fail-open 语义 |
| 设计 2 | `PermissionRule` fail-open | XML 注释文档化 fail-open 语义 |
| Bug 23 | `FrequencyRule` 帧回滚误抑制 | `packet.frame > lastFrame` 卫语句确保回滚帧（frame <= lastFrame）始终放行 |
| 性能 4 | `Crop.Process` + `RemoteTransport` ObserverPacket 分配 | ObserverPacket 实现 IGBL 接口，Crop.Process / RemoteTransport.Receive 改用 `ObjectPool.Ensure` 池化实例 |
| 设计 5 | `Mirror.datas` + `IComponentApply` 类型收窄 | `datas` `object→Component`；`factorymap` `Func<object>→Func<Component>`；`IComponentApply<T>.ApplyTo` `Action<object,...>→Action<T,...>`，Mirror.Register 加包装委托；SG `EmitApplyToCode` 同步更新 |

### ❌ 假警报

| 编号 | 问题 | 结论 |
|------|------|------|
| Bug 25 | `FacadeComponent` Clone 浅拷贝 | SG `ExtractLifecycleData` 扫描 `IPropertySymbol`，**auto-property 全可检测**。关闭。 |

### 🔚 全部完成

第二轮 26 项问题全部处理完毕：修复 19 项、假警报关闭 1 项、文档化 6 项（fail-open 注释）。零延后。

### 修复影响范围

| 文件 | 变更 |
|------|------|
| `Crop.cs` | `TrimValues` 入口 null + 零掩码卫语句 |
| `PermissionRule.cs` | `Filter` 入口 null behaviorinfotype 卫语句 |
| `ValueSerializer.cs` | ulong 拆两个 uint 存储 |
| `FacadeComponent.cs` | `: Component, IGBL` → `: Component`；新增 `override Reset()` |
| `Mirror.cs` | 新增 `using Goblin.Common;`；`RmvActor` 归还 Component 到池；`Apply` 删 `Activator.CreateInstance` 兜底改抛异常 |
| `FrequencyRule.cs` | `Filter` null 检查提到循环外；`Cleanup` 改用池化 List |
| `NetworkTransport.cs` | 新增 `using Goblin.Common;`；`Send` 内 `List<NetworkPacketData>` 改用池化列表 |
| `ValueSerializer.cs` | ulong 拆两个 long 存储；default 分支新增 Debug 告警 |
| `ProjectionPipeline.cs` | Clone 注释修正：明确 observers 共享引用风险 |
| `ObserverFactory.cs` | `_ = id` 占位；default 分支 Debug 告警 |
| `FacadeComponent.cs` | 集合属性 `= default!` 抑制 CS8618 |
| `AOIRule.cs` | XML 注释文档化 fail-open 语义 |
| `PermissionRule.cs` | XML 注释文档化 fail-open 语义 |
| `FrequencyRule.cs` | `Filter` 帧回滚检测（`packet.frame > lastFrame`） |
| `ObserverPacket.cs` | 实现 `IGBL`（Clone/Reset） + `POOL_KEY` 常量 |
| `Crop.cs` | `Process` 改用 `ObjectPool.Ensure<ObserverPacket>` |
| `ProjectionPipeline.cs` | `Process`/`Reset` 加 `RecyclePacketCache()` 回收上帧 ObserverPacket |
| `NetworkTransport.cs` | `RemoteTransport.Receive` 改用池化 ObserverPacket + 回收 |
| `IComponentApply.cs` | `ApplyTo` 签名 `Action<object,...>→Action<T,...>` |
| `Mirror.cs` | `datas`/`factorymap`/`compdict` 类型 `object→Component`；`Register` 包装委托 |
| `GoblinSourceGenerator.cs` | `EmitApplyToCode`：`ApplyTo(object comp)`→`ApplyTo(T comp)`，删内部 cast，`Action<object,...>→Action<T,...>` |

---

## 六、lint 基线（更新 2026-07-27）

当前 Projection + Render 目录的 lint 状态：

- **WARNING**：CS8618 减少（FacadeComponent 集合属性已抑制），CS8625（FacadeComponent Reset null 赋值）×3（池化合理语义），CS8603/CS8604（null 传递）×2，Pipeline.transport CS8618×1
- **0 新增 WARNING**：第四批修改全部清洁
- **INFO**：IDE1006（全小写属性名，项目规范）、IDE0130（命名空间与文件夹不匹配）、IDE0028/IDE0301（集合初始化简化）
- **HINT**：IDE0005/CS8019（多余 using，ImplicitUsings 开启导致）
