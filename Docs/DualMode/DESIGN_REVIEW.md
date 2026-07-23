# 双模同步 + RIL/Render 重构 — 设计评审

> 基于对三份设计文档和全部实际代码的交叉审查。以资深动作游戏开发工程师视角，在代码推进前做一次系统性的"找茬"。

---

## 0. 总体评价

**核心方向正确**：RIL 层统一 Diff/Merge、渲染层 Agent 自主生命周期、双模可插拔传输是值得推进的方向。当前 3 条 Diff 路径（hash + IRIL_DIFF + RILCross）确实过度工程化，渲染侧 EffectAgent 自己 diff effectdict 也确实逻辑外泄。

**但有 9 个问题需要在设计阶段解决，否则实现到一半会被卡住。** 其中 3 个是阻塞性问题。

---

## 1. 阻塞性问题（不改不能推进）

### 1.1 FacadeEffectTranslator `once=true` 移除后，哈希漂移导致每帧全量发送

**现状**：`FacadeEffectTranslator.OnCalcHashCode` 将 `eff.elapsed` 纳入哈希（第 29 行），而 `elapsed` 每帧都会变化（哪怕是静态特效，elapsed 也在动画推进）。依赖 `once=true` 阻挡后续检查，所以哈希比较从未被触发。

**设计意图**：移除 `once=true`，改为每帧 Diff 检测，以支持状态同步。

**实际后果**：移除 `once=true` 后，`Translator<T,E>::OnRIL(BehaviorInfo)` 的 `once` 分支不再短路，进入 `CacheHashCode`。由于 `elapsed` 变化，哈希每帧不同 → 每帧创建 RIL → 每帧 Send → 每帧 RILBucket 收到 → 每帧 EffectAgent 处理。

对 100 个有特效的 actor，每帧新建 100 个 `RIL_FACADE_EFFECT` 对象、100 次 dict 克隆、100 次 Diff、100 次 Merge。EffectAgent 每帧做 add/remove 遍历。

**而实际上**，特效是**不应该每帧同步**的东西：一次添加、逐帧自行推进、逻辑层不会中途改 elapsed/position/euler。当前 `once=true` 才是正确语义。

**修正方案**：

| 方案 | 描述 | 推荐度 |
|------|------|--------|
| A | 保留 `once=true`，特效变更用 `RIL_EVENT`（EffectAdd/EffectRemove）通知 | ★★★ 推荐 |
| B | 移除 `once=true` 但把 `elapsed` 从哈希中拿掉，让只有特效集合结构变化时才触发 | ★★☆ 次选 |
| C | 保持设计原样但接受每帧开销 | ☆☆☆ 不推荐 |

**方案 A 细节**：

```csharp
// RIL_EVENT_EFFECT_ADD / RIL_EVENT_EFFECT_REMOVE
// 当 BehaviorInfo 的 effectdict 结构变化时 emit event
// EffectAgent 监听 event 做 add/remove

// once=true 的 RIL 保留，作为 "首次加载时发送全量特效快照"
// 状态同步时，直接从 RILBucket 取存储的 RIL_FACADE_EFFECT 序列化即可
```

这样，帧同步路径零额外开销（once 保持不变），状态同步路径只需要在需要序列化整个 world 时从 RILBucket 抓取当前存储的 RIL。

**设计文档里没有讨论这个问题，但它是 `once=true` 移除决策中最关键的考量。**

---

### 1.2 EffectAgent 缺失 "updated key" 处理路径

**现状**：`EffectAgent.OnRILFacadeEffect` 处理了 add（新 key）和 remove（缺失 key）。对于同一个 key 但 EffectInfo 值已变化的情况（如 Logic 层改了特效的 position/euler/scale），当前代码段 86-91 做了**替换**：

```csharp
// EffectAgent.cs:84-93 — 当前代码
foreach (var kv in ril.effectdict)
{
    if (effects.TryGetValue(kv.Key, out var effect))
    {
        effects.Remove(kv.Key);
        effects.Add(kv.Key, (kv.Value, effect.controller)); // 替换 Info，保留 Controller
        continue;
    }
    CreateEffect(kv.Value);
}
```

**设计意图**：用 `addedkeys` / `removedkeys` 替代上述比较。

