# RIL 同步体系设计（迭代版）

## 0. 设计原则

Goblin 需要同时支持**帧同步**和**状态同步**两种网络模型。RIL 体系是两种模式共享的基础设施。

```
帧同步 (Lockstep)                        状态同步 (State Sync)
─────────────────                        ─────────────────────
网络只传输入                              网络传状态差量
各端跑完整确定性模拟                       服务端跑权威模拟
Logic 用定点数保证确定性                    客户端收状态快照/增量

RIL 的角色：纯本地通信                     RIL 的角色：本地 + 网络传输
Logic → RIL → Render（同机）              Server Logic → 序列化 → 网络
                                          Client 反序列化 → RILBucket → Render
```

对 RIL 体系的核心要求：

| 要求 | 帧同步 | 状态同步 |
|------|--------|---------|
| RIL 序列化 | 不需要——本地直接传引用 | 必须——网络传输 |
| 字段级 Diff | 锦上添花——减少本地拷贝 | 关键——带宽优化 |
| 传输延迟 | 零——同帧到达 | 有——需要插值/预测 |
| 确定性 | 必须——Logic 定点数 | 不需要——服务端权威 |
| RIL 定义 | 同一份 | 同一份 |

**核心结论**：RIL 定义和 Translator 两种模式完全共享，差异只在**传输层**——帧同步是本地函数调用，状态同步是序列化+网络。传输层必须可插拔。

```
                    ┌──────────────────────────┐
                    │      RIL Definition        │  ← 两种模式共享
                    └────────────┬─────────────┘
                                 │
                    ┌────────────▼─────────────┐
                    │       Translator          │  ← 两种模式共享
                    │   (Info → RIL 填充)       │
                    └────────────┬─────────────┘
                                 │
                    ┌────────────▼─────────────┐
                    │        RILSync            │  ← 两种模式共享
                    │   Snapshot Diff (统一)    │
                    └────────────┬─────────────┘
                                 │
                    ┌────────────┴─────────────┐
                    │     IRILTransport          │  ← 模式分叉点
                    └───┬──────────────────┬───┘
                        │                  │
            ┌───────────▼──────┐  ┌────────▼──────────┐
            │  帧同步 (Local)   │  │ 状态同步 (Network) │
            │  直接 onril      │  │ Serialize → Send   │
            │  → RILBucket     │  │ → Client Deserialize│
            └──────────────────┘  └───────────────────┘
```

---

## 1. 核心决策：统一走 Snapshot Diff，干掉所有特例路径

现状有三条路径，本方案合并为一条：

| 现状路径 | 用途 | 本方案 |
|---------|------|--------|
| Full RIL（hash + 整体替换） | 值类 | 合入 Snapshot Diff（Diff 算 fieldmask） |
| DIFF RIL（IRIL_DIFF + RILCross） | 集合类 | **删除**，集合类也走 Snapshot Diff（Diff 算 added/removed） |
| hash 比较 | 判变化 | 删除，由 Diff 替代 |

**统一路径**：

```
Translator 填 RIL（值类填字段 / 集合类填集合）
    ↓
rilsync.Send(ril)
    ↓ 按 (actor, id) 查 snapshot
    ↓ ril.Diff(snapshot) → fieldmask
    ↓ 0 则回收不发；非 0 则 transport.Send + 更新 snapshot
transport（Local/Network）
    ↓ LocalTransport: 直接调 RILBucket.SetRIL
    ↓ NetworkTransport: Serialize(fieldmask 标记字段) → 网络
RILBucket.SetRIL
    ↓ oldril.Merge(ril, fieldmask)  ← 按 fieldmask 合并到已存 RIL
    ↓ RILDispatch（带 fieldmask）
Agent
    ↓ 读 fieldmask 做字段级响应
```

**值类**：Diff 是字段比较，Merge 是字段赋值。
**集合类**：Diff 是集合比较（算 added/removed），Merge 是集合增删。added/removed 作为 RIL 自身字段，不需要独立载体。
**高频类（SPATIAL）**：Diff 实现直接返回全 1（跳过比较，因为必 dirty），仍走统一路径，只是 Diff 实现不同。

**没有特例，没有 Cross，没有 IRIL_DIFF**。

---

## 2. 现状痛点

### 2.1 三种数据，三处真相

```
Logic 层                    传输                     Render 层
┌──────────────┐     ┌──────────────┐     ┌──────────────┐
│ BehaviorInfo │ ──→ │     RIL      │ ──→ │ RILBucket    │
│ (活的，每帧变) │     │ (临时消息)    │     │ (存储的状态)  │
└──────────────┘     └──────────────┘     └──────────────┘
```

