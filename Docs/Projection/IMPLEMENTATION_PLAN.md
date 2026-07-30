# Property Sync 实施计划

> 状态：`Plan`
>
> **统筹文档**：汇总三个设计文档的全部任务，拆解为可执行的阶段与子任务。
>
> 本计划记录迁移时的目标和历史进度，不是当前完成度报告。计划中的 `Entity`/`Component` 等名称对应当前的 `Canvas`/`Shadow` 实现；实际状态详见 [ARCHITECTURE.md](../ARCHITECTURE.md) §3.6 和 [../README.md](../README.md)。
>
> 基线文档：
> - [CORE.md](CORE.md) — 哲学底座：Simulation → Projection → Presentation
> - [PROPERTY_SYNC_DESIGN.md](PROPERTY_SYNC_DESIGN.md) — 完整工程方案
> - [BEHAVIORINFO_LIFECYCLE_REPORT.md](BEHAVIORINFO_LIFECYCLE_REPORT.md) — 生命周期自动化分析

---

## 1. 总体目标

**删 RIL，标 Projector，Entity/Component 镜像。Logic 几乎不动。**

| 指标 | 当前 | 目标 |
|------|------|------|
| Logic → Render 中间类 | ~40（RIL + Translator + Cross） | 0 |
| Render 类 | ~32（Agent/Enchant/Invoker/Chase） | 3（Entity/Component/RenderWorld） |
| 新增同步字段改动文件数 | 3-5 | 1（加注解） |
| BehaviorInfo 手写生命周期方法 | 72 | 0（Source Generator 生成） |
| Diff 机制 | 手写 hash + 快照比较 | 脏标记（写入记账，零 Diff） |
| 裁剪 | 无接入点 | 规则链原生支持 |

详见 `PROPERTY_SYNC_DESIGN.md` §1/§9，`BEHAVIORINFO_LIFECYCLE_REPORT.md` §1。

---

## 2. 架构速查

```
Simulation（不动）
    Logic BehaviorInfo + [Projector] 注解
                │
                ▼
Sync Projection（新增）
    ProjectorSystem 脏标记 → Crop 规则链裁剪 → Transport 分叉
                │
                ▼
Presentation（重写）
    Entity + Component（删 Agent/Enchant/Invoker/Chase）
```

详见 `CORE.md`。

---

## 3. 阶段总览

| 阶段 | 天数 | 核心交付 | 删除 | 新增 |
|------|------|---------|------|------|
| Phase 1：基础管线 | ~7 | 脏标记 → ProjectorPacket → Component.Apply | ~72 类 | ~18 类 |
| Phase 2：表现层 | ~5 | 插值 + 模型加载 + 特效 + 动画 | — | — |
| Phase 3：裁剪 + 网络 | ~4 | 规则链 + NetworkTransport | — | — |
| Phase 4：回滚 | ~3 | 快照回滚 + Component 粒度 Flash | — | — |
| Phase 5：优化（可选） | ~2 | ProjectState 扁平化 | — | — |

**总计 ~21 天**。

---

## 4. Phase 1：基础管线（~8 天）

> 验收：`dotnet test` 全绿。Logic 改 `SpatialInfo.position`，下一帧 `SpatialComponent.position` 自动更新。`partial class + IGBL` 的 Reset 零手写。

### 4.0 测试策略

主项目（`goblin.csproj`）使用 `Godot.NET.Sdk/4.6.2`，`LobbyView → Director → Render` 流程已断，**Phase 1 不能靠跑游戏测试**。

Phase 1 测试分为两层：

| 层 | 项目 | 测试什么 | 框架 |
|----|------|---------|------|
| **SG 快照测试** | `Tests/Goblin.SourceGenerators.Tests` | 生成代码语法 + 文本一致性 | Roslyn CSharpSourceGeneratorTest |
| **生命周期集成测试** | `Tests/Goblin.Logic.Tests` | 真实实例化 BehaviorInfo → Reset/Clone → 断言 ObjectCache 行为 | xUnit |

**关键设计**：集成测试需要一个能引用 BehaviorInfo + ObjectCache + Kowtow.Math 的测试项目。`Godot.NET.Sdk` 项目无法被标准测试项目引用。

**解决方案**：创建 `Tests/Goblin.Logic.Standalone`（`Microsoft.NET.Sdk`，net8.0），通过**文件链接**（`<Compile Link="...">`）引入主项目中的纯 C# 文件：

