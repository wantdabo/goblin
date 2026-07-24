# Property Sync 体系设计

> **删 RIL，标 Project，Entity/Component 镜像。** Logic 几乎不动。

---

## 1. 设计原则

### 1.1 原 RIL 体系的问题

RIL 体系的核心假设是"Logic 和 Render 之间通过类型化消息通信"——每种数据一个 RIL 子类、一个 Translator 去填、一条 Diff 路径去比、一条 Dispatch 路径去发。这导致：

- **~40 个类**只为"把 BehaviorInfo 的字段搬到 Render"
- **3 条互不兼容的 Diff 路径**（hash / IRIL_DIFF / RILCross）
- **Translator 手写 hash + 字段赋值**，漏一个静默 Bug
- **RIL 作为中间层**，新增字段改 3-5 个文件
- **Projection（裁剪）无接入点**，所有 RIL 发给所有人

### 1.2 新体系的核心决策

**BehaviorInfo 直接声明哪些字段同步，Render 侧 Entity/Component 直接收。**

```
旧：BehaviorInfo → Translator → RIL → RILSync → RILBucket → Agent
新：BehaviorInfo → [Project] → ProjectorSystem → 裁剪修饰 → Transport → Entity.Component
```

| | 旧（RIL） | 新（Property Sync） |
|---|---|---|
| Logic 改动 | 写 Translator 填 RIL | BehaviorInfo 字段加 `[Project(index)]` |
| 中间层 | ~10 个 RIL 类 + ~10 个 Translator + ~10 个 Cross | **无中间层** |
| Render 单元 | Agent（生命周期散落 Enchant） | Entity + Component（1:1 映射） |
| Diff | 手写 RIL.Diff(snapshot) | 脏标记（字段写入时记账，无需 Diff） |
| 传输 | RIL 消息 | 属性值数组 + fieldmask |
| 裁剪 | 无 | Crop 规则链（AOI/权限/视野/频率） |
| 删除量 | — | **~40 个类**（整个 RIL 体系 + Agent/Enchant/Invoker/Chase） |

### 1.3 三层架构

```
                Simulation
            (Logic BehaviorInfo)

                   │
                   ▼

            Sync Projection
            (ProjectorSystem + Crop)

                   │
                   ▼

            Presentation
            (Render Entity/Component)
```

- **Simulation**：不动。BehaviorInfo 加 `[Project]` 注解。
- **Sync Projection**：新增。脏标记 → 规则链裁剪 → 传输。
- **Presentation**：重写。删 Agent/Enchant/RILBucket，换 Entity/Component。

---

## 2. Logic 层：`[Project]` 注解

### 2.1 注解体系

```csharp
[Lifecycle]                                 // 类级：接管全部字段的 Reset/Clone
public partial class SpatialInfo : BehaviorInfo
{
    [Project(index: 0)] public FPVector3 position;
    [Project(index: 1)] public FPVector3 euler;
    [Project(index: 2)] public FP scale;

    // 非 [Project]，但 [Lifecycle] 仍自动接管 Reset/Clone
    public SpatialInfo preframe;
}

[Lifecycle]
public partial class AttributeBucketInfo : BehaviorInfo
{
    [Project(index: 0)] public ProjectorDict<ulong, Dictionary<ushort, int>> attributes;
}
```

两个注解正交：

| 注解 | 级别 | 职责 |
|------|------|------|
| `[Project(index)]` | **字段级** | 此字段参与投影同步 → Source Generator 生成脏标记属性 + TakeProjectValues + 序列化 |
| `[Lifecycle]` | **类级** | 当前类全部字段纳入自动池生命周期 → Source Generator 生成 `ResetFields()` + `CloneFields()` |

- **`[Project(index)]` 参数**：`index` 在同一个 BehaviorInfo 类型内唯一，对应 fieldmask 的位。可选 `default` 指定 Reset 时的非零缺省值
- **`[Lifecycle]` 不继承**：每个类自决。父类字段父类负责，子类字段子类负责
- **全有或全无**：标了 `[Lifecycle]` 则全部字段自动接管；不标则全手写。没有逐字段排除
- **容器所有权**：标 `[Lifecycle]` 后容器归 BehaviorInfo 所有，不独立还池。Reset 只清数据（`container.Reset()`）
- **Logic 代码不变**：字段读写走生成属性，但赋值语法与原生字段一致

### 2.2 字段类型

支持的类型及传输策略：

| 值类型 | 传输方式 | 脏标记 |
|--------|---------|------|
| `FPVector3` | 序列化为 3 个 `long` | 属性 setter 自动记账 |
| `FP` | 序列化为 `long` | 同上 |
| `int` / `uint` / `long` / `ulong` | 值类型直接写 | 同上 |
| `bool` | `byte` | 同上 |
| `enum` / `STATE_DEFINE` | 转为 `int` | 同上 |

| 集合类型 | 传输方式 | 变更追踪 |
|---------|---------|------|
| `ProjectorDict<K, V>` | 整包序列化（K,V 为值类型） | **自追踪**：写入即记账，`CollectDiff()` 返回 added/removed/changed |
| `ProjectorList<T>` | 整包序列化（T 为值类型） | 同上 |

| 对象类型 | 当前策略 |
|---------|---------|
| 嵌套 class/struct | **Phase 1 不支持**。后续通过 `[ProjectNested]` 扩展 |

### 2.3 集合容器的自追踪机制

集合字段不使用原生 `Dictionary<K,V>` / `List<T>`，而是使用带变更追踪的包装容器。

**设计理由**：快照方案需要每帧 Clone 整个集合 + 遍历全部 key 比较。1000 个特效条目 → 1000 次分配 + 1000 次 Equals。自追踪容器写入时记账，零快照、零遍历。

**ProjectorDict**：