每种数据类型在三个地方各有一份表示。同步的本质是：**Logic 改了 → Render 要知道改了什么**。

### 2.2 当前两条互不相通的路径

**路径 1 — Full RIL（值类）**：

```
Translator.OnCalcHashCode(info)     ← 手动遍历字段算 hash
    ↓ hash 变了
Translator.OnRIL(info, ril)         ← 手动逐字段赋值
    ↓
RILSync.Send(ril)                   ← 直接 onril
    ↓
RILBucket.SetRIL(ril)               ← hash 比较，不同则整体替换
```

**路径 2 — DIFF RIL（集合类，本方案要删除）**：

```
Behavior 主动调 Stage.Diff()        ← 显式触发
    ↓
RILSync.Send(IRIL_DIFF)             ← 进 diffqueue
    ↓ Translate() 末尾
stage.ondiff(clone)
    ↓
RILBucket.SetDiff(diff)             ← crossdict 找 RILCross
    ↓
RILCross.HasNew/HasDel(ril, diff)   ← 手动合并进已存储的 RIL
```

### 2.3 手写代码量统计

新增一个 RIL 类型，人需要手写的代码：

| 代码 | 位置 | 行数 | 容易出错？ |
|------|------|------|-----------|
| RIL 类定义 | `RIL_XXX.cs` | ~30 | 低 |
| `OnCalcHashCode` | `XXXTranslator.cs` | ~15-40 | **高** — 字段漏算、顺序错 |
| `OnRIL` 填字段 | `XXXTranslator.cs` | ~10-20 | **高** — 字段漏填 |
| `OnReady/OnReset` | `RIL_XXX.cs` | ~10 | 低 |
| 如果是集合类 | `RIL_DIFF_XXX.cs` | ~20 | 中 |
| 如果是集合类 | `XXXCross.cs` | ~30 | **高** — 合并逻辑错误 |
| 注册 | `RIL_DEFINE.cs` | +1 | 低 |

一个字段改动至少改 3 个文件。漏改任何一个，bug 是静默的——不报错，只是数据不同步。

### 2.4 痛点拆解

**痛点 1：Hash 计算对高频 RIL 是浪费**

`SPATIAL` 几乎每帧都变，hash 算出来总是"变了"。算 hash 遍历所有字段，然后 `OnRIL` 又把这些字段赋给 RIL——同样的字段被遍历两遍。

**痛点 2：AttributeTranslator 已经「破窗」**

```csharp
// AttributeTranslator.cs — 完全绕过了 Translator<T,E> 的泛型模式
protected override void OnRIL(BehaviorInfo info)  // 重写了基类的非泛型方法
{
    if (info is not AttributeBucketInfo bucket) return;
    foreach (var kv in bucket.attributes) { /* 自己遍历、自己算 hash、自己 Send */ }
}
protected override void OnRIL(AttributeBucketInfo info, RIL_ATTRIBUTE ril) { }  // 空实现
```

破窗根因不是 Snapshot Diff 不支持 N 产出，而是现状的 hash 机制 + `Translator<T,E>` 泛型假设一对一。换 Diff 后 `rilsync.Send` 按 `(actor, id)` 查快照，天然支持每个 actor 独立 Diff——`AttributeTranslator` 遍历 actors 逐个 Send 即可，破窗消除。

**痛点 3：集合类 RIL 需要 3 个类（IRIL_DIFF / RILCross 必须删除）**

| 类 | 作用 | 本方案 |
|---|------|--------|
| `RIL_ACTOR` | 全量数据 | 保留，加 Diff/Merge |
| `RIL_DIFF_ACTOR` | 差量载体 | **删除** |
| `ActorCross` | 合并逻辑 | **删除**（合并逻辑移入 RIL.Merge） |

新增一个集合类型 = 3 个新文件。合并逻辑写错（比如 `HasDel` 漏了 `RmvAgent`），bug 在运行时才暴露。

**痛点 4：RILBucket.SetRIL 是整体替换**

Agent 端收到的永远是"整个 RIL 变了"，无法感知字段级变化。状态同步下即使只有一个字段变了也要序列化整个 RIL。

**痛点 5：Hash 函数的手写错误是静默的**

```csharp
// FacadeEffectTranslator.OnCalcHashCode — 现存 bug
foreach (var id in info.effects)
{
    hash = hash * 31 + info.actor.GetHashCode();  // ← bug：应是 id.GetHashCode()
}
```