```
Tests/Goblin.Logic.Standalone/
  ├── (linked) ../../godot/Scripts/Goblin/Gameplay/Logic/Common/
  │     ObjectCache.cs         ← 对象池（依赖 CAPACITY_DEFINE）
  │     Defines/CAPACITY_DEFINE.cs
  │     Defines/ANIM_DEFINE.cs
  │     Defines/STATE_DEFINE.cs
  │     Defines/ATTRIBUTE_DEFINE.cs
  │     Math/FP.cs              ← 定点数
  │     Math/FPVector3.cs
  │     Math/FPVector2.cs
  │     Math/FPQuaternion.cs
  │     Math/IntVector2.cs
  │     Math/IntVector3.cs
  │     Math/FPMath.cs
  │     Math/FPMatrix.cs
  │     Math/FPMatrix4x4.cs
  │     Math/FPRandom.cs
  │     Math/FPF.cs
  │     Math/FPAcosLut.cs
  │     Math/FPSinLut.cs
  │     Math/FPTanLut.cs
  │     Math/FPVector4.cs
  │     Extensions/FPExtension.cs
  ├── (linked) ../../godot/Scripts/Goblin/Gameplay/Logic/Core/
  │     BehaviorInfo.cs         ← 基类
  ├── (linked) ../../godot/Scripts/Goblin/Common/
  │     ObjectPool.cs
  │     IGBL.cs                  ← 池化对象接口
  └── TestFixtures/
        SimpleInfo.cs           ← 测试桩：纯值类型
        ContainerInfo.cs         ← 测试桩：List<T> 容器
        NestedPoolInfo.cs        ← 测试桩：嵌套池化对象
        ProjectFieldInfo.cs      ← 测试桩：含 [Projector] 字段
```

这些文件的全部依赖仅 `System.*` 命名空间 + `CAPACITY_DEFINE` 常量，**零 Godot API 调用**。

文件链接机制：改主项目源文件自动反映到测试项目，不需要维护副本。

### T1.0 测试基础设施（1 天）

#### T1.0a 创建 `Tests/Goblin.Logic.Standalone` 项目（0.3 天）✅

- [x] 新建 `Tests/Goblin.Logic.Standalone/Goblin.Logic.Standalone.csproj`（`Microsoft.NET.Sdk`，net10.0，nullable enable）— 环境仅 10.0 运行时
- [x] 链接 ObjectCache.cs / ObjectPool.cs / IGBL.cs / CAPACITY_DEFINE / ANIM_DEFINE / STATE_DEFINE / ATTRIBUTE_DEFINE
- [x] 链接 Kowtow.Math 全部源文件（FP / FPVector2 / FPVector3 / FPQuaternion / IntVector2 / IntVector3 / FPMath / FPMatrix / FPMatrix4x4 / FPRandom / FPF / FPExtension + 3 个 Lut 文件）+ FPVector4
- [x] 链接 BehaviorInfo.cs 基类 + ProjectorAttribute.cs
- [x] 创建 4 个测试桩（SimpleInfo / ContainerInfo / NestedPoolInfo / ProjectFieldInfo），覆盖三种对象池模式（见 §4.1）
- [x] `dotnet build` 通过 — 29 个链接文件编译成功

#### T1.0b 创建 `Tests/Goblin.SourceGenerators.Tests` 项目（0.3 天）✅

- [x] 新建项目，引用 `Microsoft.CodeAnalysis.CSharp 4.8.0` + xUnit（避开 Roslyn 3.8.0 与 4.8.0 版本冲突，用直接 `CSharpGeneratorDriver` 替代 Testing 包）
- [x] 引用 `Goblin.SourceGenerators`（T1.1 产物）
- [x] 编写 4 个 SG 快照测试：partial+IGBL 生成、非 partial 不生成、无 IGBL 不生成、基类继承 IGBL 生成
- [x] `dotnet test` 通过 — 4/4 全绿

#### T1.0c 创建 `Tests/Goblin.Logic.Tests` 项目（0.4 天）✅

- [x] 新建项目（`Microsoft.NET.Sdk`，net10.0，xUnit），引用 `Goblin.Logic.Standalone` + `Goblin.SourceGenerators`
- [x] 编写首个生命周期集成测试：实例化 SimpleInfo → 设值 → Reset() → 断言字段归零（3 测试 T1.4 依赖，已 Skip）
- [x] 验证 SG 生成的 Reset() 被正确调用 — 基类钩子链通过
- [x] `dotnet test` 通过 — 4/4 全绿，3 跳过

**产出**：3 个测试项目 + 4 个测试桩 BehaviorInfo + 首个集成测试

**从 T1.0 开始，每个后续任务都追加对应测试，不允许"先写完再补测试"。**

---

### 4.1 ObjectCache 三种模式的测试桩

> 测试桩放在 `Goblin.Logic.Standalone/TestFixtures/`，覆盖 SG 必须处理的全部对象池模式。

#### 模式 1：值类型字段（SimpleInfo）

```csharp
public partial class SimpleInfo : BehaviorInfo  // BehaviorInfo : IGBL → SG 自动生成
{
    public int value;
    public FP speed;
    public bool active;
}
// SG 生成：override Reset() → value=0, speed=FP.Zero, active=false, base.Reset()
// 测试：SetObj → Reset → 断言归零
```

#### 模式 2：容器字段 — 值类型元素（ContainerInfo）

```csharp
public partial class ContainerInfo : BehaviorInfo
{
    public GBLList<uint> ids;
    public GBLDict<int, ulong> dict;
}
// SG 生成：override Reset() → ids.Reset(), dict.Reset(), base.Reset()
// 测试：Add 数据 → Reset → 断言 Count==0、容器引用未变
```