```csharp
public class ProjectorDict<K, V> : IEnumerable<KeyValuePair<K, V>>
{
    private Dictionary<K, V> data;

    // 本帧变更记录
    private HashSet<K> addedKeys;
    private HashSet<K> removedKeys;
    private HashSet<K> changedKeys;

    public V this[K key]
    {
        get => data[key];
        set
        {
            if (data.TryGetValue(key, out var old))
            {
                if (false == Equals(old, value))
                {
                    data[key] = value;
                    if (false == addedKeys.Contains(key))
                        changedKeys.Add(key);
                }
            }
            else
            {
                data[key] = value;
                addedKeys.Add(key);
                removedKeys.Remove(key);  // 刚删又加 → 抵消
            }
        }
    }

    public bool Remove(K key)
    {
        if (false == data.Remove(key)) return false;
        if (addedKeys.Remove(key)) return true;  // 刚加又删 → 抵消
        changedKeys.Remove(key);
        removedKeys.Add(key);
        return true;
    }

    // ... TryGetValue / ContainsKey / Count / GetEnumerator 透传 ...

    /// <summary>
    /// 收集本帧变更，返回后清空跟踪状态
    /// </summary>
    public DiffResult CollectDiff()
    {
        var r = new DiffResult(addedKeys, removedKeys, changedKeys);
        addedKeys = ObjectPool.Ensure<HashSet<K>>();
        removedKeys = ObjectPool.Ensure<HashSet<K>>();
        changedKeys = ObjectPool.Ensure<HashSet<K>>();
        return r;
    }

    /// <summary>
    /// 清空数据与脏追踪。容器本身不还池（归 BehaviorInfo 所有）。
    /// </summary>
    public void Reset()
    {
        data.Clear();
        addedKeys.Clear();
        removedKeys.Clear();
        changedKeys.Clear();
    }

    /// <summary>
    /// 深拷贝数据，不拷贝脏追踪状态。
    /// </summary>
    public ProjectorDict<K, V> Clone()
    {
        var c = ObjectCache.Ensure<ProjectorDict<K, V>>();
        foreach (var kv in data)
            c.data[kv.Key] = kv.Value;
        return c;
    }
}
```

**ProjectorList**：同理，跟踪 `addedIndices` / `removedIndices`。

**对 BehaviorInfo 的影响**：`FacadeInfo.effectdict` 字段类型从 `Dictionary<uint, EffectInfo>` 改为 `ProjectorDict<uint, EffectInfo>`。Logic 层读写代码不变（`dict[key] = value`），只是类型名不同。如果没有变更操作（集合未被修改），`CollectDiff()` 返回三空集合，mask 位为 0。

### 2.4 Source Generator 生成

Source Generator 按注解组合生成不同内容：

| 注解组合 | 生成内容 |
|---------|---------|
| 仅有 `[Project]` | 属性 + 脏标记 + TakeProjectValues + 序列化 + Render 映射 |
| 仅有 `[Lifecycle]` | `ResetFields()` + `CloneFields()`（接管全部字段） |
| `[Project]` + `[Lifecycle]` | 以上全部 |

**`[Lifecycle]` 不继承**：Source Generator 只扫当前类的直接字段。父类字段由父类自己的 `ResetFields`/`OnReset` 处理。互不侵入。

#### 2.4.1 基类钩子

```csharp
public abstract class BehaviorInfo
{
    public ulong actor { get; private set; }
    public bool active { get; set; }

    public void Reset()
    {
        ResetFields();         // internal virtual — Source Generator 填，用户不动
        OnReset();             // protected virtual — 用户覆写（非 [Lifecycle] 类的字段手动处理）
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

调用链：`Stage.Recycle → info.Reset() → ResetFields()（生成）→ OnReset()（用户）→ actor/active 归零`

#### 2.4.2 `[Lifecycle]` 类生成模板

```csharp
// ── 用户手写 ──
[Lifecycle]
public partial class SpatialInfo : BehaviorInfo
{
    [Project(index: 0, default: 0)] public FPVector3 position;
    [Project(index: 1)] public FPVector3 euler;
    [Project(index: 2, default: 1)] public FP scale;
    public SpatialInfo preframe;                     // 不参与同步，但 Lifecycle 接管
}

// ── Source Generator 生成 ──
public partial class SpatialInfo
{
    // ═══════════════════════ 属性 + 脏标记 ═══════════════════════
    private FPVector3 _position;
    private FPVector3 _euler;
    private FP _scale;
    internal ulong projectDirtyMask { get; set; }

    public FPVector3 position
    {
        get => _position;
        set
        {
            if (_position == value) return;
            _position = value;
            projectDirtyMask |= (1ul << 0);
            Stage.RegisterDirty(this);
        }
    }
    // euler → index 1, scale → index 2（同 pattern）

    // ═══════════════════════ 生命周期 ═══════════════════════
    internal override void ResetFields()
    {
        _position = FPVector3.Zero;
        _euler = FPVector3.Zero;
        _scale = FP.One;
        preframe = null;
        projectDirtyMask = 0;
    }

    internal void CloneFields(SpatialInfo src)
    {
        _position = src._position;
        _euler = src._euler;
        _scale = src._scale;
        preframe = src.preframe;         // 对象引用直接拷贝
        projectDirtyMask = 0;
    }

    // ═══════════════════════ 同步 ═══════════════════════
    // TakeProjectValues / Serialize / Deserialize / TakeSnapshot / CloneSnapshot
}
```

#### 2.4.3 `[Lifecycle]` + 容器字段

```csharp
// ── 用户手写 ──
[Lifecycle]
public partial class FacadeInfo : BehaviorInfo
{
    [Project(index: 0)] public uint model;
    [Project(index: 1)] public ProjectorDict<uint, EffectInfo> effectdict;
    public List<AnimationSlot> animslots;
}

// ── Source Generator 生成 ──
public partial class FacadeInfo
{
    // 属性 + 脏标记（略）