hash 漏一个字段，该字段变化时 hash 不变，永远不会同步到 Render 层。没有编译错误，没有运行时异常，只是画面错了。Diff 直接字段比较，不会漏。

**痛点 6：无序列化能力，状态同步无从谈起**

当前 RIL 是纯 C# 对象，没有 Serialize/Deserialize 接口。

---

## 3. 方案

### 3.1 IRIL 扩展：统一 Diff/Merge/Serialize 接口

`IRIL` 现状是 abstract class，有 `actor`/`hashcode`/`id` 和 `OnReady/OnReset`。扩展为：

```csharp
/// <summary>
/// 渲染指令基类
/// </summary>
public abstract class IRIL
{
    public abstract ushort id { get; }
    public ulong actor { get; private set; }
    /// <summary>
    /// 字段变更掩码（Diff 产生，0 表示无变化）
    /// hashcode 字段删除，由 fieldmask 替代
    /// </summary>
    public ulong fieldmask { get; private set; }

    public void Ready(ulong actor, ulong fieldmask)
    {
        this.actor = actor;
        this.fieldmask = fieldmask;
        OnReady();
    }

    public void Reset()
    {
        OnReset();
        actor = 0;
        fieldmask = 0;
    }

    protected abstract void OnReady();
    protected abstract void OnReset();

    /// <summary>
    /// 与快照比较，产生 fieldmask。返回 0 表示无变化
    /// 值类：字段比较
    /// 集合类：集合比较（填充 added/removed 字段，返回对应 mask）
    /// 高频类：直接返回全 1（跳过比较）
    /// </summary>
    public abstract ulong Diff(IRIL snapshot);

    /// <summary>
    /// 将 fieldmask 标记的变化合并到自身
    /// </summary>
    public abstract void Merge(IRIL other, ulong fieldmask);

    /// <summary>
    /// 序列化 fieldmask 标记的字段（状态同步用，帧同步不调用）
    /// </summary>
    public virtual void Serialize(BinaryWriter writer, ulong fieldmask) { }

    /// <summary>
    /// 反序列化并合并（状态同步用）
    /// </summary>
    public virtual void Deserialize(BinaryReader reader, ulong fieldmask) { }

    /// <summary>
    /// 克隆（用于 RILSync 维护快照）
    /// </summary>
    public abstract IRIL Clone();
    // 集合类 Clone 必须深拷贝内部字典
    // snapshot 与工作 RIL 不能共享 dict（否则 Reset 清空会破坏快照）
}
```

**过渡说明**：`hashcode` 暂时保留（未迁移 RIL 走 hash 路径，行为不变），新增 `fieldmask`。RIL 子类逐步实现 `Diff`/`Merge`/`Clone`，实现后声明 `isdiffable` 走 Diff 路径。全部迁移后删 `hashcode` + hash 路径。基类默认 `Diff` 返回哨兵值 `DIFF_PENDING`（表示未实现，回退 hash 路径），不是返回全 1——否则未迁移的低频 RIL 会从"hash 相同不 Send"退步成"每帧 Send+Clone+Merge"。

### 3.2 值类 RIL：字段级 Diff/Merge

以 `RIL_SPATIAL` 为例（高频，Diff 跳过比较）：

```csharp
public partial class RIL_SPATIAL : IRIL
{
    public const ulong FM_POSITION = 1ul << 0;
    public const ulong FM_EULER = 1ul << 1;
    public const ulong FM_SCALE = 1ul << 2;

    public FPVector3 position;
    public FPVector3 euler;
    public FP scale;

    public override ushort id => RIL_DEFINE.SPATIAL;

    // 高频 RIL：必 dirty，跳过比较，仍走统一路径
    public override ulong Diff(IRIL snapshot) => FM_POSITION | FM_EULER | FM_SCALE;

    public override void Merge(IRIL other, ulong mask)
    {
        var ril = (RIL_SPATIAL)other;
        if (0 != (mask & FM_POSITION)) position = ril.position;
        if (0 != (mask & FM_EULER)) euler = ril.euler;
        if (0 != (mask & FM_SCALE)) scale = ril.scale;
    }

    public override IRIL Clone()
    {
        var r = RILCache.Ensure<RIL_SPATIAL>();
        r.position = position; r.euler = euler; r.scale = scale;
        return r;
    }

    protected override void OnReady() { }
    protected override void OnReset() { position = default; euler = default; scale = default; }
}
```