#### 模式 3：嵌套池化对象（NestedPoolInfo）

```csharp
public class PooledItem : IGBL
{
    public int x;
    public int y;
    public void Reset() { x = 0; y = 0; }
    public IGBL Clone() => ObjectCache.Ensure<PooledItem>().Assign(x, y);
}

public partial class NestedPoolInfo : BehaviorInfo
{
    public GBLList<PooledItem> items;
}
// SG 生成：IGBL 元素 → foreach item.Reset() + ObjectCache.Set + items.Reset()
//        override Clone() → foreach Add((PooledItem)src[i].Clone())
// 测试：Reset 后 Count==0、元素已还池、容器引用未变
```

#### 模式 4：含 [Projector] 字段（ProjectFieldInfo）

> **[Projector] 为类级 Attribute，`AllowMultiple=true`，`AttributeTargets.Class`。**
> 格式：`[Projector(name, typeof(T), index: N, defaultvalue?: V)]`
> SG 扫类上注释作 `/// <summary>`，生成 backing field `{类名小写}_{name}` + 脏标记属性。

```csharp
// 角色世界坐标
[Projector("position", typeof(FPVector3), index: 0)]
// 模型缩放（Reset 时归 FP.One 非 FP.Zero）
[Projector("scale", typeof(FP), index: 1, defaultvalue: 1)]
public partial class ProjectFieldInfo : BehaviorInfo
{
    public string name { get; set; }
}
```
SG 生成：
```csharp
partial class ProjectFieldInfo
{
    private FPVector3 projectfieldinfo_position { get; set; }
    private FP projectfieldinfo_scale { get; set; }

    public FPVector3 position { get; set; }  // setter 注入脏标记
    public FP scale { get; set; }             // setter 注入脏标记

    public object[] TakeProjectValues(ulong mask) { ... }
    public void ClearProjectDirty() { ... }
}
```
测试：SetObj → 断言 projectdirtymask 位正确、TakeProjectValues 只取脏字段。

---

### T1.1 注解定义 + Source Generator 框架（1 天）✅

- [x] 创建 `SourceGenerators/Goblin.SourceGenerators/Goblin.SourceGenerators.csproj`（`Microsoft.NET.Sdk`，netstandard2.0，LangVersion 11.0，引用 `Microsoft.CodeAnalysis.CSharp 4.8.0`）
- [x] 在 Common 层定义 `IGBL` 接口（`Reset()` + `IGBL Clone()`，`godot/Scripts/Goblin/Common/IGBL.cs`）
- [x] 定义 `[Projector(name, typeof(T), index, defaultvalue)]` 类级 Attribute：`ProjectorAttribute.cs`（`AllowMultiple=true`，`AttributeTargets.Class`，SG 扫类上注释生成 `/// <summary>`）
- [x] 实现 `GoblinSourceGenerator : IIncrementalGenerator` 入口：扫描 `partial class + IGBL` → 产出空 `.g.cs`（验证管线）
- [x] 主项目 `goblin.csproj` 引用此 SG：`<ProjectReference Include="..." OutputItemType="Analyzer" ReferenceOutputAssembly="false"/>`
- [x] 更新 `goblin.sln`，添加 3 个测试项目 + SG 项目 + Standalone

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.1，`BEHAVIORINFO_LIFECYCLE_REPORT.md` §4
**产出**：`IGBL.cs` / `ProjectorAttribute.cs` / `GoblinSourceGenerator.cs` + SG 空管线验证

**追加测试**：T1.0b SG 测试 → 标记 `partial class + IGBL` 的空 class → 断言 SG 产出了 `.g.cs` 文件。4/4 通过。

---

### T1.2 BehaviorInfo 基类钩子（0.5 天）✅

- [x] `BehaviorInfo` 实现 `IGBL` 接口
- [x] `Reset()` 改为 `virtual`：`OnReset()` → `actor=0; active=false`
- [x] `OnReset()` — `protected virtual`，空实现，用户覆写
- [x] 新增 `Clone()` — `virtual`，空实现，SG 为 `partial class + IGBL` 类生成 `override`
- [x] `IGBL.Clone()` 显式接口实现，委托 `Clone()`
- [x] 投影职责剥离到 `IProjectable` 接口（`projectdirtymask` + `TakeProjectValues`），不在 BehaviorInfo 基类
- [x] 现有手写 `OnReset/OnReady/OnClone` 暂时保留，T1.11 才替换

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.4.1，`BEHAVIORINFO_LIFECYCLE_REPORT.md` §5
**产出**：`BehaviorInfo.cs`（修改）

**追加测试**：SimpleInfo `partial class` → Reset() → 断言基类钩子链正确。4/4 通过，3 个 T1.4 依赖测试已 Skip。

---

### T1.3 属性 + 脏标记生成（1.5 天）

#### T1.3a ProjectorAttribute 升级为类级（0.3 天）✅

