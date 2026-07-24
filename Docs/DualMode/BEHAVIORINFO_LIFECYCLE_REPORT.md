# BehaviorInfo 生命周期分析报告

> **问题**：逻辑层 24 个 BehaviorInfo 子类的手写 Reset/Clone/Ready 样板代码量过大，字段增删易遗漏，已发现多个 Bug。

---

## 1. 现状

### 1.1 规模

| 项目 | 数量 |
|------|------|
| BehaviorInfo 子类总数 | 24 |
| 简单值类型类（无容器字段） | 12 |
| 含容器字段类（List/Dictionary 等） | 11 |
| 未实现生命周期 | 1（SkillCooldownInfo） |

### 1.2 每种生命周期方法手写量

每个 BehaviorInfo 子类必须手写三个方法：

```
OnReady()    ← 从池取出时初始化
OnReset()    ← 回池前清理
OnClone()    ← 快照深拷贝
```

24 × 3 = **72 个手写方法**。

### 1.3 容器字段嵌套回收的复杂度

简单类型（12 个类）只需逐字段清零：

```csharp
// TickerInfo.OnReset — 1 字段，还行
protected override void OnReset()
{
    timescale = FP.One;
}
```

容器类型（11 个类）必须逐层回收嵌套结构。以 `FacadeInfo.OnReset` 为例：

```csharp
protected override void OnReset()
{
    model = 0;
    animticktype = ANIM_DEFINE.TICK_AUTOMATIC;
    animstate = 0;
    animhash = 0;
    animelapsed = FP.Zero;
    effectincrement = 0;

    rmveffects.Clear();
    ObjectCache.Set(rmveffects);           // 回池 List<uint>

    effects.Clear();
    ObjectCache.Set(effects);              // 回池 List<uint>

    effectdict.Clear();
    ObjectCache.Set(effectdict);           // 回池 Dictionary<uint, EffectInfo>

    foreach (var slot in animslots)
        ObjectCache.Set(slot);             // 每个 AnimationSlot 逐个回池
    animslots.Clear();
    ObjectCache.Set(animslots);            // 回池 List<AnimationSlot>
}
```

嵌套深度分布：

| 嵌套层级 | 类数 | 代表 |
|---------|------|------|
| 1 层（简单容器） | 6 | EventorInfo, SeatInfo, CareerInfo |
| 2 层 | 3 | TagInfo, GamepadInfo, FlowEffectInfo |
| 3 层 | 4 | FacadeInfo, AttributeBucketInfo, FlowInfo, StageInfo |
| 递归嵌套（Dictionary → List → 需要遍历回收） | 2 | SilentMercyInfo, FlowCollisionInfo |

---

## 2. 已发现的 Bug

### 2.1 FlowCollisionInfo.OnClone 硬编码子类类型

```csharp
// FlowCollisionInfo.OnClone() — 第 73 行
protected override BehaviorInfo OnClone()
{
    var clone = ObjectCache.Ensure<FlowCollisionHurtInfo>();  // BUG！
    clone.Ready(actor);
    // ... 拷贝字段 ...
}
```

`FlowCollisionSensorInfo` 也继承 `FlowCollisionInfo`，但它没有重写 `OnClone`。调用 `FlowCollisionSensorInfo.Clone()` 会返回 `FlowCollisionHurtInfo` 实例，类型错误且多出不该有的字段。

### 2.2 FlowCollisionHurtInfo 子类字段未在 OnReset 中清理

`FlowCollisionHurtInfo` 声明了 6 个额外字段（`usesparkself`、`sparkselfinfluence`、`sparkselftoken`、`usesparktarget`、`sparktargetfluence`、`sparktargettoken`），但：
- 父类 `FlowCollisionInfo.OnReset` 不处理这些字段
- 子类没有重写 `OnReset`

对象池复用后这些字段会保留上一轮的值。

### 2.3 OnReady 调 OnReset 的反模式

8 个类的 `OnReady()` 委托给 `OnReset()`：

```csharp
// SpatialInfo — 值类型，可以这么干
protected override void OnReady()
{
    OnReset();
}
```

这对值类型类没问题，但对容器类型类（如 FacadeInfo）意味着：刚 `ObjectCache.Ensure` 出来的容器立刻被 `OnReset` 的 `Clear() + ObjectCache.Set()` 销毁，然后又要重新 `Ensure`。浪费一次池操作。