    internal override void ResetFields()
    {
        model = default;
        effectdict.Reset();              // 清数据，不还池
        foreach (var slot in animslots)
            slot.Reset();                // 元素原地 Reset
        animslots.Clear();              // 清列表，不还池
        projectDirtyMask = 0;
    }

    internal void CloneFields(FacadeInfo src)
    {
        model = src.model;
        effectdict = src.effectdict.Clone();
        animslots = ObjectCache.Ensure<List<AnimationSlot>>();
        foreach (var slot in src.animslots)
            animslots.Add(slot.Clone());
        projectDirtyMask = 0;
    }
}
```

#### 2.4.4 容器所有权规则

`[Lifecycle]` 类中的容器字段**不独立还池**。整个池生命周期中容器始终挂在 BehaviorInfo 实例上：

```csharp
// ❌ 旧：容器独立还池 — 反模式（OnReset 还 → OnReady 取，往返浪费）
OnReset() { effectdict.Clear(); ObjectCache.Set(effectdict); }
OnReady() { effectdict = ObjectCache.Ensure<ProjectorDict<...>>(); }

// ✅ 新：只清不还 — Source Generator 生成
ResetFields() { effectdict.Reset(); }  // 清数据，对象不动
```

嵌套深度 3 层的容器全部遵守此规则。

> **default 值**：用户可在注解中声明非零缺省值 `[Project(index: 2, default: 1)]`，ResetFields 时用此值。未声明则值类型用 `default`，ProjectorDict 调 `Reset()`。
>
> **CloneFields 不触发脏标记**：直接写 backing field（`_position`）而非走属性 `set`，确保新实例不会误注册到脏集中。

---

## 3. ProjectorSystem：脏标记 → Packet

### 3.1 每帧流程

**ProjectorSystem 不做 Diff——字段写入时已记账。它只遍历脏集出包。不裁剪。裁剪是 Crop 按 Observer 各自做的。**

```
Logic Tick 期间
    │
    ├─ Behavior 修改 BehaviorInfo 字段
    │    │
    │    └─ 属性 setter 自动（Source Generator 注入）：
    │         projectDirtyMask |= (1ul << index)
    │         Stage.RegisterDirty(this)          ← 注册到全局脏集
    │
    ▼
Logic Tick 结束
    │
    ▼
ProjectorSystem.Tick()                    ← 全局一次
    │
    ├─ 遍历脏 BehaviorInfo（每帧 5-10 个，非 500）
    │    │
    │    ├─ fieldmask = info.projectDirtyMask    ← 直接读，零 Diff
    │    │
    │    ├─ values = info.TakeProjectValues(mask) ← 只读脏字段
    │    │
    │    ├─ 集合字段 → info.CollectCollectionDiffs()
    │    │    （ProjectorDict/ProjectorList 写入时已记账，CollectDiff 仅归集）
    │    │    → 填入 packet.addedkeys / packet.removedkeys
    │    │
    │    ├─ 产出 ProjectorPacket
    │    │    (actor, behaviorType, frame, fieldmask, values)
    │    │
    │    └─ info.ClearProjectDirty()
    │         （projectDirtyMask = 0，脏集移出）
    │
    └─ 产出 List<ProjectorPacket>        ← 全量，未裁剪
    │
    │   ┌──────────────────────────────────────┐
    │   │  从此处分叉：每个 Observer 独立裁剪     │
    │   │                                      │
    │   │  Observer A（Player）→ Crop → Transport│
    │   │  Observer B（GM）    → Crop → Transport│
    │   │  Observer C（Replay）→ Crop → Transport│
    │   └──────────────────────────────────────┘
```

**性能分析**：

| 条目 | 规模 | 操作 | 代价 |
|------|------|------|------|
| 脏标记注册 | 每次字段写入 | 位运算 + HashSet.Add | O(1)，内联于 setter |
| 脏集遍历 | 每帧 5-10 实例（通常） | 读 mask + TakeValues | 可忽略 |
| 集合 Diff | 变动时才发生 | 写入时记账，CollectDiff 仅归集 | O(变动 key 数) |

> 值字段不 Diff、不全遍历、无快照比较。99% 的 BehaviorInfo 本帧不变，零开销。**瓶颈在网络带宽。**

### 3.2 ProjectorPacket

```csharp
public class ProjectorPacket
{
    public ulong actor;
    public Type behaviorType;
    public ulong fieldmask;
    /// <summary>
    /// Logic 帧号。Render 层据此判断时间方向：
    /// frame > renderFrame → 预测，frame <= renderFrame → 插值
    /// </summary>
    public long frame;
    /// <summary>
    /// 数据到达 Render 时的滞后帧数（= renderFrame - frame）。
    /// 帧同步时为 0 或 1；状态同步时为网络 RTT 折算。
    /// Render 层据此调整插值缓冲窗大小和预测步长。
    /// </summary>
    public int latency;
    public object[] values;       // values[i] = 对应 [Project(index: i)] 的当前值
    public List<uint> addedkeys;   // 集合类型：新增的 key
    public List<uint> removedkeys; // 集合类型：移除的 key
}
```

### 3.3 快照与脏集管理

```csharp
public class ProjectorSystem
{
    // 脏集：本帧修改过的 BehaviorInfo。属性 setter 自动注册，ProjectorSystem 消费后清空
    private HashSet<BehaviorInfo> dirtyInfos { get; set; }