以 `RIL_STATE_MACHINE` 为例（低频，字段级 Diff）：

```csharp
public partial class RIL_STATE_MACHINE : IRIL
{
    public const ulong FM_STATE = 1ul << 0;
    public const ulong FM_SUBSTATE = 1ul << 1;
    public const ulong FM_TIME = 1ul << 2;

    public int state;
    public int substate;
    public FP time;

    public override ushort id => RIL_DEFINE.STATE_MACHINE;

    public override ulong Diff(IRIL snapshot)
    {
        var s = (RIL_STATE_MACHINE)snapshot;
        ulong mask = 0;
        if (false == state.Equals(s.state)) mask |= FM_STATE;
        if (false == substate.Equals(s.substate)) mask |= FM_SUBSTATE;
        if (false == time.Equals(s.time)) mask |= FM_TIME;
        return mask;
    }

    public override void Merge(IRIL other, ulong mask)
    {
        var ril = (RIL_STATE_MACHINE)other;
        if (0 != (mask & FM_STATE)) state = ril.state;
        if (0 != (mask & FM_SUBSTATE)) substate = ril.substate;
        if (0 != (mask & FM_TIME)) time = ril.time;
    }

    // ...
}
```

**手写成本**：新增字段 = 在 RIL 类加字段 + Diff 加一行 + Merge 加一行。比现状"加字段 + 改 hash + 改 OnRIL"少一处，且 hash 易错问题消除。

### 3.3 集合类 RIL：Diff 算 added/removed，Merge 增删（无 IRIL_DIFF/RILCross）

以 `RIL_FACADE_EFFECT` 为例。added/removed 是 RIL 自身字段，Diff 时填充，Merge 时按其增删，Agent 直接读取——**不需要 IRIL_DIFF 载体，不需要 RILCross 合并器**。

```csharp
public partial class RIL_FACADE_EFFECT : IRIL
{
    public const ulong FM_ADDED = 1ul << 0;
    public const ulong FM_REMOVED = 1ul << 1;

    public Dictionary<uint, EffectInfo> effectdict;

    // Diff 时填充，Merge/Agent 读取
    public List<uint> addedkeys { get; private set; }
    public List<uint> removedkeys { get; private set; }

    public override ushort id => RIL_DEFINE.FACADE_EFFECT;

    public RIL_FACADE_EFFECT()
    {
        addedkeys = ObjectPool.Ensure<List<uint>>();
        removedkeys = ObjectPool.Ensure<List<uint>>();
    }

    public override ulong Diff(IRIL snapshot)
    {
        var s = (RIL_FACADE_EFFECT)snapshot;
        addedkeys.Clear(); removedkeys.Clear();

        // 新增 + 修改
        foreach (var kv in effectdict)
        {
            if (false == s.effectdict.TryGetValue(kv.Key, out var old) || false == old.Equals(kv.Value))
                addedkeys.Add(kv.Key);
        }
        // 删除
        foreach (var k in s.effectdict.Keys)
            if (false == effectdict.ContainsKey(k)) removedkeys.Add(k);

        ulong mask = 0;
        if (0 != addedkeys.Count) mask |= FM_ADDED;
        if (0 != removedkeys.Count) mask |= FM_REMOVED;
        return mask;
    }

    public override void Merge(IRIL other, ulong mask)
    {
        var ril = (RIL_FACADE_EFFECT)other;
        // ril.addedkeys/removedkeys 已在 Diff 时填充
        if (0 != (mask & FM_REMOVED)) foreach (var k in ril.removedkeys) effectdict.Remove(k);
        if (0 != (mask & FM_ADDED)) foreach (var k in ril.addedkeys) effectdict[k] = ril.effectdict[k];
    }

    protected override void OnReset()
    {
        effectdict?.Clear();
        addedkeys.Clear(); removedkeys.Clear();
    }
}
```

`RIL_ACTOR` 同理（`actors` 是 `List<ulong>`，Diff 算 added/removed actors）。

**EffectAgent 不再自己 diff**：直接读 `ril.addedkeys`/`ril.removedkeys` 创建/回收特效。痛点 R4 闭环。

**EffectInfo 必须实现 `IEquatable<EffectInfo>`**：`Diff` 中 `old.Equals(kv.Value)` 判断 EffectInfo 是否变化。若 EffectInfo 无值相等实现（引用默认 `object.Equals` 仅比较引用），每次都会误判为 added——原因：`Translator` 每帧重新填充 RIL，当前 effectdict 中的 EffectInfo 是当帧构造的新对象，与 snapshot 中上帧的老对象引用不同，即使内容一样也会判定为"变化"。按现状 `FacadeEffectTranslator` 看，`EffectInfo` 是 `BehaviorInfo` 子类，需判断字段相等。若字段过多不适合 `IEquatable`，可用内容哈希/版本号替代。