---

## 3. 根因：三个生命周期方法高度冗余

每个 BehaviorInfo 的 `OnReset` / `OnClone` / `OnReady` 都遵循相同模式，差异只在字段列表：

| 操作 | 模式 A：值类型 | 模式 B：容器 |
|------|-------------|------------|
| OnReady | 调用 OnReset | ObjectCache.Ensure 容器 + 手动初始化 |
| OnReset | `field = default` | 递归 `container.Clear()` + `ObjectCache.Set(container)` + 值字段归零 |
| OnClone | `ObjectCache.Ensure<T>()` → `clone.Ready(actor)` → 逐字段拷贝 | 同上 + 容器 `AddRange` / `foreach Add` 深拷贝 |

这完全是**字段元数据驱动的机械操作**。Source Generator 完全可以生成。

---

## 4. 解决方案：`[Lifecycle]` 类级注解 + Source Generator

### 4.1 设计决策

| 决策 | 理由 |
|------|------|
| **类级注解** | 一个 BehaviorInfo 的所有字段（包括容器嵌套）统一接管，观感干净；不需要字段级 `[Lifecycle]` |
| **不继承** | 每个类自决是否标 `[Lifecycle]`；父类字段父类负责，子类字段子类负责，互不侵入 |
| **对象池保留** | BehaviorInfo 和内部容器都走池；容器不独立还池，始终跟随 BehaviorInfo 实例 |
| **Reset 只清不还** | 容器 `.Reset()` 清数据，不调 `ObjectCache.Set()`；取消容器到池的往返 |

### 4.2 注解体系

```
[Lifecycle]      类级 — 接管当前类全部字段的 Reset/Clone
[Project(index)] 字段级 — 此字段参与投影同步（与 Lifecycle 正交）
```

**全有或全无**：标了 `[Lifecycle]` 则全部字段自动接管；不标则全手写。没有逐字段排除——一个字段如果不需要 Reset，说明它不属于 BehaviorInfo 状态。

### 4.3 容器所有权规则

容器字段归 BehaviorInfo 所有。整个池生命周期中容器始终挂在实例上。

```csharp
// ❌ 旧：容器独立还池 — 反模式
OnReset()
{
    effectdict.Clear(); ObjectCache.Set(effectdict);  // 还池
}
OnReady()
{
    effectdict = ObjectCache.Ensure<ProjectorDict<...>>();  // 重新拿
}

// ✅ 新：容器只清不还 — Source Generator 生成
internal override void ResetFields()
{
    effectdict.Reset();  // 清数据，ProjectorDict 对象不动
}
```

嵌套深度 3 层的容器全部遵守此规则。

---

## 5. 基类钩子设计

`BehaviorInfo.Reset()` 拆两个钩子，避免 Source Generator 与用户 override 冲突：

```csharp
public abstract class BehaviorInfo
{
    public ulong actor { get; private set; }
    public bool active { get; set; }

    public void Reset()
    {
        ResetFields();         // internal virtual — Source Generator 填，用户不动
        OnReset();             // protected virtual — 用户覆写（可选）
        actor = 0;
        active = false;
    }

    /// <summary>
    /// Source Generator 为 [Lifecycle] 类生成 override。
    /// 非 [Lifecycle] 类保持空实现。
    /// </summary>
    internal virtual void ResetFields() { }

    /// <summary>
    /// 用户覆写。非 [Lifecycle] 类的字段手动处理。
    /// </summary>
    protected virtual void OnReset() { }
}
```

调用链（`Stage.Recycle` 触发）：

```
info.Reset()
    │
    ├─ ResetFields()    ← 生成（[Lifecycle] 类）或空（非 [Lifecycle] 类）
    │
    ├─ OnReset()        ← 用户覆写（非 [Lifecycle] 类的字段手动处理）
    │
    └─ actor = 0; active = false
```

---

## 6. 两个场景

### 场景 1：`[Lifecycle]` — 全自动

```csharp
[Lifecycle]
public partial class SpatialInfo : BehaviorInfo
{
    [Project(index: 0)] public FPVector3 position;
    [Project(index: 1)] public FPVector3 euler;
    [Project(index: 2)] public FP scale;
    public SpatialInfo preframe;
}

// Source Generator 生成：
partial class SpatialInfo
{
    internal override void ResetFields()
    {
        _position = FPVector3.Zero;
        _euler = FPVector3.Zero;
        _scale = FP.One;
        preframe = null;
        projectDirtyMask = 0;
    }
    // 不生成 OnReset
}
```