    // (actor, behaviorType) → 上帧快照（仅用于 Phase 4 回滚恢复，不用于 Diff）
    private Dictionary<(ulong, Type), BehaviorInfoSnapshot> snapshots { get; set; }
}
```

- **脏集**：BehaviorInfo 属性 setter（Source Generator 注入）自动调用 `Stage.RegisterDirty(this)`。ProjectorSystem Tick 消费后清空。不需要遍历全量 BehaviorInfo
- **快照**：Phase 1 仅用于回滚恢复，不参与字段 Diff。Source Generator 生成 `TakeSnapshot` / `CloneSnapshot`，只拷贝 `[Project]` 字段
- **Actor 移除时**：`ProjectorSystem.RmvActor(actor)` 清理对应快照条目

---

## 4. 裁剪修饰：多层规则链裁剪

裁剪修饰 是 CORE.md 中 World Projector 层的落地实现。同一个 Simulation，不同 Observer 看到的属性集合不同。

### 4.1 单 Observer 输入输出

**Crop 按 Observer 独立执行。每个玩家有自己的裁剪规则链，在 ProjectorSystem 出包后分叉：**

```
ProjectorSystem 产出 ProjectorPacket[]（全量，全局一次）
    │
    ├─ Observer A（Player 1）→ Crop(A规则链) → ObserverPacket[]
    ├─ Observer B（Player 2）→ Crop(B规则链) → ObserverPacket[]
    ├─ Observer C（GM）       → Crop(GodRule)  → ObserverPacket[]
    └─ Observer D（Replay）   → Crop(D规则链) → ObserverPacket[]

单个 Observer 的裁剪过程：

ProjectorPacket (全量, 所有 Actor, 所有属性)

        │
        ▼
┌──────────────────────────────┐
│         裁剪修饰              │
│                               │
│  输入: ProjectorPacket[]           │
│       Observer (规则链绑定)    │
│                               │
│  规则链串联 (见 4.3):         │
│    规则 1: AOI 过滤            │
│        mask &= 规则允许的位    │
│    规则 2: 权限过滤            │
│        mask &= 规则允许的位    │
│    规则 3: 视野过滤            │
│        mask &= 规则允许的位    │
│    规则 4: 频率控制            │
│        mask &= 本帧该发的位    │
│                               │
│  输出: (actor, behaviorType,  │
│         filteredMask, values) │
│        mask == 0 → 丢弃       │
└──────────────┬─────────────────┘
               │
               ▼
        Transport.Send
```

### 4.2 裁决粒度

| 粒度 | 决策 | 触发条件 |
|------|------|---------|
| **Actor 级** | 整条丢弃，不发 | AOI 距离外 / 视野不可见（草丛/隐身） |
| **Field 级** | 修剪 fieldmask | 敌方只能看位置，不能看 HP |
| **频率级** | 部分字段本帧不推送 | 位置 20Hz，背包 1Hz |

四层规则**串联**，每层输入上层的 `(fieldmask, values)`，输出修剪后的。最后 mask == 0 则丢弃整条。

### 4.3 规则接口

```csharp
public interface IProjectionRule
{
    /// <summary>
    /// 裁剪一个 (actor, fieldmask, values)
    /// 返回修剪后的 fieldmask。0 表示整条丢弃
    /// </summary>
    ulong Filter(ProjectorPacket packet, Observer observer, ulong currentMask);
}
```

规则链：

```csharp
public class Crop
{
    private List<IProjectionRule> rules { get; set; }

    public ulong Project(ProjectorPacket packet, Observer observer)
    {
        ulong mask = packet.fieldmask;
        foreach (var rule in rules)
        {
            mask = rule.Filter(packet, observer, mask);
            if (0 == mask) return 0;
        }
        return mask;
    }
}
```

### 4.4 内置规则

| 规则 | 实现概要 |
|------|---------|
| **AOIRule** | 按 Observer 坐标计算与 Actor 距离，超半径则返回 0。可根据"分区"（MMO 地图格）进一步过滤 |
| **PermissionRule** | 查 Observer 与 Actor 的关系（同队/敌方/中立）。维护一张 `(Observer 关系, behaviorType) → 允许的 fieldmask` 配置表。敌方：hp/maxhp 位 mask 掉 |
| **VisibilityRule** | 查 Logic 层（草丛/隐身）标记。Actor 不可见则返回 0 |
| **FrequencyRule** | 每个字段维护各自的推送帧间隔。position 每帧发，背包每 60 帧发。本帧未到 → mask 掉对应位 |
| **GodRule** | GM/Observer 类型为 God → 全通过，不修剪 |

### 4.5 Observer 类型

```csharp
public enum ObserverType
{
    Player,      // 普通玩家：完整规则链
    Spectator,   // 观战：锁定玩家视角
    GM,          // GM：GodRule 全通过
    Replay,      // Replay：时间轴驱动
    AI,          // AI：视野裁剪（作弊 AI 挂 GodRule）
    Editor,      // 编辑器预览
}
```

不同 Observer 绑定不同规则链。Phase 1 所有 Observer 挂 GodRule（零裁剪），后续逐步替换。

---

## 5. 投影策略：预测与插值

预测和插值不是"状态同步才有的东西"，也不是 Transport 的职责。它们是**投影策略**——Render 层根据数据的帧号与当前渲染时间的关系，决定如何平滑消费属性值。

### 5.1 核心原则

Render 层收到的是 `(actor, fieldmask, values[], frame)`。它不问数据从哪来（本地/网络），只问**时间方向**：

```
frame < renderTime  →  插值（Interpolation）：数据在过去，平滑追上
frame > renderTime  →  预测（Prediction）  ：数据在未来，向前推算
frame == renderTime →  直接 Apply       ：恰好对齐
```

### 5.2 帧同步场景：纯插值

```
Logic Tick(N)               Logic Tick(N+1)
    │                            │
    ▼                            ▼
ProjectorPacket(frame=N)        ProjectorPacket(frame=N+1)
    │                            │
    ▼                            ▼
Render 在 N 和 N+1 之间渲染：
    │
    ├─ 收到 frame=N → renderTime > N → 插值
    ├─ 收到 frame=N+1 → renderTime < N+1 → 预测（但本地延迟为 0，实际不触发）
    │
    ▼