**集合类序列化策略**：状态同步发整包 `effectdict`（Serialize/Deserialize 序列化整包），不论 fieldmask。接收端全量覆盖。增量（只发 added/removed keys）中带宽收益很小（effectdict 通常不大），但接收端合并逻辑复杂（需处理丢包/乱序），Phase 5 上生成器后评估是否切增量。目前整包简单可靠。

**删除清单**：
- `IRIL_DIFF` 基类
- `RIL_DIFF_ACTOR`、`RIL_DIFF_FACADE_EFFECT` 等所有 RIL_DIFF_* 子类
- `RILCross`、`ActorCross`、`FacadeEffectCross` 等所有合并器
- `RILBucket.crossdict`、`RILBucket.SetDiff`
- `RILSync.diffqueue`、`Stage.ondiff`、`Stage.Diff()` 调用点
- `Behavior` 中所有 `stage.Diff(...)` 调用（改为 Behavior 修改集合后由 Translator 下一帧全量填 RIL，Diff 自然算出 added/removed）

### 3.4 Sa 级 RIL：退回 per-actor，标准 Diff（无特殊基类）

`AttributeTranslator` 的"一个 Info → N 个 Actor 的 RIL"不再需要特殊基类。`rilsync.Send` 本就按 `(actor, id)` 查快照，每个 actor 独立 Diff：

```csharp
public class AttributeTranslator : Translator<AttributeBucketInfo, RIL_ATTRIBUTE>
{
    public override ushort id => RIL_DEFINE.ATTRIBUTE;

    // 不重写非泛型 OnRIL（不再破窗）
    // 改为：Translator<T,E> 基类支持"批量产出"模式
    protected override void OnRIL(AttributeBucketInfo bucket, RIL_ATTRIBUTE template)
    {
        foreach (var kv in bucket.attributes)
        {
            var actor = kv.Key;
            if (false == stage.cache.Valid(actor)) continue;

            var ril = RILCache.Ensure<RIL_ATTRIBUTE>();
            ril.Ready(actor, 0);
            ril.hp = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.HP);
            ril.maxhp = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.MAXHP);
            ril.movespeed = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.MOVESPEED);
            ril.attack = stage.attrb.GetAttributeValue(actor, ATTRIBUTE_DEFINE.ATTACK);
            // 标准路径：Send 内部按 (actor, ATTRIBUTE) 查快照 Diff
            stage.rilsync.Send(ril);
        }
    }
}
```

`Translator<T,E>` 基类的调整：当 Translator 需要批量产出时，`OnRIL(T info, E template)` 的 `template` 参数只是类型提示，Translator 内部自行 `Ensure` + `Send`。基类不再假设一对一（不再自动 `Ensure` 单个 RIL + `Send`）。

**破窗消除**：`AttributeTranslator` 不再重写非泛型 `OnRIL`，走标准泛型路径，内部循环 Send。每个 `Send` 都走标准 Diff。

### 3.5 RILSync：统一 Snapshot Diff 管线

```csharp
public partial class RILSync : Behavior
{
    /// <summary>
    /// 快照集合（按 (actor, id) 维护）
    /// </summary>
    private Dictionary<(ulong, ushort), IRIL> snapshotdict { get; set; }

    /// <summary>
    /// 传输层
    /// </summary>
    private IRILTransport transport { get; set; }

    /// <summary>
    /// 发送 RIL（统一入口，所有 RIL 走此路径）
    /// </summary>
    public void Send(IRIL ril)
    {
        var key = (ril.actor, ril.id);
        if (false == snapshotdict.TryGetValue(key, out var snapshot))
        {
            // 首次：全量发送
            ril.fieldmask = 0xFFFFFFFFFFFFFFFF;
            transport.Send(ril);
            snapshotdict[key] = ril.Clone();
            return;
        }

        var mask = ril.Diff(snapshot);
        if (0 == mask)
        {
            // 无变化，回收
            ril.Reset();
            RILCache.Set(ril);
            return;
        }

        ril.fieldmask = mask;
        transport.Send(ril);
        // 更新快照（用 ril 的最新内容覆盖 snapshot）
        ril.CloneInto(snapshot);
    }
}
```