用户零代码。容器字段同规则——嵌套 3 层全自动。

### 场景 2：非 `[Lifecycle]` — 全手写

```csharp
public class CareerInfo : BehaviorInfo
{
    public uint career;
}

// 用户手写：
partial class CareerInfo
{
    protected override void OnReset()
    {
        career = 0;
    }
}
```

`ResetFields()` 保持基类空实现。

---

## 7. Clone 自动化

Source Generator 同时为 `[Lifecycle]` 类生成 Clone 方法，与 Reset 对应：

```csharp
[Lifecycle]
public partial class FacadeInfo : BehaviorInfo
{
    [Project(index: 0)] public uint model;
    [Project(index: 1)] public ProjectorDict<uint, EffectInfo> effectdict;
    public List<AnimationSlot> animslots;
}

// Source Generator 生成（直接写 backing field，不触发脏标记）：
partial class FacadeInfo
{
    internal FacadeInfo CloneFields(FacadeInfo src)
    {
        _model = src._model;
        _effectdict = src._effectdict.Clone();  // ProjectorDict.Clone()
        // animslots：李深拷贝元素
        animslots = ObjectCache.Ensure<List<AnimationSlot>>();
        foreach (var slot in src.animslots)
            animslots.Add(slot.Clone());
        projectDirtyMask = 0;
        return this;
    }

    // Clone 入口（基类提供框架）：
    public override BehaviorInfo Clone()
    {
        var clone = ObjectCache.Ensure<FacadeInfo>();
        clone.CloneFields(this);
        clone.Ready(actor);
        return clone;
    }
}
```

---

## 8. 迁移策略

### Phase 1：Source Generator 支持 `[Project]` 字段

1. 实现基础框架：属性注入 + 脏标记 + `ResetFields` + `CloneFields`
2. `[Lifecycle]` 只生成 Project 字段的 Reset/Clone（此时非 Project 字段仍需手写）
3. 非 `[Lifecycle]` 类全部手写照旧

### Phase 2：`[Lifecycle]` 注解扩展

1. 支持类级 `[Lifecycle]`
2. Source Generator 扩展 `ResetFields` / `CloneFields` 覆盖全部字段
3. 批量迁移 24 个 BehaviorInfo 子类

按复杂度从低到高：

| 批次 | 类 | 字段特征 | 风险 |
|------|-----|---------|------|
| 1 | TickerInfo, MovementInfo, MagicInfo | 纯值类型，1-2 字段 | 零 |
| 2 | SpatialInfo, StateMachineInfo, SkillLauncherInfo 等 | 值类型 + struct | 低 |
| 3 | TagInfo, GamepadInfo | 1-2 层容器 | 中 |
| 4 | FacadeInfo, StageInfo, FlowCollisionInfo 系列 | 深层嵌套容器 | 高（含已知 Bug） |

---

## 9. 同时修复的 Bug

迁移过程中自然消除：

| Bug | 如何修复 |
|-----|---------|
| FlowCollisionInfo.OnClone 硬编码子类类型 | Source Generator 按实际类型生成 `CloneFields`，用 `ObjectCache.Ensure<实际类型>()` |
| FlowCollisionHurtInfo 子类字段遗漏 | `[Lifecycle]` 接管当前类全部字段，不依赖父类 OnReset |
| OnReady 调 OnReset 反模式 | 容器不还池（只 `Reset()`），无往返 |

---

## 10. 关键数字

| 项目 | 数值 |
|------|------|
| 手写生命周期方法 | 72 个，迁移后降至 0 |
| 已知 Bug | 3 个（全部自然消除） |
| 容器嵌套最大深度 | 3 层（FacadeInfo / FlowInfo / StageInfo） |
| 迁移总周数 | ~4 天（嵌入 Phase 1 Property Sync 实施中） |

---

## 11. 文档索引

| 文档 | 定位 |
|------|------|
| `CORE.md` | 哲学底座 |
| `PROPERTY_SYNC_DESIGN.md` | Property Sync 体系完整设计 |
| `BEHAVIORINFO_LIFECYCLE_REPORT.md`（本文） | BehaviorInfo 生命周期自动化分析 |
| `IMPLEMENTATION_PLAN.md` | 实施任务拆解与依赖 |