SpatialComponent 拿到 lastValue（frame=N）和 nextValue（frame=N+1）
在两者之间 lerp
```

帧同步下 Logic 和 Render 同机，`renderTime ≈ frame`。插值的作用是**同一帧内从上次状态平滑过渡到当前状态**，消除 Logic Tick 间隔的视觉跳跃。

### 5.3 状态同步场景：预测 + 插值

```
Server Tick(N)              Server Tick(N+1)
    │                            │
    │  网络延迟 100ms            │
    ▼                            ▼
Client 收到 frame=N             Client 收到 frame=N+1
(此时 renderTime >> N)          │
    │                            │
    ├─ 收到 N 太晚               ├─ 收到 N+1
    │  → 已经过了，直接 Apply    │  → 与 N 之间插值
    │                            │
    │  但 player input 产生的    │
    │  角色位置在 N 之后：       │
    │  → 客户侧预测位置（Prediction）
    │  → 收到 Server 确认后修正
```

状态同步的预测不是"把数据往前推"，而是**客户端基于上一次收到的快照 + 自身输入，推算出当前时刻的状态**。收到服务端确认后，用插值平滑修正误差。

### 5.4 不同 Observer 的投影策略

| Observer | 策略 | 说明 |
|----------|------|------|
| Player（帧同步） | 插值 | Logic 本地，只需帧间平滑 |
| Player（状态同步） | 预测 + 插值 | 本地输入预测，服务端确认修正 |
| Replay | 插值 | Replay 时间轴驱动，帧间平滑 |
| AI | 快照 Apply | AI 读取离散状态，不渲染 |
| Spectator | 插值 | 锁定玩家视角，跟随目标时间轴 |
| GM | 插值 | 同帧同步 |

### 5.5 Component 侧接口

```csharp
public abstract class Component
{
    // Source Generator 生成：写入值
    public abstract void Apply(ulong fieldmask, object[] values);

    // 用户覆盖：每帧表现（Phase 2+）
    // dt: 渲染帧间隔
    protected virtual void OnExpress(float dt) { }

    /// <summary>
    /// 存储最近两次的属性快照，用于插值/预测/矫正
    /// frame: Logic 帧号，判断时间方向
    /// latency: 滞后帧数，控制缓冲窗大小和平滑因子
    /// </summary>
    internal void PushHistory(long frame, int latency, ulong fieldmask, object[] values)
    {
        // ring buffer 滑动：
        // if (frame > nextFrame) → lastFrame=nextFrame, nextFrame=frame
        // if (frame <= lastFrame) → 回滚帧，刷新 lastFrame
        // OnExpress 内部根据 latency 调整缓冲窗和矫正强度
    }
}
```

Source Generator 为含 `[Project]` 注解的字段生成带时间戳的历史缓冲区（ring buffer，容量 2-4 帧），`OnExpress` 内部根据 `frame` 和 `latency` 自动计算插值/预测/矫正。

### 5.6 关键点

- **不是 Transport 的事**：Transport 只管送，不做时间方向判断
- **不是同步模式的事**：帧同步和状态同步在 Render 用**同一套插值逻辑**
- **是投影策略的事**：不同 Observer 挂不同的时间策略
- **对 Render 透明**：Component 不关心帧同步还是状态同步，只关心 frame 和 renderTime 的关系

### 5.7 延迟感知的算法矫正

Component 在 `OnExpress` 中拿到的不只是 `(lastValue, nextValue)`，还有 `latency`。延迟信息用于自适应矫正：

**Jitter Buffer 自适应窗**：

```
插值缓冲窗 = clamp(baseWindow + latencyVariance * k, minWindow, maxWindow)

latency 稳定（方差小） → 小缓冲窗（1-2 帧），低延迟
latency 抖动（方差大） → 大缓冲窗（4-6 帧），牺牲延迟换平滑
```

**死推算（Dead Reckoning）**：

```
预测位置 = lastPosition + velocity × latency × tickDuration

收到服务端确认后：
误差 = serverPosition - predictedPosition
if (误差 < 阈值) → lerp 平滑修正（不跳变）
if (误差 >= 阈值) → snap 跳正（避免视觉错误持续）
```

**平滑修正（Smooth Correction）**：

```
// 不直接 Apply 服务端值，而是渐进修正
correctionDelta = (serverValue - currentDisplayedValue) * smoothFactor
currentDisplayedValue += correctionDelta
```

三种矫正策略按 Observer 组合：

| Observer | 缓冲窗 | 预测 | 平滑修正 |
|----------|--------|------|---------|
| Player（帧同步） | 1 帧（本地延迟为零） | 不启用 | 不启用 |
| Player（状态同步） | latency 自适应 | 启用（本地输入死推算） | 启用（服务端确认修正） |
| Spectator | latency + 2 帧（保守） | 不启用 | 启用 |
| AI | 0（离散读取） | 不启用 | 不启用 |
| GM | 1 帧（本地） | 不启用 | 不启用 |

**关键**：矫正算法是 Component.OnExpress 内部的纯 Render 逻辑，不依赖 Transport 类型、不依赖同步模式。算法参数（`baseWindow`、`smoothFactor`、阈值）由 Observer 注入，Component 不做 if-else 分叉。

---

## 6. Transport：模式分叉

### 6.1 接口

```csharp
public interface IPropertyTransport
{
    /// <summary>
    /// 发送修剪后的 ProjectorPacket（多 Observer）
    /// </summary>
    void Send(List<ObserverPacket> packets);
}

public class ObserverPacket
{
    public Observer observer;
    public ulong actor;
    public Type behaviorType;
    public ulong fieldmask;
    public long frame;
    public object[] values;
}
```

### 6.2 LocalTransport（帧同步 / 单机）

```csharp
public class LocalTransport : IPropertyTransport
{
    private RenderWorld renderWorld { get; set; }