**实际后果**：设计的 `EffectAgent.OnExpress` 只有创建和销毁两路，漏掉了上述"已存在的 key 值被替换"的第三路。加上 1.1 每帧发送问题被修复后频率很低，因此此问题**被 1.1 的修正方案所掩盖**（方案 A 下不会频繁触发更新路径）。

但如果走方案 B（移除 once 但去 elapsed），当 position/euler 变化时仍需更新路径。而设计文档中根本没有 `FM_UPDATED` 或等效的第三个掩码位。

**修正方案**：新增 `FM_CHANGED`，在 Diff 中为 `effectdict` 的每个 key 检测内容变化（`Equals`），变化的 key 放入 `changedkeys`，Agent 收到后以新 EffectInfo 替换旧 Info 但保留 Controller。

如果走推荐方案 A（once=true + events），此问题自动化解为不需要处理更新路径。

---

### 1.3 AttributeTranslator 批量每 actor 发送的 Translator 基类改造未被设计

**现状**：`AttributeTranslator` 完全覆写了 `Translator<T,E>::OnRIL(BehaviorInfo)`（非泛型版本），手动处理 hash 检查、RIL 创建、Send 所有流程。`OnRIL(T, E)` 为**空实现**。这是一种"破窗"：

```csharp
// AttributeTranslator.cs — 当前结构
protected override void OnRIL(BehaviorInfo info)  // 覆写非泛型 OnRIL
{
    // 手动处理一切：hash、RIL 创建、Send…
}
protected override void OnRIL(AttributeBucketInfo info, RIL_ATTRIBUTE ril)
{
    // 空实现 — 基类不会调用到这儿
}
```

**设计意图**：改为每个 actor 一个 `RIL_ATTRIBUTE`，Translator 自行迭代 actor 创建并 Send 多条 RIL。

**实际后果**：`Translator<T,E>` 基类假设 `1 Info → 1 RIL`。如果 `OnRIL(T, E)` 内部已调用 `stage.rilsync.Send()`，基类在 `OnRIL` 返回后还会再次 Send 模板 RIL（`ril` 参数），导致**重复发送**或**发送空 RIL**。设计文档只说"基类不再假设一对一"但没有给出具体机制。

**修正方案**：在 Translator 上增加显式标记，表示此 Translator 自行管理批量 Send：

```csharp
public abstract class Translator<T, E> : Translator where T : BehaviorInfo where E : IRIL, new()
{
    /// <summary>
    /// 当 true 时，基类不自动 Ensure/Send，由子类自行管理
    /// </summary>
    protected virtual bool manualsend => false;  // 新增

    protected override void OnRIL(BehaviorInfo info)
    {
        if (once && rileds.Contains(info.actor)) return;
        
        if (manualsend)
        {
            OnRIL(info as T, default);  // 传 null/default 作类型提示
            return;
        }
        
        var result = CacheHashCode(info);
        if (false == result.diffed) return;
        if (once) rileds.Add(info.actor);
        
        var ril = RILCache.Ensure<E>();
        ril.Ready(info.actor, result.hashcode);
        OnRIL(info as T, ril);
        stage.rilsync.Send(ril);
    }
}
```

`AttributeTranslator` 设置 `manualsend => true`，在 `OnRIL` 中自行 Ensure + Send 每个 actor 的 RIL。基类的 CacheHashCode/once 仍可用（子类在 batch OnRIL 开头显式调用 `CacheHashCode`）。

**这个改动量虽小但极关键——没有它，Phase 3 无法开始。**

---

## 2. 中等问题（实现前需明确）

### 2.1 RILSync snapshotdict 缺失 Actor 生命周期清理

RILSync 维护 `snapshotdict: Dictionary<(ulong actor, ushort id), IRIL>`。当一个 actor 被永久移除（死亡、销毁），其 snapshot 仍在字典中：
- 对于值类型 RIL（SPATIAL）：仅 3 个 FPVector3，泄漏量小
- 对于集合类型 RIL（FACADE_EFFECT）：持有 `Dictionary<uint, EffectInfo>` 引用，**随着游戏时间增长持续泄漏**

当前代码不存在此问题（hashcodedict 仅存 `int`）。新设计引入 Clone 快照后必须配套清理。