- [x] `ProjectorAttribute` 改为 `AttributeTargets.Class`，`AllowMultiple = true`
- [x] 参数：`string name`（属性名）、`System.Type type`（C# 类型）、`int index`（位索引）、`int defaultvalue = 0`（Reset 时的缺省值）
- [x] 更新 `ProjectorAttribute.cs`，删 `index` 只读属性，改用构造函数参数 + `defaultvalue` 全小写
- [x] 更新 `ProjectFieldInfo` 测试桩为新格式（类级注解）

#### T1.3b SG 扫描 [Projector] + 生成 backing field（0.4 天）✅

- [x] SG 入口从扫描 `partial class + IGBL` 扩展为同时扫描 `[Projector]` 注解
- [x] 为每个 `[Projector]` 生成 backing field：`private T {类名小写}_{name} { get; set; }`
  - 例：`SpatialInfo` + `position` → `private FPVector3 spatialinfo_position { get; set; }`
- [x] 扫描类上 `//` 注释，匹配到 `[Projector]` 即生成 `/// <summary>` XML 文档注释
  - 取最近的上一行 `//` 前缀注释

#### T1.3c SG 生成脏标记属性（0.5 天）✅

- [x] 生成 `public T name { get => backing; set { ... } }`：
  - setter 值变检测：`if (backing != value)` → 写 backing + `projectdirtymask |= (1ul << index)`
  - FPVector3/FP 等值类型比较依赖 `!=` 重载（无重载时 SG 生成 `!(a == b)`）
- [x] 生成 `public object[] TakeProjectValues(ulong mask)` — `IProjectable` 接口实现，按 mask 位取脏字段值装箱
- [x] 生成 `public void ClearProjectDirty()` — `projectdirtymask = 0`
- [x] FP 类型序列化：`new FP(backing.rawValue)` 避免装箱，object[] 中用 FP 实例

#### T1.3d 值类型序列化（0.3 天）✅

- [x] SG 按类型生成序列化路径（`SerExpression` + `CastExpression`）：
  - 值类型（int/bool/FP/FPVector3 等）→ `(object)` 显式装箱
  - 引用类型（string 等）→ 直接传入
  - Phase 1 使用 LocalTransport，装箱即可；Phase 5 flat struct 优化序列化
- [x] Deserialize 反向路径在 `IProjectable.SetProjectValues()` + `Component.Apply()` 中处理

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.4.2
**产出**：`ProjectorAttribute.cs`（修改）+ Source Generator 属性生成逻辑

**追加测试**：ProjectFieldInfo → SetObj → 断言 projectdirtymask 位正确、TakeProjectValues 只取脏字段

---

### T1.4 生命周期生成（1.5 天）✅

- [x] `partial class + IGBL` 生成 `public override void Reset()`：
  - 值类型 → default 值（尊重 `[Projector(default: x)]`）
  - GBLDict/GBLList → 调 `container.Reset()`（清数据不还池）
  - `IGBL` 引用类型 → `foreach Reset + ObjectCache.Set → null`（还池）
  - 非 IGBL 引用类型 → `null`
  - `projectdirtymask = 0`
  - 尾调 `base.Reset()` → 触发 `OnReset()` + `actor/active` 归零
- [x] 生成 `public override BehaviorInfo Clone()`：
  - 值类型 → 直接赋值
  - 容器值类型 → `new T(field)` 拷贝构造
  - 容器 IGBL → `new T(field.Count) + foreach Clone()`
  - `IGBL` 引用类型 → `src.field?.Clone()`
  - `Ensure<T>()` → 字段拷贝 → `Ready(actor)` → 返回 `this`
- [x] SG 入口：`partial class + IGBL`（不限于 BehaviorInfo 子类）
- [x] `dotnet test` 通过 — 7/7 全绿

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.4.2-2.4.3，`BEHAVIORINFO_LIFECYCLE_REPORT.md` §5-6
**产出**：Source Generator Reset/Clone 生成逻辑

**追加测试**：
- SimpleInfo Reset → 断言 value==0, speed==FP.Zero, active==false
- ContainerInfo Reset → 断言 ids.Count==0, dict.Count==0, 容器引用未变
- NestedPoolInfo Reset → 断言 items 内元素 Reset 被调用, 列表 Clear 但对象未还池
- NestedPoolInfo Clone → 断言新对象通过 ObjectCache.Ensure 获取, 嵌套元素深拷贝

---

### T1.5 GBLDict / GBLList 体系（含脏追踪变体）（1 天）✅

- [x] `GBLDict<K,V>`：池感知字典基类，`Reset()` 回收 `IGBL` 元素 + 值类型清空，`Clone()` 深拷贝数据、不拷贝脏状态
- [x] `TGBLDict<K,V>`（继承 `GBLDict`）：脏追踪字典 — 写入即记账（`addedkeys`/`removedkeys`/`changedkeys`），`CollectDiff()` 消费差量后归零追踪，增删同一 key 自动抵消
- [x] `GBLList<T>`：池感知列表基类，`Reset()` 回收 `IGBL` 元素，`Clone()` 深拷贝
- [x] `TGBLList<T>`（继承 `GBLList`）：脏追踪列表 — 写入即记账（`addedindices`/`removedindices`），`CollectDiff()` 消费后归零，增删同一索引自动抵消
- [x] `dotnet test` 通过 — 59/59 全绿（+56 GBL 测试，含 TGBLDict/TGBLList 脏追踪）

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.3
**产出**：`GBLDict.cs` / `GBLList.cs` / `TGBLDict.cs` / `TGBLList.cs`