    public void Send(List<ObserverPacket> packets)
    {
        foreach (var p in packets)
            renderWorld.Apply(p.actor, p.behaviorType, p.frame, p.fieldmask, p.values);
    }
}
```

不序列化、不走网络，直接写入 RenderWorld。

### 6.3 NetworkTransport（状态同步，Phase 2+）

```csharp
public class NetworkTransport : IPropertyTransport
{
    public void Send(List<ObserverPacket> packets)
    {
        foreach (var p in packets)
        {
            using var ms = ObjectPool.Ensure<MemoryStream>();
            var writer = new BinaryWriter(ms);
            writer.Write(p.actor);
            writer.Write(p.frame);
            writer.Write(BehaviorTypeRegistry.GetId(p.behaviorType));
            writer.Write(p.fieldmask);
            // 调用 Source Generator 生成的 Serialize
            ProjectorSerializer.Serialize(p.behaviorType, writer, p.fieldmask, p.values);
            NetworkSend(p.observer.connection, ms.GetBuffer());
        }
    }
}
```

### 6.4 双模共享

帧同步和状态同步**共享同一份** `[Project]` 注解、同一份属性索引、同一份脏标记路径、同一套投影策略。差异只在 Transport 实现：

```
帧同步：  ProjectorSystem → 裁剪修饰 → LocalTransport → RenderWorld
                                                      │
                                          ProjectionStrategy（插值）
状态同步：ProjectorSystem → 裁剪修饰 → NetworkTransport → 网络 → RemoteTransport → RenderWorld
                                                                                │
                                                                ProjectionStrategy（预测 + 插值）
```

---

## 7. Render 层：Entity + Component

### 7.1 Phase 1 范围

> 数据到达 Component，携带时间元信息。**不表达、不插值、不处理回滚。**

Phase 1 唯一目标：Logic 改了 `[Project]` 字段 → 对应 Component 字段自动更新。

Phase 1 已经携带但暂不消费的元信息：

- **`frame`**：Logic 帧号，标记数据产生时间点
- **`latency`**：滞后帧数（= renderFrame - frame），帧同步恒为 0~1，状态同步为网络 RTT 折算
- **`fieldmask`**：变更位，Component.Apply 仅刷新脏字段

Phase 2+ 基于这些元信息开启：

- **插值**（SpatialComponent / AnimationComponent）：根据 `frame` 与 renderFrame 的时间差，在 lastValue 和 nextValue 之间 lerp
- **延迟矫正**（Jitter Buffer）：根据 `latency` 的方差动态调整缓冲窗大小，高延迟时扩大缓冲换平滑
- **模型加载**（FacadeComponent）：modelid 变化 → 异步加载 .tscn/.glb
- **特效创建**（EffectComponent）：读 `addedkeys` / `removedkeys`
- **回滚机制**：快照恢复 + 按 Component 粒度 Flash

### 7.2 删掉的东西

```
当前 Render 层 — 全部删除：

Agent 体系：
  Agent / Agent<T> / SpatialAgent / ModelAgent
  AnimationAgent / EffectAgent / SoundAgent
  PrimitiveMeshAgent / PrimitiveAnimAgent           ~9 个类

Enchant 体系：
  AgentEnchant / SpatialEnchant / ModelEnchant
  AnimationEnchant / EffectEnchant                  ~5 个类

Invoker / Chase 体系：
  Invoker / Invoker<T> / WatchRIL / DoRIL
  Chase / OnChase / OnArrived / ChaseStatus         ~8 个类

Batch / Bucket 体系：
  SpatialBatch / Batch
  RILBucket / RILBucket.SetRIL / RILDispatch       ~5 个类

其他：
  RILSalute / Salute / IRIL_EVENT 转发              ~5 个类

────────────────────────────────────────────────
  ~32 个类删除
```

### 7.3 Entity

Actor 的容器，关联一组 Component。

```csharp
public class Entity
{
    public ulong actor { get; }

    private Dictionary<Type, Component> comps { get; set; }

    internal Entity(ulong actor)
    {
        this.actor = actor;
        comps = ObjectPool.Ensure<Dictionary<Type, Component>>();
    }

    public T GetComp<T>() where T : Component
    {
        if (comps.TryGetValue(typeof(T), out var c))
            return c as T;
        return null;
    }

    internal void AddComp(Component comp)
    {
        comp.entity = this;
        comps[comp.GetType()] = comp;
        comp.OnCreate();
    }

    internal void RmvComp<T>() where T : Component
    {
        if (comps.TryGetValue(typeof(T), out var comp))
        {
            comp.OnDestroy();
            comps.Remove(typeof(T));
        }
    }

    internal void Destroy()
    {
        foreach (var comp in comps.Values)
            comp.OnDestroy();
        comps.Clear();
        ObjectPool.Set(comps);
    }
}
```

### 7.4 Component

纯数据容器。Phase 1 不做表现。

```csharp
public abstract class Component
{
    public Entity entity { get; internal set; }
    public ulong actor => entity.actor;

    /// <summary>
    /// Source Generator 生成的方法。
    /// 按 fieldmask 将 values[] 写入 Component 字段
    /// </summary>
    public abstract void Apply(ulong fieldmask, object[] values);

    /// <summary>
    /// 创建时调用（Entity.AddComp 触发）
    /// </summary>
    protected virtual void OnCreate() { }

    /// <summary>
    /// 销毁时调用
    /// </summary>
    protected virtual void OnDestroy() { }
}
```

### 7.5 BehaviorInfo → Component 映射

Source Generator 根据 `[Project]` 注解的位置生成映射表：

```
SpatialInfo     [Project(0)] position  ──→  SpatialComponent.position
                [Project(1)] euler     ──→  SpatialComponent.rotation
                [Project(2)] scale     ──→  SpatialComponent.scale

FacadeInfo      [Project(0)] model     ──→  FacadeComponent.modelid
                [Project(1)] effects   ──→  EffectComponent.effectdict

TickerInfo      [Project(0)] timescale ──→  TickerComponent.timescale
```

如果某个 BehaviorInfo 没有对应的 Component 需求，可以不创建映射（Sync 数据仅用于网络传输）。

Component 示例：

```csharp
// 用户手写
public partial class SpatialComponent : Component
{
    public Vector3 position;
    public Quaternion rotation = Quaternion.Identity;
    public float scale = 1f;
}