**修正方案**：RILSync 增加 `RmvActor(ulong actor)` 方法，在 `Stage.OnActorDestroy` 中调用：

```csharp
public void RmvActor(ulong actor)
{
    foreach (var id in snapshotdict.Keys.Where(k => k.actor == actor))
    {
        var snap = snapshotdict[id];
        snap.Reset();
        RILCache.Set(snap);
        snapshotdict.Remove(id);
    }
}
```

### 2.2 RILBucket 中 oldril 的首次创建与生命周期

新设计下 RILBucket 存储 `oldril`（持久化副本），每次新 RIL 到达时 `oldril.Merge(ril, mask)`。但**首次 RIL 到达时**没有 oldril，需 Clone 一份：

```csharp
// RILBucket.SetRIL — 首次到达
if (false == typedict.TryGetValue(type, out var oldril))
{
    typedict[type] = RILCache.Ensure(type); // 从池取
    ril.CloneInto(typedict[type]);           // 拷贝数据
    // 需要触发 dispatch 让 Agent 收到（首次也是"新增"）
}
```

三个细节需要明确：
1. **首次也需要 dispatch**：Agent 第一次收到 RIL 需要创建响应（SpatialAgent 创建 Node3D、EffectAgent 创建 effects 等）。这些首次处理与"更新"不同，dispatch 方式应保持一致。
2. **CloneInto 必须深拷贝集合**：对于 `RIL_FACADE_EFFECT`，CloneInto 不能只赋值引用，必须复制 `Dictionary<uint, EffectInfo>` 的每个条目（否则 oldril 和传入 ril 共享同一 dict，Merge 会破坏快照）。
3. **Actor 移除时需清理**：Actor 消亡后，rildict 中对应的 typedict 条目需清理，防止残留 RIL 被误读。

### 2.3 集合类型的序列化策略不一致

设计文档在 `RIL_UNIFIED_DIFF_DESIGN.md` 中说：

> 集合类序列化策略：状态同步发整包 `effectdict`

同时，Diff 产生的 `addedkeys` / `removedkeys` 会被 `Transport.Send` 发送。但在状态同步模式下：
- Sender：Diff 产生 addedkeys/removedkeys + 整包 effectdict → 一起序列化
- Receiver：收到整包 effectdict + addedkeys/removedkeys

这里 addedkeys/removedkeys 是**冗余**的——Receiver 拿到全量 effectdict 后可以自行 diff 本地 oldril。发送方计算的 addedkeys/removedkeys 对 Receiver 无用。

**修正方案**：两种传输模式下 RIL 序列化格式不同：

| 字段 | 帧同步（LocalTransport） | 状态同步（NetworkTransport） |
|------|--------------------------|------------------------------|
| fieldmask | 发送 | 不发送（Receiver 自行 Diff） |
| addedkeys | 发送 | 不发送 |
| removedkeys | 发送 | 不发送 |
| effectdict | 不发送（oldril 已有） | 整包发送 |

状态同步下 Receiver 收到整包后自行 `Diff(oldril)` 得到 fieldmask/addedkeys/removedkeys，然后走 `Merge → Dispatch`。

### 2.4 EffectInfo IEquatable 要求与手写哈希是同一类 Bug

设计将手写哈希（`hash*31+value`）列为必须消除的 Bug 来源。但新方案要求 `EffectInfo` 实现 `IEquatable<EffectInfo>`——开发者仍然需要逐字段手写比较逻辑，**遗漏一个字段导致静默 Bug 的根因并未消除**。

**修正方案**：

| 方案 | 描述 |
|------|------|
| A | 要求 EffectInfo 必须是**值类型 struct**，struct 的 `Equals` 默认按字段逐一比较，无需手写任何代码 |
| B | 在 `IRIL` 基类上提供 `ContentEquals` 虚方法，由 RIL 类型自行实现（与现有 IRIL_DIFF Clone 模式类似） |
| C | 在 Diff 中使用哈希比较而非 Equals：用 `OnCalcHashCode` 的输出当"指纹"，两个做全等比较 |

**方案 A 最优**——如果 EffectInfo 本身没有引用其他对象的需求，改为 struct 一劳永逸。但 BehaviorInfo 的生命周期方法（OnReady/OnReset/OnClone）假设了引用类型语义，需要确认 EffectInfo 是否需要这些方法。