`IRIL.CloneInto(IRIL target)` 把自身内容拷贝到 target（避免反复分配）。

**`ishighfrequency` 跳过 snapshot 存储**：高频 RIL（SPATIAL）Diff 返回全 1（不比较），snapshot 仅用于 `CloneInto` 保持最新——但 CloneInto 每帧一次拷贝（FPVector3×3）无实际用途。声明 `ishighfrequency=true` 后 `RILSync.Send` 跳过 `snapshotdict` 存取：不 Clone、不 CloneInto、不 Diff 比较，fieldmask 直接 = 全 1 后 Send。首次也跳过（高频不存在"首次全量"概念，每帧都是全量）。真零开销路径。

**删除**：`hashcodedict`、`CacheHashCode`、`Query`、Translator 的 `OnCalcHashCode`。

### 3.6 传输层抽象：IRILTransport

```csharp
public interface IRILTransport
{
    void Send(IRIL ril);
    void Send(IRIL_EVENT e);
}

// 帧同步：本地直接调用
public class LocalTransport : IRILTransport
{
    private Stage stage { get; set; }
    public void Send(IRIL ril) { stage.onril?.Invoke(ril); }
    public void Send(IRIL_EVENT e) { stage.onevent?.Invoke(e); }
}

// 状态同步：序列化 + 网络发送（Phase 4 实现）
public class NetworkTransport : IRILTransport
{
    public void Send(IRIL ril)
    {
        if (0 == ril.fieldmask) return;  // 无变化不发送
        using var ms = ObjectPool.Ensure<MemoryStream>();
        var writer = new BinaryWriter(ms);
        writer.Write(ril.id);
        writer.Write(ril.actor);
        writer.Write(ril.fieldmask);
        ril.Serialize(writer, ril.fieldmask);
        // 网络发送 ms.GetBuffer()
    }
}
```

**帧同步零开销**：`LocalTransport` 不碰 `Serialize`，生成的 `Serialize` 代码不被调用（程序集体积存在，构建期成本可接受）。

### 3.7 RILBucket：Merge 替代整体替换

```csharp
public void SetRIL(IRIL ril)
{
    if (false == rildict.TryGetValue(ril.actor, out var dict))
    {
        rildict.Add(ril.actor, dict = ObjectPool.Ensure<Dictionary<Type, IRIL>>());
        dict.Add(ril.GetType(), ril);
        RILDispatch(ril);
        return;
    }

    if (dict.TryGetValue(ril.GetType(), out var oldril))
    {
        // Merge：按 fieldmask 合并到已存 RIL
        oldril.Merge(ril, ril.fieldmask);
        ril.Reset(); RILCache.Set(ril);
        // 分发合并后的 RIL（带 fieldmask，Agent 可做字段级响应）
        RILDispatch(oldril);
        return;
    }

    dict.Add(ril.GetType(), ril);
    RILDispatch(ril);
}
```

**删除**：`SetDiff`、`crossdict`、`RILCross` 初始化（`Cross()` 方法）。

### 3.8 状态同步插值缓冲

状态同步下客户端收到快照有延迟，需要插值。`RILBucket` 支持"按时间戳的快照 ring buffer"：

```csharp
public class RILBucket
{
    // 帧同步：rildict（单份最新状态）
    // 状态同步：rildict + historydict（ring buffer，容量 N）
    private Dictionary<(ulong, ushort), RingBuffer<IRIL>> historydict;

    public IRIL SeekRILAtTime(ulong actor, ushort id, long targettime)
    {
        if (false == historydict.TryGetValue((actor, id), out var ring)) return default;
        return ring.Interpolate(targettime);
    }
}
```

帧同步不用（零延迟）。状态同步启用 `historydict`，传输层模式切换时配置。

### 3.9 序列化：手写但用 helper（Phase 1 不上 Source Generator）

```csharp
public static class RILWriter
{
    public static void WriteFP(BinaryWriter w, FP v) => w.Write(v.Raw);
    public static void WriteFPVector3(BinaryWriter w, FPVector3 v)
    {
        WriteFP(w, v.x); WriteFP(w, v.y); WriteFP(w, v.z);
    }
    public static void WriteList<T>(BinaryWriter w, List<T> list, Action<BinaryWriter, T> writeitem)
    {
        w.Write(list.Count);
        foreach (var item in list) writeitem(w, item);
    }
}

public partial class RIL_SPATIAL
{
    public override void Serialize(BinaryWriter writer, ulong fieldmask)
    {
        if (0 != (fieldmask & FM_POSITION)) RILWriter.WriteFPVector3(writer, position);
        if (0 != (fieldmask & FM_EULER)) RILWriter.WriteFPVector3(writer, euler);
        if (0 != (fieldmask & FM_SCALE)) RILWriter.WriteFP(writer, scale);
    }
}
```