// Source Generator 生成
public partial class SpatialComponent
{
    public override void Apply(ulong fieldmask, object[] values)
    {
        if (0 != (fieldmask & (1ul << 0)))
            position = ((FPVector3)values[0]).ToVector3();
        if (0 != (fieldmask & (1ul << 1)))
            rotation = Quaternion.FromEuler(
                ((FPVector3)values[1]).ToVector3() * MathF.PI / 180f);
        if (0 != (fieldmask & (1ul << 2)))
            scale = ((FP)values[2]).AsFloat();
    }
}
```

### 7.6 RenderWorld

管理所有 Entity，接收 Transport 推送的 Sync 数据。

```csharp
public class RenderWorld
{
    private Dictionary<ulong, Entity> entities { get; set; }
    private Dictionary<Type, Type> behaviorToComp { get; set; }

    public RenderWorld()
    {
        entities = ObjectPool.Ensure<Dictionary<ulong, Entity>>();
        behaviorToComp = ObjectPool.Ensure<Dictionary<Type, Type>>();
        // Source Generator 注册映射
        RegisterMappings();
    }

    private partial void RegisterMappings();

    /// <summary>
    /// 应用 ProjectorPacket 到 Entity.Component。
    /// latency 由 Transport 层计算（本帧 renderFrame - packet.frame），
    /// 帧同步恒为 0~1，状态同步为网络 RTT 折算。
    /// </summary>
    public void Apply(ulong actor, Type behaviorType, long frame, int latency, ulong fieldmask, object[] values)
    {
        // 1. 确保 Entity 存在
        if (false == entities.TryGetValue(actor, out var entity))
        {
            entity = new Entity(actor);
            entities[actor] = entity;
            OnEntityCreated?.Invoke(entity);
        }

        // 2. 确保 Component 存在
        if (false == behaviorToComp.TryGetValue(behaviorType, out var compType))
            return;  // 此 BehaviorInfo 不需要 Render Component

        var comp = entity.GetComp(compType);
        if (null == comp)
        {
            comp = ObjectPool.Ensure(compType) as Component;
            entity.AddComp(comp);
        }

        // 3. 推入历史缓冲区（含 latency，用于插值/预测/矫正）
        comp.PushHistory(frame, latency, fieldmask, values);
    }

    /// <summary>
    /// 移除 Actor
    /// </summary>
    public void RmvEntity(ulong actor)
    {
        if (entities.TryGetValue(actor, out var entity))
        {
            entity.Destroy();
            entities.Remove(actor);
            OnEntityRemoved?.Invoke(entity);
        }
    }

    public Entity GetEntity(ulong actor) =>
        entities.TryGetValue(actor, out var e) ? e : null;

    // 事件钩子（Phase 2+ 的表现层订阅）
    public event Action<Entity> OnEntityCreated;
    public event Action<Entity> OnEntityRemoved;
}
```

---

## 8. 完整管线时序

### 8.1 正常帧

```
Logic Tick
    │
    ├─ Behavior 更新 BehaviorInfo 字段
    │
    ▼
ProjectorSystem.Tick()
    │
    ├─ 遍历脏 BehaviorInfo（全局脏集，每帧 5-10 个）
    ├─ 读 projectDirtyMask → fieldmask（零 Diff）
    ├─ 产出 ProjectorPacket[]（含 frame）
    │
    ▼
裁剪修饰.Project(packets, observers)
    │
    ├─ Observer A × 规则链 → 修剪后的 packets
    ├─ Observer B × 规则链 → 修剪后的 packets
    │
    ▼
Transport.Send(observerPackets)
    │
    ├─ LocalTransport → RenderWorld.Apply()
    ├─ NetworkTransport → Serialize → 网络
    │
    ▼
RenderWorld.Apply(actor, behaviorType, frame, latency, fieldmask, values)
    │
    ├─ Ensure Entity
    ├─ Ensure Component
    ├─ Component.PushHistory(frame, latency, fieldmask, values)
    │    │
    │    └─ 存入 ring buffer（lastFrame→nextFrame），附带 latency
    │
    ▼
（Phase 2+：Component.OnExpress(dt)
    │  [latency → Jitter Buffer 自适应窗]
    │  [latency → 死推算步长]
    │  [latency → 平滑修正的 smoothFactor]）
    │  根据 renderTime 与 lastFrame/nextFrame 的关系
    │  自动计算插值 t 或预测偏移量）
```

### 8.2 Actor 创建/销毁

```
Logic: Actor 出生
    │
    ├─ Stage 创建 BehaviorInfo 实例
    │
    ▼
ProjectorSystem.Tick()
    ├─ 新 BehaviorInfo → 首次注册时 projectDirtyMask 置为全 1 ← 全量同步
    │   （Source Generator 生成的 Init 方法在构造后自动调用）
    ├─ 产出 ProjectorPacket（全量）
    │
    ▼
RenderWorld.Apply()
    ├─ Entity 不存在 → new Entity + new Component
    ├─ OnEntityCreated 事件触发
    │
    ▼
（Phase 2+：加载模型、创建 Node3D）

──────────────────────────

Logic: Actor 死亡
    │
    ├─ Stage 移除 BehaviorInfo
    │
    ▼
ProjectorSystem.RmvActor(actor)
    ├─ 清理快照
    ├─ 产出 RemovalPacket
    │
    ▼
RenderWorld.RmvEntity(actor)
    ├─ Entity.Destroy()
    ├─ OnEntityRemoved 事件触发