### 2.5 SpatialBatch 并行读取 RILBucket 的线程安全性

当前 `SpatialBatch` 使用 `Parallel.ForEach` 处理 RIL_SPATIAL 条目。设计保留此并行路径，每次迭代改为调用 `agent.Express(dt)`，其中 `agent.Express` 内部调用 `SeekRIL<RIL_SPATIAL>()` → 读取 `RILBucket.rildict`。

多个线程并发读取 `Dictionary<Type, IRIL>` 在 .NET 中**不保证安全**。虽然实践上纯读通常不出错，但 .NET 官方文档明确声明 Dictionary 不是线程安全的，并发读会触发内部版本号检查（枚举时）。

**修正方案**：

- **短期**：SpatialBatch 在 `Parallel.ForEach` 之前将所有必要的 RIL 数据拷贝到数组，避免线程内读取 rildict
- **长期**：将 `RILBucket.rildict` 改为 `ConcurrentDictionary`（需要评估性能）

或直接在设计中注明："Express 期间 RILBucket 不会写入（World.OnTick 中 ReconcileAgent 先写入、Express 后只读）"。

设计文档没有提到这个问题。

---

## 3. 轻微问题

### 3.1 "3 层 pipeline" 实际是 4 个 Phase

`RENDER_LAYER_DESIGN.md` 第五章标题为"3 层 Express Pipeline"，但正文列了 Phase A/B/C/D 四个。Phase D（Sound/Event/Effect）是兜底层，不算"层"说得通但容易混淆。

**修正**：改为"分阶段 Express 管线"或"4 Stage Express Pipeline"。

### 3.2 回滚事件 frame 去重窗口 300 帧偏大

300 帧 = 5 秒。如果两个事件在 5 秒内重复发出（如 DoT 伤害第 1 帧发一次、第 100 帧又发一次），`CleanupBefore(rollbackframe - 300)` 会正确保留两个事件。但如果回滚到 50 帧前，第 1 帧的事件也还在窗口中——这是**正确行为**（回滚后需要重新触发）。

但 300 帧意味着 `processed` 集合最多保留 5 秒内的所有事件帧号。对高频率事件（如每帧挥刀检测），5 秒 × 60fps = 300 条目。看起来不大。

**无需修改**。但写入文档时应注明设计意图（"防止回滚重新触发 + 防止短时间内真实重复事件被误判"）。

### 3.3 FM_ prefix 命名不统一

设计文档中 fieldmask 常量使用了多种命名：
- `FM_POSITION`, `FM_EULER`（SPATIAL）
- `FM_STATE`, `FM_SUB_STATE`（STATE_MACHINE）
- `FM_ADDED`, `FM_REMOVED`（FACADE_EFFECT）

这些在各自 RIL 类内部命名空间独立，不冲突。**但 FM_CHANGED 和 FM_UPDATED 语义重叠**：新增修改掩码时建议统一用 `FM_UPDATED`（与 add/remove 并列）。

---

## 4. 架构层面的更优方案建议

### 4.1 `once=true` 不应被移除，而是正式化为第一公民

三份文档的隐含假设是"所有 RIL 都应每帧 Diff"以支持双模同步。但实际使用中：
- 有每帧变化的 RIL（SPATIAL、ANIMATION）
- 有仅首次发送的 RIL（FACADE_EFFECT、FACADE_MODEL）
- 有按变更频率触发的 RIL（STATE_MACHINE）

`once=true` 精准表达了"此 RIL 只在 actor 生命期初发送一次"的语义。建议**保留**这个机制，在其之上构建双模同步：

```csharp
// RILSync 的状态同步接口
public IEnumerable<IRIL> SnapshotAllRILs()
{
    // 状态同步时：从 RILBucket 获取当前所有 actor 的持久 RIL 数据
    // 这些 RIL 在帧同步路径可能是一次性发送的，但 RILBucket 保留了它们
    return rilbucket.GetActors().SelectMany(a => rilbucket.GetRILs(a));
}
```

这样：
- **帧同步路径**：once=true 的 RIL 只发一次，零常态开销。需要变更时用 RIL_EVENT。
- **状态同步路径**：RILBucket 中存储的就是世界状态。序列化时直接从 Bucket 读，不需要重启 Translator。