`FP`/`FPVector3` 底层格式封装在 helper 里。Phase 5 上 Source Generator 时，生成器替换 helper 调用，RIL 定义不变。

### 3.10 IRIL_EVENT：加 frame 字段（回滚幂等支持）

`IRIL_EVENT` 现状无 frame。渲染层回滚重模拟会产生确定性事件，`RILSalute` 是 fire-and-forget（音效/伤害飘字），重模拟会重复触发。渲染层用 `frame` 去重（见渲染层 3.7），因此 `IRIL_EVENT` 需加 `frame` 字段：

```csharp
public abstract class IRIL_EVENT
{
    public abstract ushort id { get; }
    public ulong actor { get; private set; }
    /// <summary>
    /// 逻辑帧号（渲染层 Salute 回滚幂等去重用）
    /// </summary>
    public long frame { get; private set; }

    public void Ready(ulong actor, long frame) { this.actor = actor; this.frame = frame; OnReady(); }
    public void Reset() { OnReset(); actor = 0; frame = 0; }

    protected abstract void OnReady();
    protected abstract void OnReset();
    protected abstract void OnClone(IRIL_EVENT clone);

    /// <summary>
    /// 序列化（状态同步用，事件无 Diff）
    /// </summary>
    public virtual void Serialize(BinaryWriter writer) { }
}
```

**事件不 Diff**：fire-and-forget 语义保留，`frame` 仅用于渲染层 `RILSalute` 去重。`IRILTransport.Send(IRIL_EVENT)` 直接送，不走 `rilsync.Send` 的 Diff 路径。状态同步下事件走 `Serialize`，接收端按 frame 去重。迁移时 Phase 1 一并实现。

---

## 4. 与渲染层重构的关系

两个重构**不**正交：

| 渲染层痛点 | 根因 | 本方案解决？ |
|-----------|------|-------------|
| R1: Agent 无法感知字段变化 | RIL 整体替换 | **是** — fieldmask 告诉 Agent 哪些字段变了 |
| R2: Agent 交叉依赖隐式 | 无依赖图 | 否（渲染层自解） |
| R3: Agent 回读 RILBucket | Agent 不信任推送 | 部分 — Merge 后存储的是权威数据 |
| R4: EffectAgent 内部 diff | RIL 整体替换 | **是** — Merge 产出 addedkeys/removedkeys，Agent 直接读 |
| R5: RILBucket 职责过多 | 历史演进 | 是 — 删 SetDiff/crossdict |
| R6: Dispatch 链太长 | 多层间接 | 否（渲染层自解） |
| R7: Agent 生命周期散落 | 无统一管理 | 否（渲染层自解） |

**推荐顺序**：

```
Phase 1-3: RIL 重构（本方案 3.1-3.7）
  - IRIL 扩展 Diff/Merge/Serialize/Clone
  - 值类迁移（字段级 Diff）
  - 集合类迁移（删除 IRIL_DIFF/RILCross，Diff 算 added/removed）
  - Sa 级退回 per-actor（删除 MultiTranslator 破窗）
  - IRILTransport 抽象
  ↓ 产出：fieldmask 可用，Merge 可用，added/removed 可用

Phase 4: 渲染层重构
  - 两阶段管线
  - Agent 拉 fieldmask 做字段级响应（R1 闭环）
  - EffectAgent 读 addedkeys/removedkeys（R4 闭环）
  ↓ 产出：渲染层结构清晰，字段级响应可用

Phase 5（可选）: Source Generator
  - 生成 Diff/Merge/Serialize，消除手写
  - 视 Phase 1-3 手写量是否值得自动化决定
```

**若先做渲染层**：R1/R4 只能"改善"不能"解决"，做完 RIL 重构后还得回头改 Agent——迁移成本双算。不推荐。

---

## 5. 迁移计划

### Phase 1：IRIL 扩展 + 基础设施（过渡期双路径）