---

### T1.6 ProjectorSystem（1 天）✅

- [x] 自检遍历 `stage.cache.behaviorinfodict`，`is IProjectable` 过滤含 `[Projector]` 的类
- [x] `OnEndTick()`：读 `projectdirtymask` → `TakeProjectValues` → 产出 `ProjectorPacket[]` → 清零
- [x] 全量同步：`IProjectable.MarkAllDirty()`（SG 生成）+ `Stage.AddBehaviorInfo` 注入，新对象首帧全量投影
- [x] `OnEndTick` 零分配：无脏数据不分配，有脏时 `List` 池化归还
- [ ] 集合 Diff 收集：对有 GBLDict/List 字段且 mask 位为 1 的，调 `CollectDiff()`（依赖 SG 生成字段映射，Phase 1 占位）
- [x] 快照管理（Phase 4 提前实现）：`TakeSnapshot` / `FlashRestore` / `ProjectorSnapshot` — 环形缓冲区 32 帧
- [x] Actor 移除：Stage 回收时 behaviorinfodict 自动清理，无需 RmvActor
- [x] 属性 setter 只写 `projectdirtymask` 位标记，无回调（自检模式，非脏集注册）

**输入**：`PROPERTY_SYNC_DESIGN.md` §3
**产出**：`ProjectorSystem.cs` / `ProjectorPacket.cs`

---

### T1.7 Crop 接口 + GodRule（0.5 天）✅

- [x] `IProjectionRule`：`ulong Filter(ProjectorPacket, Observer, ulong currentmask)`
- [x] `Crop`：规则链串联，mask == 0 丢弃；`Crop.Process` 批量产出 `ObserverPacket[]`
- [x] `GodRule`：全通过（零裁剪，Phase 1 所有 Observer 挂此）
- [x] `Observer` + `ObserverType` 枚举（Player/Spectator/GM/Replay/AI/Editor）

**输入**：`PROPERTY_SYNC_DESIGN.md` §4
**产出**：`IProjectionRule.cs` / `Crop.cs` / `Observer.cs` / `ObserverPacket.cs`

---

### T1.8 Transport 接口 + LocalTransport（0.5 天）✅

- [x] `IPropertyTransport`：`void Send(ObserverPacket[])`
- [x] `LocalTransport`：通过 `onsend` 事件暴露数据流（T1.9 接入 RenderWorld）
- [x] `ProjectionPipeline`：串联 ProjectorSystem → Crop → Transport
- [x] 计算 `latency`（帧同步恒 0，Phase 1 ProjectorPacket.latency = 0）

**输入**：`PROPERTY_SYNC_DESIGN.md` §6
**产出**：`IPropertyTransport.cs` / `LocalTransport.cs`

---

### T1.9 Entity + Component + RenderWorld（1 天）✅

> ⚠️ **实际实现与设计有差异**：`Entity`/`RenderWorld` 未创建，改为 `Mirror` 统一管理 ActorID→Component 映射和 ApplyPackets 入口。Component 基类通过 `IComponentApply<T>.ApplyTo()` 静态委托消费数据（零反射）。功能等价，架构更精简。

- [x] `Mirror`：datas 字典 + infotocomp/applymap/factorymap 注册表；`ApplyPackets()` 入口；`GetComp<T>()` / `HasActor()` 查询
- [x] `Component` 基类（纯数据容器，IGBL）：`virtual Clone()` / `virtual Reset()`
- [x] `IComponentApply<T>` 接口：`static abstract ApplyTo(T, ulong, object[])` — 零反射消费
- [x] `[ProjectorTarget(typeof(XxxInfo))]` 类级注解标注 BehaviorInfo→Component 映射
- [x] `LocalTransport` 接入 Mirror（Send → ApplyPackets）
- [x] 端到端链路验证：ProjectorSystem → ProjectionPipeline → LocalTransport → Mirror → Component.ApplyTo
- [x] 用户手写首批 Component：`SpatialComponent`（SpatialInfo 3 投影字段：position/euler/scale）、`FacadeComponent`、`HUDComponent`
- [ ] 用户手写：`TickerComponent`（需先给 TickerInfo 加 [Projector] 注解）
- [ ] Source Generator 生成 `Apply` 方法（依赖 BehaviorInfo→Component 映射机制，后续）

**输入**：`PROPERTY_SYNC_DESIGN.md` §7
**产出**：`Entity.cs` / `Component.cs` / `RenderWorld.cs` / `SpatialComponent.cs` / `TickerComponent.cs`

---

### T1.10 删 RIL + Agent 体系 + Director 重接（0.5 天）✅