`FacadeEffectTranslator` 保持 `once=true`，不需要改为每帧 Diff。

### 4.2 集合 RIL 的 Diff 不需要每帧做

对于 `once=true` 的集合 RIL（如 FACADE_EFFECT），Diff 只需要做**一次**（首次发送时），所以 `addedkeys/removedkeys` 模式的价值大幅缩水——首次发送必然是整包，不存在增量。

而 `RIL_EVENT`（EffectAdd/EffectRemove）的粒度天然适合"集合变更通知"，不需要 Diff 机制参与。

因此建议：**集合类 RIL 不走 Diff 路径**，改用以下模式：
- 首次：Translator 物化为 RIL（整包），RILBucket 存储
- 变更：Logic 发 RIL_EVENT，RILBucket 的 Salute 处理 add/remove
- 序列化：直接从 RILBucket 读

这与当前架构差异最小、侵入最低。

### 4.3 渲染 Express 管线不需要严格的 4 Phase 顺序

设计文档的 Phase A→B→C→D 管线假设了严格的依赖顺序（Spatial→Model→Animation→Sound/Event/Effect）。但 `IBindDependencies` 的存在暗示 Agent 之间的依赖应在运行时建立：

```csharp
// ModelAgent.BindDependencies
spatial = world.GetAgent<SpatialAgent>(actor);
```

如果在 Express 之前统一 `BindDependencies()`，那么 Express 的执行顺序就不那么关键了——Agent 已持有依赖引用，可以并行 Express。

这样可以把 4 个 Phase 简化为 2 层：
1. **Reconcile**（增删 Agent）→ **BindDependencies**（建立引用）
2. **Express**（并行或顺序执行所有 Agent 的 OnExpress）

SpatialBatch 的并行前缀得到保留，后续 Phase 也可以根据需要并行化。

---

## 5. 修订后的迁移路线

考虑上述问题后，建议调整为：

| Phase | 内容 | 天数 | 关键调整 |
|-------|------|------|----------|
| 1 | IRIL 添加 Diff/Merge/Clone/CloneInto，新增 IRILTransport | 5 | 无变动 |
| 2 | RILSync.Send 切到 Diff → Send 路径，保留双路径 | 3 | 无变动 |
| 3 | 值类 RIL（SPATIAL/STATE_MACHINE/ANIMATION）迁移到 Diff | 3 | 无变动 |
| 4 | **Translator 基类加 `manualsend`，AttributeTranslator 改 per-actor** | 4 | **新增 Phase** |
| 5 | 渲染层 Agent 基础改造（Express/OnShouldExist/BindDependencies） | 4 | 移除 4 Phase 硬编码 |
| 6 | RIL_EVENT 路径统一（IRIL_SALUTE frame 去重） | 3 | 精简 |
| 7 | **一次性 RIL 保留 once=true，通过 RILBucket snapshot 支持状态同步** | 3 | **新增 Phase** |
| 8 | 移除旧路径（RILCross/IRIL_DIFF/hashcode 字段） | 2 | 无变动 |
| 9 | 集成测试 + 性能验证 | 3 | 无变动 |

总计从 28 天升至 **~30 天**（新增 2 天为 Phase 4 和 Phase 7 的核心代码）。

---

## 6. 总结

**设计质量**：整体方向正确，RIL 统一 Diff 和渲染层 Agent 独立生命周期的核心思路值得推进。

**主要风险**：
1. `once=true` 移除被低估——这不是工程决策而是玩法设计决策，且哈希漂移会把它变成每帧开销炸弹。
2. Translator 基类 batch 模式改造只有概念没有 API 设计——实现时必卡。
3. snapshotdict 泄漏没有清理机制——上线后几个月会发现内存在涨。

**关键建议**：
- **保留 `once=true`**，用它区分"一次性信息（特效/模型）"和"高频信息（空间/动画）"
- **集合 RIL 不走 Diff**，走 Event 变更 + RILBucket 持久化模式
- **先写 Translator 基类的 `manualsend` API 设计**，再开始 Phase 4

以上问题如果在设计阶段修正，实现阶段可以顺畅推进。如果不修正，实现到 30-50% 进度时会触雷返工。