1. `IRIL` 加 `fieldmask`、`Diff`、`Merge`、`Serialize`/`Deserialize`、`Clone`/`CloneInto` 虚方法。**保留 `hashcode`**（未迁移 RIL 兼容）。基类 `Diff` 默认返回哨兵 `DIFF_PENDING`（表示未实现，回退 hash 路径），`isdiffable` 默认 false
2. `IRILTransport` 接口 + `LocalTransport` 实现（行为等同现状 `stage.onril` 直调）
3. `RILSync.Send` 双路径：`isdiffable` 走 Diff（`snapshotdict` 快照比较，产 fieldmask），否则走 hash（`hashcodedict` 比较，兼容现状）。两条路径并存
4. `RILBucket.SetRIL`：Diff 路径走 Merge，hash 路径走整体替换（兼容现状）

**验收**：编译通过。无 RIL 迁移（全走 hash）时整体行为零变化。**不能靠"默认 Diff 返回全 1"假装零变化**——那会让低频 RIL 从"hash 相同不 Send"退步成"每帧 Send+Clone+Merge"。

### Phase 2：值类 RIL 迁移

1. `RIL_SPATIAL` → `Diff` 返回全 1（高频），`Merge` 整体替换
2. `RIL_STATE_MACHINE` → 字段级 `Diff`/`Merge`
3. `RIL_FACADE_MODEL` → 同上
4. `RIL_TICKER`/`RIL_SEAT`/`RIL_STAGE` → 同上
5. 每迁移一个，删除对应 Translator 的 `OnCalcHashCode`
6. `AttributeTranslator` 退回 per-actor，走标准 `Send`（删除非泛型 `OnRIL` 重写）

**验收**：低频 RIL 不再每帧产 RIL（无变化时回收）。

### Phase 3：集合类 RIL 迁移 + 删除 IRIL_DIFF/RILCross

1. `RIL_FACADE_EFFECT` → `Diff` 算 added/removed，`Merge` 增删
2. `RIL_ACTOR` → 同上
3. 删除 `IRIL_DIFF`、`RIL_DIFF_ACTOR`、`RIL_DIFF_FACADE_EFFECT`
4. 删除 `RILCross`、`ActorCross`、`FacadeEffectCross`
5. 删除 `RILBucket.crossdict`/`SetDiff`/`Cross()` 初始化
6. 删除 `RILSync.diffqueue`
7. 删除 `Stage.ondiff`、`Stage.Diff()`、所有 Behavior 中的 `stage.Diff(...)` 调用（Behavior 修改集合后由 Translator 全量填 RIL，Diff 自然算 added/removed）

**`FacadeEffectTranslator` 每帧 Clone 开销**：现状 `FacadeEffectTranslator.once=true`（只处理一次，之后不填 RIL）。Snapshot Diff 后删 `once`，每帧填充 `effectdict` 入 RIL——但 `effectdict` 数据来自 Logic 线程的 BehaviorInfo，跨线程不能共享字典引用，必须每帧 Clone 整个 dict 到 RIL 对象。这是新增开销，承认。`RIL_ACTOR` 的 `actors`（`List<ulong>`）同理。

**验收**：集合类 RIL 走统一路径，无 IRIL_DIFF/RILCross 残留。

### Phase 4：状态同步传输层

1. `NetworkTransport` 实现（`Serialize` + 网络发送）
2. `RILBucket` 加 `historydict` + `SeekRILAtTime`
3. 客户端插值逻辑

### Phase 5（可选）：Source Generator

1. 定义 `[RIL]` Attribute
2. 生成 `Diff`/`Merge`/`Serialize`/`Clone`，替换手写
3. **前置条件**：Phase 2-3 手写模式稳定，确认生成器能覆盖所有字段类型（FP/FPVector3/Dictionary/List/嵌套结构）

---

## 6. 开放问题（收敛）

| 问题 | 处置 |
|------|------|
| 集合类 Source Generator 策略 | Phase 5 处理，Phase 1-3 手写（3.3） |
| FPVector3 序列化格式 | `RILWriter` helper 封装，确认 FP 底层 long（3.9） |
| IRIL_EVENT | 加 `frame` 字段（3.10）支持渲染层 Salute 回滚去重；事件不 Diff，fire-and-forget 语义保留 |
| 帧同步确定性 | RIL 不影响 Logic 确定性，Behavior 不读 RIL 做逻辑判断（保持现状） |
| 状态同步插值缓冲 | `historydict` ring buffer 明确设计（3.8） |
| Agent 依赖图 / Dispatch 精简 / Agent 生命周期 / fieldMask 传递 | 渲染层重构解决 |

**无阻断性开放问题**。Sa 级和集合类两个原"硬骨头"已在本方案给出明确路径（退回 per-actor / Diff 算 added-removed），不留待生成器。