> ⚠️ **确认 T1.9 链路跑通后再删。**

- [x] 删 RIL 体系（~40 类）：IRIL 及子类、Translator 及子类、RILSync/RILDispatch/RILCache/RILCross/IRIL_DIFF/RIL_DEFINE/RILSalute/Salute
- [x] 重写 Director：GameplayDirector + LocalDirector（旧版已归档，按新 ProjectorSystem → Transport → RenderWorld 流程重建）
- [x] 清理 Sys/ 层对旧 Director 的引用（GameplayView、HUDView 移除 RIL 引用）
- [x] 删除 `_Archive` 目录（旧 Director/Render/Agent 全部移除）
- [x] 创建最小 `InputSystem` 替代旧 World.input

**输入**：`PROPERTY_SYNC_DESIGN.md` §7.2/§9
**产出**：删除 ~40 文件/类 + 新 Director 2 文件 + InputSystem 1 文件

---

### T1.11 `partial class` 迁移（0.5 天）

按复杂度分 4 批迁移 26 个 BehaviorInfo 子类（**26/26 全部完成**）：

| 批次 | 类 | 特征 | 风险 | 状态 |
|------|-----|------|------|------|
| 1 | TickerInfo, MovementInfo, MagicInfo | 纯值类型，1-2 字段 | 零 | ✅ |
| 2 | SpatialInfo, StateMachineInfo, SkillLauncherInfo, ColliderInfo, HitLagInfo, SkillCooldownInfo, CareerInfo, BuffInfo, RandomInfo, SeatInfo, EventorInfo | 值类型 + struct + 单层容器 | 低 | ✅ |
| 3 | TagInfo, BuffBucketInfo, FlowEffectInfo, GamepadInfo | 单层容器 | 中 | ✅ |
| 4 | FacadeInfo, StageInfo, FlowCollisionInfo 系列, AttributeBucketInfo, FlowInfo, SilentMercyInfo | 深层嵌套容器 | 高 | ✅ 全部 8 个 |

> **批次1-3 完成（18 个类）**：标 partial + 删 OnClone（SG 接管）；OnReset 仅保留非 default 值字段（TickerInfo.timescale / ColliderInfo.layer / RandomInfo.a,c,m）；容器类 OnReady null 检查 Ensure（只清不还）。
>
> **批次4 完成 8 个**：AttributeBucketInfo / FlowInfo / SilentMercyInfo / StageInfo / FacadeInfo（嵌套容器深拷贝）+ FlowCollisionInfo / FlowCollisionHurtInfo / FlowCollisionSensorInfo（抽象类 + 继承链 Clone）。
>
> **FlowCollisionInfo 系列所需 SG 扩展**：
> - 抽象类：SG 跳过 Clone 生成（`Ensure<abstract class>()` 非法）
> - 继承链 Clone：子类 Clone 通过 `CollectParentFields` 沿 IGBL 链收集父类字段，按类型规则生成深拷贝
>
> **SG 新增**：
> - `ContainerNestedValue`/`ContainerNestedIGBL` 类别，识别 `Dictionary<K, List<V>>` / `Dictionary<K, Dictionary<K2,V>>` / `Dictionary<K, List<IGBL>>` 嵌套模式，Clone 递归深拷贝内层，Reset 外层+内层 Clear
> - **非 BehaviorInfo IGBL 支持**：SG 通过 `isBehaviorInfo` 标志区分 `override Reset/Clone`（BehaviorInfo 子类）与接口实现 `void Reset() / IGBL Clone()`（纯 IGBL 类如 AnimationSlot）
>
> **SG 修复**：
> - Clone `Ready` 移到字段拷贝前（避免 OnReady/OnReset 覆盖拷贝值）
> - Clone 源字段加 `this.` 前缀（避免变量名 `c` 与属性 `c` 冲突，如 RandomInfo）
> - 嵌套容器深拷贝（ContainerNestedValue/ContainerNestedIGBL）
> - 非 BehaviorInfo IGBL 类：Clone 不调用 `c.Ready(actor)`，返回 `IGBL` 而非 `BehaviorInfo`
> - 抽象类跳过 Clone 生成
> - 子类 Clone 包含父类链字段（`parentFields` 收集）
>
> **AnimationSlot IGBL 化**：`AnimationSlot` 标 `partial` + `: IGBL`，SG 生成 Reset（default 所有字段）和 Clone（Ensure + 拷贝），FacadeInfo 的 `List<AnimationSlot>` 自动识别为 `ContainerIGBL` 深拷贝。

每批操作：类加 `partial` → SG 识别 `IGBL` 自动生成 Reset/Clone → 删手写 OnReady/OnReset/OnClone → `dotnet test` 全绿再进下一批。

随批次 4 自然修复 3 个已知 Bug：
- FlowCollisionInfo.OnClone 硬编码子类类型 → SG 用 `Ensure<实际类型>()` ✅
- FlowCollisionHurtInfo 子类字段未 Reset → `partial + IGBL` 接管全部字段 ✅
- OnReady 调 OnReset 反模式 → 容器不还池 ✅