```

---

## 9. 与 RIL 体系的对比

| 指标 | RIL 体系 | Property Sync |
|------|---------|---------------|
| BehaviorInfo 改动 | 写 Translator 类 | 加 `[Project]` 注解 |
| 中间类数量 | ~10 RIL + ~10 Translator + ~10 Cross | 0（Source Generator 生成） |
| Render 类数量 | ~32（Agent/Enchant/Invoker/Chase） | 3（Entity/Component/RenderWorld） |
| 新增同步字段 | 改 RIL 类 + Translator + Diff + Merge（3-5 文件） | 加一个 `[Project(index)]` |
| Diff 漏字段 | 静默 Bug（不同步，不报错） | Source Generator 生成，不遗漏 |
| 裁剪 | 无接入点 | 裁剪修饰规则链原生支持 |
| 传输 | RILCache + rilqueue | 属性值数组 + fieldmask |
| 回滚 | Flash + frame 去重（全 Agent 抖动） | Phase 2+ 按 Component 粒度 |

---

## 10. 迁移计划

### Phase 1（~7 天）：基础管线 + Entity/Component

1. **`[Project]` + `[Lifecycle]` Attribute** 定义 + Source Generator 框架
2. **BehaviorInfo 基类钩子**：`ResetFields()`（internal virtual）+ `OnReset()`（protected virtual）
3. **ProjectorSystem**：脏标记 → ProjectorPacket 产出（含 `frame`）
4. **Crop**：接口 + GodRule（Phase 1 零裁剪）
5. **IPropertyTransport** + LocalTransport
6. **Entity + Component + RenderWorld**：
   - 删 `Agent`/`Enchant`/`Invoker`/`Chase`/`SpatialBatch`/`RILBucket`
   - 用户手写 `SpatialComponent` / `FacadeComponent` / `EffectComponent` 等（Phase 1 纯数据容器）
   - Source Generator 生成 `Apply` 方法
   - Component 加 `PushHistory`（ring buffer，容量 2 帧，留作 Phase 2 插值用）
7. **删除 RIL 体系**：`IRIL` 及所有子类 / `Translator` 及所有子类 / `RILSync` / `RIL_DEFINE` / `RILCache` / `RILCross` / `IRIL_DIFF`
8. **`[Lifecycle]` 类迁移**：按 4 批次逐步标 `[Lifecycle]`，Source Generator 接管 Reset/Clone

**验收**：Logic 改 `SpatialInfo.position`，下一帧 `SpatialComponent.position` 自动更新。`[Lifecycle]` 类的 Reset 方法零手写。

### Phase 2（~5 天）：统一投影策略 + 表现层

1. **ProjectionStrategy**：`OnExpress` 读取 lastFrame/nextFrame，计算插值 t 或预测偏移
2. SpatialComponent 加插值（position / rotation lerp，帧同步/状态同步同一套逻辑）
3. FacadeComponent 加模型加载（`modelid` 变化 → 加载 .tscn / .glb）
4. EffectComponent 加特效创建/回收（读 `addedkeys` / `removedkeys`）
5. AnimationComponent 加动画推进
6. 分层 Express（Phase A/B/C/D，依赖 IBindDependencies）

### Phase 3（~4 天）：裁剪规则 + 状态同步传输

1. AOIRule / PermissionRule / VisibilityRule / FrequencyRule 实现
2. Observer 类型工厂 + 规则链绑定
3. NetworkTransport（序列化 + 网络发送，接收端按 frame 推入 RenderWorld）
4. **预测策略**：Player Observer 在状态同步下启用输入预测（客户端推演本地输入 → 服务端确认后插值修正）

### Phase 4（~3 天）：回滚机制

1. ProjectorSystem 快照回滚（帧同步 rollback 时恢复上帧快照）
2. RenderWorld 回滚：按 dirty actor 标记，只 Flash 受影响的 Entity.Component
3. 事件幂等（frame 去重）

### Phase 5（~2 天，可选）

1. `ProjectState` 扁平 struct：将 `[Project]` 字段打包为 struct，快照/序列化 memcpy 量级
2. 嵌套对象支持 `[ProjectNested]`
3. 性能验证与边缘 case 覆盖

**总计：~22 天**（比 RIL 重构方案 30 天少 8 天，因为删的比写的多）。

---

## 11. 关键数字

| 项目 | 数值 |
|------|------|
| 删除类/接口 | ~72（RIL ~40 + Render ~32） |
| 新增类/接口 | ~18（ProjectorSystem / Crop / IProjectionRule / Rule × 5 / IPropertyTransport / Transport × 2 / Entity / Component / RenderWorld / ProjectorPacket / ObserverPacket / DiffResult / BehaviorInfoSnapshot） |
| [Project] 注解手写量 | 每个 BehaviorInfo 3-15 个字段 |
| [Lifecycle] 注解 | 类级一个（24 个类逐步标） |
| 手写生命周期方法 | 72 个 → 0 |
| Logic 层改动量 | 仅加注解，不动逻辑 |
| Phase 1 天数 | ~7 天 |

---

## 12. 不动的清单

| 模块 | 说明 |
|------|------|
| Behavior 生命周期 | 照旧：OnAssemble/OnTick/OnEndTick/OnDisassemble |
| BehaviorInfo 生命周期 | **自动化**：`[Lifecycle]` 类由 Source Generator 接管 Reset/Clone |
| Stage / World / Cache | 照旧 |
| Actor 管理 | 照旧 |
| 定点数 FP / FPVector3 / FPQuaternion | 照旧 |
| 配置表 / StateMachine / Skill 系统 | 照旧 |
| 帧同步确定性 | 照旧（ProjectorSystem 不参与 Logic 计算） |

---

## 13. 文档索引

| 文档 | 定位 |
|------|------|
| `CORE.md` | 哲学底座：Simulation → Projection → Presentation |
| `PROPERTY_SYNC_DESIGN.md`（本文） | Property Sync 体系完整设计 |
| `BEHAVIORINFO_LIFECYCLE_REPORT.md` | BehaviorInfo 生命周期自动化分析 |
| `IMPLEMENTATION_PLAN.md` | 实施任务拆解与依赖 |