**输入**：`BEHAVIORINFO_LIFECYCLE_REPORT.md` §2/§8
**产出**：26 个 BehaviorInfo 子类迁移完成，72 个手写方法归零

**每批迁移都通过 `Goblin.Logic.Tests` 的集成测试验证。用真实 BehaviorInfo 子类实例跑 Reset/Clone 断言。**

---

### Phase 1 任务依赖

```
T1.0（测试基础设施）
 │
 ├── T1.0a（Standalone 项目）
 ├── T1.0b（SG 测试项目）
 └── T1.0c（Logic 集成测试）
 │
 └── T1.1（注解 + SG 框架）
      │
      ├── T1.2（基类钩子）
      │     │
      │     ├── T1.3（属性 + 脏标记生成）
      │     │     │
      │     │     ├── T1.6（ProjectorSystem）
      │     │     │     │
      │     │     │     ├── T1.7（Crop + GodRule）
      │     │     │     │     │
      │     │     │     │     └── T1.8（Transport）
      │     │     │     │           │
      │     │     │     │           └── T1.9（Entity/Component/RenderWorld）
      │     │     │     │                 │
      │     │     │     │                 └── T1.10（删 RIL + 重接 Director）
      │     │     │     │
      │     │     │     └── T1.5（GBLDict/List）
      │     │     │
      │     │     └── T1.4（生命周期生成）
      │     │
      │     └── T1.11（partial class 迁移）
```

**关键路径**：T1.0 → T1.1 → T1.2 → T1.3 → T1.6 → T1.7 → T1.8 → T1.9 → T1.10（9 步，~7 天）
**可并行**：T1.4 与 T1.5 在 T1.3 之后并行推进

**测试纪律**：每个 T1.x 完成时，对应测试必须通过。T1.11 每批次迁移后 `dotnet test` 全绿再进下一批。

---

## 5. Phase 2：表现层（~5 天）

> 验收：角色移动平滑插值，模型加载正常，特效跟随集合变更。

### T2.1 ProjectionStrategy：插值与预测（1.5 天）✅ 核心实现完成

- [x] Component 基类新增 `OnExpress(float dt)` 虚方法 + `CaptureSnapshot()` + 环形缓冲区（4 帧）
- [x] PushHistory ring buffer 扩充至 4 帧（`CaptureSnapshot` 在 Apply 后捕获全量属性）
- [x] `OnExpress` 中自适应插值：`accumulatedTime` 跨帧边界 Snap 到最新，未越界做线性插值
- [ ] 自动判断时间方向：`frame < renderFrame` → 插值，`frame > renderFrame` → 预测，`frame == renderFrame` → 直接 Apply（Phase 3 网络）
- [ ] Jitter Buffer 自适应窗（latency 稳定 → 小窗，抖动 → 大窗）（Phase 3 网络）
- [ ] 平滑修正：`correctionDelta = (serverValue - current) * smoothFactor`（Phase 3 网络）
- [ ] 阈值 Snap：误差过大直接跳正（Phase 3 网络）

**输入**：`PROPERTY_SYNC_DESIGN.md` §5
**产出**：`Component.cs`（修改）/ `ProjectionStrategy.cs`

---

### T2.2 SpatialComponent 插值（0.5 天）✅

- [x] `OnExpress` 中 position lerp（FPVector3.Lerp）+ euler lerp
- [x] t = accumulatedTime / LOGIC_FRAME_INTERVAL（40ms @ 25fps）

**产出**：`SpatialComponent.cs`（修改）

---

### T2.3 FacadeComponent：模型加载（1 天）

- [ ] 监听 modelid 变更 → 异步加载 .tscn/.glb
- [ ] 加载完成实例化挂到 Entity Node3D
- [ ] modelid 变更 → 销毁旧模型、加载新模型

**产出**：`FacadeComponent.cs`

---

### T2.4 EffectComponent：特效创建/回收（1 天）

- [ ] 监听 effectdict 的 addedKeys/removedKeys/changedKeys
- [ ] added → 创建特效实例，removed → 回池，changed → 更新参数
- [ ] 特效对象池管理

**产出**：`EffectComponent.cs` / `EffectPool.cs`

---

### T2.5 AnimationComponent：动画推进（0.5 天）

- [ ] 监听 animstate/animhash/animticktype
- [ ] 状态变化 → 播动画，手动 ticktype → 手动设置 progress
- [ ] Crossfade 带 blend time

**产出**：`AnimationComponent.cs`

---

### T2.6 分层 Express（0.5 天）

- [ ] 定义 Phase A(Spatial) → B(Animation) → C(Facade) → D(Effect) 执行顺序
- [ ] Component 间依赖声明

**产出**：`RenderWorld.cs`（修改：分层 Tick）

---

## 6. Phase 3：裁剪 + 网络（~4 天）

> 验收：敌方只看位置不看 HP，网络序列化正确，状态同步链路跑通。

### T3.1 裁剪规则实现（1.5 天）

- [ ] `AOIRule`：距离过滤，超半径返回 0
- [ ] `PermissionRule`：(关系, behaviorType) → 允许的 fieldmask。敌方 mask 掉 hp
- [ ] `VisibilityRule`：草丛/隐身标记，不可见返回 0
- [ ] `FrequencyRule`：每个字段独立推送间隔
- [ ] Observer 工厂：按 ObserverType 组装规则链

**输入**：`PROPERTY_SYNC_DESIGN.md` §4.4
**产出**：AOIRule/PermissionRule/VisibilityRule/FrequencyRule/ObserverFactory

---

### T3.2 NetworkTransport + 序列化（1.5 天）

- [ ] `NetworkTransport`：按 ObserverPacket 序列化（actor+frame+behaviorType+fieldmask+values）
- [ ] SG 生成 Serialize/Deserialize
- [ ] 接收端 `RemoteTransport` → 反序列化 → `RenderWorld.Apply()`

**输入**：`PROPERTY_SYNC_DESIGN.md` §6.3
**产出**：`NetworkTransport.cs` / `RemoteTransport.cs` / SG 序列化生成

---

### T3.3 状态同步预测（1 天）

- [ ] Player Observer 输入预测：客户端基于快照 + 本地输入死推算
- [ ] 服务端确认 → 误差平滑修正/snap

**输入**：`PROPERTY_SYNC_DESIGN.md` §5.3/§5.7
**产出**：`ProjectionStrategy.cs`（修改）/ `PredictionState.cs`

---

## 7. Phase 4：回滚（~3 天）

> 验收：帧同步 rollback 时 Component 粒度 Flash，不回滚的 Component 不受影响。

### T4.1 ProjectorSystem 快照回滚（1 天）

- [ ] `TakeSnapshot` / `CloneSnapshot` — 仅 `[Projector]` 字段
- [ ] 回滚时取出目标帧快照 → 恢复 BehaviorInfo → 重新 Tick
- [ ] 清理超出回滚窗口的快照

**产出**：`ProjectorSystem.cs`（修改）/ `BehaviorInfoSnapshot.cs`

---

### T4.2 RenderWorld 回滚（1 天）

- [ ] 记录回滚窗口内每个帧的 `(actor, behaviorType, fieldmask, values)`
- [ ] Rollback 时标记受影响 Entity → Component
- [ ] 只 Flash 标记为 dirty 的 Component

**产出**：`RenderWorld.cs`（修改）

---

### T4.3 事件幂等（1 天）

- [ ] 事件关联 frame：`lastProcessedFrame` 去重
- [ ] 回滚时回放事件（非重复不重播）

**产出**：事件系统修改

---

## 8. Phase 5：优化（可选，~2 天）

### T5.1 ProjectState 扁平 struct（1 天）

- [ ] 将 `[Projector]` 字段打包为 struct
- [ ] 快照/序列化 memcpy 量级，消除 object[] 装箱

**产出**：`ProjectState` struct + SG 生成逻辑修改

---

### T5.2 嵌套对象支持（0.5 天）

- [ ] `[ProjectNested]` 注解
- [ ] SG 生成嵌套对象递归 Reset/Clone/Serialize

**产出**：`ProjectNestedAttribute` + SG 扩展

---

### T5.3 性能验证（0.5 天）

- [ ] 1000 Entity 场景压力测试
- [ ] 脏集遍历开销测量
- [ ] 序列化带宽测量
- [ ] 边缘 case 覆盖（空脏集、全脏、Actor 快速创建销毁）

---

## 9. 风险与注意事项

| 风险 | 等级 | 缓解 |
|------|------|------|
| Source Generator 调试困难 | 中 | T1.1 产空文件验证框架，T1.3 增量加逻辑 |
| 删除 RIL 后行为回归 | 高 | T1.10 放在链路跑通后，逐类删逐次编译 |
| 容器迁移遗漏嵌套 Reset | 中 | 批次 3/4 用 FacadeInfo 作金丝雀测试 |
| 性能不达预期（object[] 装箱） | 低 | Phase 5 flat struct 解决，Phase 1 不追求 |

### 铁律

1. **T1.10 删除必须放最后**：ProjectorSystem + Entity/Component 全链路跑通前，不动旧代码
2. **每步编译通过再进下一步**：Source Generator 阶段尤其
3. **批次 4 迁移前先修 Bug**：FlowCollisionInfo 的 Clone 硬编码 Bug 在迁移时自然修复，但应先有测试覆盖
4. **FPVector3 `==` 确认有重载**：属性 setter 中值比较依赖 `==` 操作符，如无则需用 Equals

---

## 10. 文档索引

| 文档 | 定位 |
|------|------|
| `CORE.md` | 哲学底座 |
| `PROPERTY_SYNC_DESIGN.md` | Property Sync 体系完整设计 |
| `BEHAVIORINFO_LIFECYCLE_REPORT.md` | BehaviorInfo 生命周期自动化分析 |
| `IMPLEMENTATION_PLAN.md`（本文） | 实施任务拆解与依赖 |
