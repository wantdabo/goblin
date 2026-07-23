# 渲染层重构设计（迭代版）

## 1. 设计原则

> **渲染层的唯一职责：把 RIL 正确地表达出来。**

- RIL 是唯一数据源，Agent 从 RILBucket 拉取自己关心的数据
- 通知只是触发——真正的数据读取由 Agent 自主决定
- 数据到达和 Agent 表达是两个阶段，先全部到达，再统一表达
- 渲染层不关心数据从哪来——帧同步本地推送也好，状态同步网络接收也好，收到什么表达什么

---

## 2. 现状问题

### 2.1 时序混乱：RIL 逐条到达，逐条分发

```
当前 LocalDirector.OnTick:
  while(rilqueue.TryDequeue(out var ril))
      world.rilbucket.SetRIL(ril);    // 存 + 立即分发

  SetRIL 内部:
      rildict[type] = ril;            // 存
      RILDispatch(ril);               // 立即通知所有 Agent
```

一帧可能到达同一 Actor 的 SPATIAL + FACADE_MODEL + FACADE_ANIMATION 三条 RIL。每条都立即触发 Agent 回调。问题：

- SPATIAL 到了 → SpatialAgent 重启插值（但 ModelAgent 还没创建）
- FACADE_MODEL 到了 → ModelEnchant 创建 ModelAgent
- FACADE_ANIMATION 到了 → AnimationEnchant 读 RILBucket 的 FACADE_MODEL 判断路由

**靠的是 RIL 到达的碰巧顺序，不是显式的时序保证。** 如果顺序反过来，AnimationEnchant 读不到 FACADE_MODEL，路由就失败。

### 2.2 Agent 生命周期散落在 Enchant 中

```
SpatialEnchant:     RIL_SPATIAL 到了 → EnsureAgent<SpatialAgent>
ModelEnchant:       判断 primitive/glb → EnsureAgent<ModelAgent | PrimitiveMeshAgent>
AnimationEnchant:   读 RILBucket 的 FACADE_MODEL → 判断路由 → EnsureAgent<AnimationAgent | PrimitiveAnimAgent>
EffectEnchant:      count > 0 ? EnsureAgent : RecycleAgent
```

**一个 Actor 应该有哪些 Agent，没有任何一个地方能看清楚。** 规则分散在 4 个 Enchant 里，且 ModelEnchant 和 AnimationEnchant 的路由逻辑重复。

### 2.3 Dispatch 链路太深

```
RIL → RILBucket.RILDispatch
  → enchantdict[ril.id]           ← 字典查找
    → foreach AgentEnchant        ← 遍历
      → enchant.DoRIL(ril)        ← 虚调用 + cast
  → world.GetAgents(ril.actor)    ← 字典查找
    → foreach Agent               ← 遍历
      → agent.DoRIL(ril)          ← 虚调用
        → rilactions[type]        ← 字典查找
          → foreach Invoker       ← 遍历
            → invoker.Invoke(ril) ← 虚调用 + actor check + cast
              → Action<T>(ril)    ← 委托调用
```

7 层间接。最终目的很简单：**告诉 Agent「数据变了」**。

### 2.4 Agent 交叉依赖隐式

```
实际依赖图:

SpatialAgent         ← 无依赖
ModelAgent           → 读 SpatialAgent.position/rotation/scale
PrimitiveMeshAgent   → 读 SpatialAgent
EffectAgent          → 读 SpatialAgent
SoundAgent           → 读 SpatialAgent（音效 3D 定位）
AnimationAgent       → 读 ModelAgent.node
PrimitiveAnimAgent   → 读 PrimitiveMeshAgent.meshinstance
```

每个 Agent 在 `OnChase` 里 `world.GetAgent<T>(actor)` 查找依赖，靠 `null` 检查容错。没有显式声明，没有更新顺序保证。

### 2.5 Agent 收到的是黑盒 RIL

SpatialAgent 收到 RIL_SPATIAL 就重置插值——即使只有 scale 变了，position/euler 也被迫重新插值，导致视觉抖动。

### 2.6 回滚时渲染层处理粗糙

帧同步回滚流程：

```
1. Stage.Snapshot()          — Logic 保存帧 N 状态
2. World.Snapshot()          — Render 清空 snapshotagents 列表
3. ... 重新模拟，产生新 RIL、新 Agent ...
4. Stage.Restore()           — Logic 回滚到帧 N（末尾 rilsync.Translate 产生事件）
5. World.Restore()           — Render 删除 snapshotagents（错误未来的 Agent）
                                对剩余 Agent 调 Flash()（立即同步）
```

**问题**：
- `Flash()` 是全量同步——所有 Agent 立即跳到最新状态，没有过渡
- 回滚期间产生的大量 RIL 直接丢弃（`OnRestore` 清空所有队列）
- Agent 不知道发生了回滚，无法做平滑过渡
- **`Stage.Restore` 末尾的 `rilsync.Translate()` 会产生事件，重模拟也会产生事件，Salute 是 fire-and-forget，音效/伤害飘字会重复触发**

### 2.7 视野管理不在渲染层

当前系统对所有 Actor 一视同仁——只要 RIL 到达就创建 Agent。这本身不是问题：

- **状态同步下**：AOI 是服务端的职责，决定发不发。客户端收到什么就表达什么
- **帧同步下**：所有 Actor 都模拟，渲染层全部表达。视野裁剪是 Godot 视锥剔除的事，不是 Agent 的事

---

## 3. 重构方案

### 3.1 核心思路：两阶段管线

**数据到达 ≠ 数据表达。** 先把一帧的 RIL 全部存好，再让 Agent 统一表达。

```
Phase 1: RIL 收集（LocalDirector.OnTick 前半段）
  → drain rilqueue   → RILBucket.SetRIL  （只存，不分发）
  → drain eventqueue → RILBucket.SetEvent（事件入待处理队列，不立即触发 Salute）

  ──────── 所有 RIL 数据已就绪 ────────

Phase 2: Agent 表达（World.OnTick）
  → ReconcileAgents()    ← 根据 RIL 存在性，创建/销毁 Agent
  → ExpressAgents(dt)    ← 分层 pipeline 更新所有 Agent
```

事件（RIL_EVENT）处理见 3.7（不再 fire-and-forget，回滚时需幂等）。

### 3.2 RILBucket 瘦身

```csharp
public class RILBucket : Comp
{
    // ===== 保留：存储 + 查询 =====
    public void SetRIL(IRIL ril)                    // 只存，不分发
    public bool SeekRIL<T>(ulong actor, out T ril)
    public T GetRIL<T>(ulong actor)
    public List<T> GetRILS<T>()
    public List<ulong> GetActors()                  // 获取所有有 RIL 的 Actor 列表
    public void LossRIL(ulong actor)

    // ===== 保留：事件 =====
    public void SetEvent(IRIL_EVENT e)              // 事件入待处理队列（非立即触发）

    // ===== 删除 =====
    // RILDispatch()       — 不再分发
    // SetDiff()           — 由 RIL.Merge 替代（RIL 重构后）
    // crossdict           — 不再需要
    // enchantdict         — 不再需要
}
```

只做两件事：**存状态** + **收事件**。事件何时触发 Salute 由 World 决定（见 3.7）。

### 3.3 Agent 生命周期：自主决策

每个 Agent 自己决定是否应该存在：

```csharp
public abstract class Agent
{
    public ulong actor { get; private set; }
    public World world { get; private set; }

    /// <summary>
    /// 每帧调用，表达 RIL 数据
    /// </summary>
    public void Express(float dt)
    {
        // Reconcile 已过滤，此处为安全兜底
        if (false == OnShouldExist()) return;
        if (world.postrestore) { OnFlash(); return; }
        OnExpress(dt);
    }

    /// <summary>
    /// 决定此 Agent 是否应该存在（子类覆盖）
    /// 事件驱动 Agent（如 SoundAgent）返回 true，不参与 Reconcile
    /// </summary>
    protected virtual bool OnShouldExist() => true;

    /// <summary>
    /// 是否事件驱动（事件驱动 Agent 不参与 Reconcile，由 Salute 创建/销毁）
    /// </summary>
    protected virtual bool oneventdriven => false;

    /// <summary>
    /// 表达 RIL 数据（子类实现）
    /// </summary>
    protected abstract void OnExpress(float dt);

    /// <summary>
    /// 回滚后直接同步到最新状态（子类覆盖）
    /// </summary>
    protected virtual void OnFlash() { }

    /// <summary>
    /// 从 RILBucket 拉取数据
    /// </summary>
    protected bool SeekRIL<T>(out T ril) where T : IRIL, new()
        => world.rilbucket.SeekRIL(actor, out ril);

    /// <summary>
    /// 获取同 Actor 的其他 Agent（依赖引用缓存，见 3.5）
    /// </summary>
    protected T GetAgent<T>() where T : Agent
        => world.GetAgent<T>(actor);
}
```

**事件驱动 Agent（SoundAgent）特例**：`oneventdriven => true`，Reconcile 跳过。

**创建**：`SoundSalute.OnSaluteUnique` 调 `World.EnsureAgent<SoundAgent>(actor)` 确保存在后播放。

**销毁**：SoundAgent 自主负责生命周期——`OnExpress` 内检测音频是否播放完成，完成后调 `world.RmvAgent(this)` 自销毁，避免泄漏。不跟随 RIL（事件驱动的 Agent 没有对应 RIL 域，`RIL_LOSS` 不适用于它）。

不强行套 `OnShouldExist` 拉取模式，承认模型不纯粹。

### 3.4 路由解耦：标记放 RIL，不查配置

原方案让 `OnShouldExist` 查 `world.engine.cfg.location.ModelInfos` 判断 primitive/glb。这把配置表查询耦合进 Agent 存在性判断，配置加载延迟会让 Agent 抖动。

**改为**：路由判断由 Translator 完成（Translator 本就在查配置），结果写入 RIL 字段。Agent 只看 RIL：

```csharp
// RIL_FACADE_MODEL 加路由标记
public partial class RIL_FACADE_MODEL : IRIL
{
    public int model;
    /// <summary>
    /// 模型路由标记（由 Translator 查 ModelInfos 填充）
    /// </summary>
    public RIL_DEFINE.MODEL_KIND kind;
}

// FacadeModelTranslator.OnRIL 填充 kind
protected override void OnRIL(FacadeInfo info, RIL_FACADE_MODEL ril)
{
    ril.model = info.model;
    if (false == stage.cfg.location.ModelInfos.TryGetValue(info.model, out var mi))
    {
        ril.kind = RIL_DEFINE.MODEL_KIND_NONE;
        return;
    }
    ril.kind = "primitive" == mi.Type
        ? RIL_DEFINE.MODEL_KIND_PRIMITIVE
        : RIL_DEFINE.MODEL_KIND_MODEL;
}
```

各 Agent 的 ShouldExist 只看 RIL 的 `kind`：

```csharp
// ModelAgent — kind == MODEL
protected override bool OnShouldExist()
{
    if (false == SeekRIL<RIL_FACADE_MODEL>(out var ril)) return false;
    return RIL_DEFINE.MODEL_KIND_MODEL == ril.kind;
}

// PrimitiveMeshAgent — kind == PRIMITIVE
protected override bool OnShouldExist()
{
    if (false == SeekRIL<RIL_FACADE_MODEL>(out var ril)) return false;
    return RIL_DEFINE.MODEL_KIND_PRIMITIVE == ril.kind;
}

// AnimationAgent — 有动画且 kind == MODEL
protected override bool OnShouldExist()
{
    if (false == SeekRIL<RIL_FACADE_ANIMATION>(out _)) return false;
    if (false == SeekRIL<RIL_FACADE_MODEL>(out var model)) return false;
    return RIL_DEFINE.MODEL_KIND_MODEL == model.kind;
}
```

路由逻辑内聚到 RIL + Agent，Translator 查一次配置，Agent 不再碰配置表。

### 3.5 Agent 更新顺序：分层 pipeline（3 层，非 2 次遍历）

原方案用"两次遍历"覆盖 3 层依赖链（Spatial → Model → Animation），靠 null 容错。这把"靠运气"从 RIL 到达顺序挪到 Agent 更新顺序，没真正解决。

**改为 3 层 pipeline**，依赖链每层一个 phase：

```csharp
// World.OnTick
private void OnTick(TickEvent e)
{
    ReconcileAgents();

    var dt = e.tick;

    // Phase A: 空间层（被最多 Agent 依赖，可用 SpatialBatch 并行）
    ExpressSpatial(dt);

    // Phase B: 外观层（依赖 Spatial）
    ExpressFacade(dt);

    // Phase C: 动画/特效层（依赖 Model/Mesh）
    ExpressAnimAndEffect(dt);

    // Phase D: 其余 Agent（事件驱动等）
    ExpressRest(dt);
}

private void ExpressSpatial(float dt)
{
    // 保留 SpatialBatch 并行能力（见 3.11）
    spatialbatch.Tick(dt);
}

private void ExpressFacade(float dt)
{
    foreach (var kv in agentdict)
    {
        var timescale = SeekTimeScale(kv.Key);
        if (kv.Value.TryGetValue(typeof(ModelAgent), out var agent))
            agent.Express(dt * timescale);
        if (kv.Value.TryGetValue(typeof(PrimitiveMeshAgent), out var pagent))
            pagent.Express(dt * timescale);
    }
}

private void ExpressAnimAndEffect(float dt)
{
    foreach (var kv in agentdict)
    {
        var timescale = SeekTimeScale(kv.Key);
        if (kv.Value.TryGetValue(typeof(AnimationAgent), out var a)) a.Express(dt * timescale);
        if (kv.Value.TryGetValue(typeof(PrimitiveAnimAgent), out var pa)) pa.Express(dt * timescale);
        if (kv.Value.TryGetValue(typeof(EffectAgent), out var eff)) eff.Express(dt * timescale);
    }
}

private void ExpressRest(float dt)
{
    foreach (var kv in agentdict)
    foreach (var agent in kv.Value.Values)
    {
        if (agent is SpatialAgent) continue;
        if (agent is ModelAgent) continue;
        if (agent is PrimitiveMeshAgent) continue;
        if (agent is AnimationAgent) continue;
        if (agent is PrimitiveAnimAgent) continue;
        if (agent is EffectAgent) continue;
        var timescale = SeekTimeScale(kv.Key);
        agent.Express(dt * timescale);
    }
}
```

**为什么 3 层而非拓扑排序**：依赖链固定（Spatial → Model → Animation），不需要通用拓扑算法的复杂度。3 个 phase 写死，简单正确。若未来依赖链变深（>4 层），再上拓扑。

**依赖引用缓存**：每个 Agent 缓存依赖引用，避免每帧 `GetAgent` 字典查找。Reconcile 时由 World 注入：

```csharp
public class ModelAgent : Agent
{
    private SpatialAgent spatialnode { get; set; }

    // World.Reconcile 时调用，刷新依赖引用
    public void BindDependencies()
    {
        spatialnode = world.GetAgent<SpatialAgent>(actor);
    }

    protected override void OnExpress(float dt)
    {
        if (null == spatialnode) return;  // 依赖未就绪，下一帧再来
        // ...
    }
}
```

```csharp
// ReconcileAgents 内部，创建/刷新后统一绑定依赖
private void ReconcileAgents()
{
    // ... 创建/销毁 Agent（见 3.6）...

    // 统一刷新所有 Agent 的依赖引用
    foreach (var kv in agentdict)
    foreach (var agent in kv.Value.Values)
    {
        if (agent is IBindDependencies bind) bind.BindDependencies();
    }
}
```

`IBindDependencies` 接口让需要缓存的 Agent 实现，Reconcile 后统一刷新。每帧只刷一次，比每帧 `GetAgent` 字典查找高效。

### 3.6 Agent 创建/销毁：Reconcile，先判后建

原方案 `TryEnsure` 先创建再靠 `OnShouldExist` 回收——会触发 `OnReady`（`ModelAgent.OnReady` 设 `loaddirty=true`，下一帧 `Load` 加载 `.tscn`），**创建即销毁是浪费且有副作用**。

**改为先判后建**：先收集每个 Actor 应该存在的 Agent 类型集合，再 Ensure，不创建不该存在的：

```csharp
private void ReconcileAgents()
{
    // 收集所有有 RIL 的 Actor
    var actors = ObjectPool.Ensure<List<ulong>>();
    actors.AddRange(rilbucket.GetActors());

    // 第一遍：为每个 Actor 计算应该存在的 Agent 类型集合
    var shouldset = ObjectPool.Ensure<Dictionary<(ulong, Type), bool>>();
    foreach (var actor in actors)
    {
        shouldset[(actor, typeof(SpatialAgent))] = rilbucket.SeekRIL<RIL_SPATIAL>(actor, out _);

        if (rilbucket.SeekRIL<RIL_FACADE_MODEL>(actor, out var modelril))
        {
            shouldset[(actor, typeof(ModelAgent))] = RIL_DEFINE.MODEL_KIND_MODEL == modelril.kind;
            shouldset[(actor, typeof(PrimitiveMeshAgent))] = RIL_DEFINE.MODEL_KIND_PRIMITIVE == modelril.kind;
        }

        if (rilbucket.SeekRIL<RIL_FACADE_ANIMATION>(actor, out _))
        {
            // 路由依赖 modelril.kind，已在上面查过
            bool ismodel = shouldset.TryGetValue((actor, typeof(ModelAgent)), out var m) && m;
            shouldset[(actor, typeof(AnimationAgent))] = ismodel;
            bool isprim = shouldset.TryGetValue((actor, typeof(PrimitiveMeshAgent)), out var p) && p;
            shouldset[(actor, typeof(PrimitiveAnimAgent))] = isprim;
        }

        if (rilbucket.SeekRIL<RIL_FACADE_EFFECT>(actor, out var effril))
        {
            shouldset[(actor, typeof(EffectAgent))] = effril.effectdict.Count > 0;
        }
    }

    // 第二遍：按集合创建缺失的 Agent
    foreach (var kv in shouldset)
    {
        if (false == kv.Value) continue;
        if (null != GetAgent(kv.Key.Item2, kv.Key.Item1)) continue;
        AddAgent(kv.Key.Item2, kv.Key.Item1);
    }

    // 第三遍：移除不该存在的 Agent（含事件驱动 Agent 之外的）
    var rmvlist = ObjectPool.Ensure<List<Agent>>();
    foreach (var kv in agentdict)
    foreach (var agent in kv.Value.Values)
    {
        if (agent.oneventdriven) continue;  // 事件驱动 Agent 不参与
        bool should = shouldset.TryGetValue((kv.Key, agent.GetType()), out var s) && s;
        if (false == should) rmvlist.Add(agent);
    }
    foreach (var agent in rmvlist) RmvAgent(agent);

    // 清理
    actors.Clear(); ObjectPool.Set(actors);
    shouldset.Clear(); ObjectPool.Set(shouldset);
    rmvlist.Clear(); ObjectPool.Set(rmvlist);

    // 刷新依赖引用（见 3.5）
    BindAllDependencies();
}
```

路由判断只做一次（在第一遍），Ensure 时直接用结果，不创建不该存在的 Agent。

### 3.7 回滚事件幂等：frame 去重（不留开放问题）

原方案把"回滚重模拟产生的事件是否重复触发 Salute"留作开放问题。但 Salute 是 fire-and-forget（音效播放、伤害飘字），重模拟产生确定性事件 = **音效播两遍、伤害飘字飘两次**。这是必须解决的正确性问题。

**改为**：`IRIL_EVENT` 加 `frame` 字段，Salute 维护"已处理帧号集合"，重复的丢弃：

```csharp
public abstract class IRIL_EVENT
{
    public abstract ushort id { get; }
    public ulong actor { get; private set; }
    /// <summary>
    /// 逻辑帧号（用于回滚幂等去重）
    /// </summary>
    public long frame { get; private set; }

    public void Ready(ulong actor, long frame) { /* ... */ }
}

public abstract class RILSalute<T> : RILSalute where T : IRIL_EVENT
{
    /// <summary>
    /// 已处理的 (actor, id, frame) 集合，用于回滚幂等
    /// </summary>
    private HashSet<(ulong, ushort, long)> processed { get; set; }

    protected override void OnSalute(T e)
    {
        var key = (e.actor, e.id, e.frame);
        if (false == processed.Add(key)) return;  // 已处理，丢弃
        OnSaluteUnique(e);
    }

    protected abstract void OnSaluteUnique(T e);
}
```

**回滚时清理 processed**：回滚到帧 N 时，清理 `frame > N` 的记录，让重模拟产生的事件能重新触发（不会因旧记录误判为重复）。同时清理 `frame < N - WINDOW_SIZE` 的过期记录，防止无界增长。

```csharp
// World.Restore 内（重模拟前调用，此时无法知道哪些 actor 会变）
public void Restore(long rollbackframe)
{
    // ... 移除错误未来 Agent ...

    // 清理回滚点之后的事件去重记录
    // 同时清理过期滑动窗口（保留最近 N 帧，无界增长防泄漏）
    saluteprocessor.CleanupAfter(rollbackframe);
    saluteprocessor.CleanupBefore(rollbackframe - EVENT_WINDOW_SIZE);

    // 清空 dirtyactors，待 OnTick Phase 1 drain rilqueue 时收集（见 3.8）
    dirtyactors.Clear();
    postrestore = true;
}
```

`EVENT_WINDOW_SIZE` 建议 300 帧（5 秒），足够覆盖回滚深度 + 重模拟长度。`CleanupBefore` 在普通帧也可每 N 帧调一次（如 60 帧一次），避免高频遍历。

**事件不再 fire-and-forget**：`RILBucket.SetEvent` 把事件入待处理队列，`World.OnTick` 末尾（Express 之后）统一触发 Salute。回滚时 `OnRestore` 清空待处理队列，避免回滚期间积压的事件触发。

### 3.8 postrestore 粒度：只 Flash 受影响的 Actor

原方案全局 Flash 所有 Agent——未受影响的 `SpatialAgent` 被 `OnFlash` 重置 `accumtime=0`，下一帧从头插值，**制造新的抖动**。

**改为**：只对重模拟后状态变化的 Actor 调 Flash。

**关键时序约束**：`GameplayDirector.Restore` 的调用链是 `OnRestore`（设 `restoreing=true`、`World.Restore`）→ 然后多次 `OnStep`（`stage.Restore` + 重模拟，RIL 进 `rilqueue`）→ `restoreing=false` 后 `OnTick` 收集 RIL。**`World.Restore` 在重模拟之前调用，此时无法知道哪些 actor 会变**。dirtyactors 必须在 `OnTick` Phase 1 drain rilqueue 时收集——所有产生了 RIL 的 actor 即为 dirty：

```csharp
// World
private HashSet<ulong> dirtyactors { get; set; }
public bool IsDirty(ulong actor) => dirtyactors.Contains(actor);
public void MarkDirty(ulong actor) => dirtyactors.Add(actor);

// LocalDirector.OnTick Phase 1：drain rilqueue 时收集
private void OnTick()
{
    if (restoreing) return;
    lock (@lock)
    {
        while (rilqueue.TryDequeue(out var ril))
        {
            world.rilbucket.SetRIL(ril);
            // postrestore 期间，记录所有产生 RIL 的 actor 为 dirty
            if (world.postrestore) world.MarkDirty(ril.actor);
        }
        while (eventqueue.TryDequeue(out var e)) world.rilbucket.SetEvent(e);
    }
}
```

```csharp
// Agent.Express 按 actor 过滤
public void Express(float dt)
{
    if (false == OnShouldExist()) return;
    if (world.postrestore && world.IsDirty(actor)) { OnFlash(); return; }
    OnExpress(dt);
}
```

**不再需要 `Stage.SeekDirtyActors`**——dirty actor 由重模拟产生的 RIL 自然推导，不需要 Logic 层额外提供接口。`Stage` 的 `cache.rmvactors`/`rmvactorset` 是回滚时被移除的 actor（用于 Agent 销毁），与 dirty（状态变化的 actor）是两个概念，不混用。

### 3.9 回滚处理完整时序

帧同步回滚时，Logic 线程会重新模拟多个帧，产生大量 RIL。两阶段管线天然适配：

```
Logic 线程（回滚 + 重模拟）:
  Stage.Restore() → 回滚到帧 N
  for frame in N..current:
      Stage.Step()          → 每帧产生 RIL
      RILSync.Translate()   → rilqueue.Enqueue

主线程:
  LocalDirector.OnRestore
    Phase 1: 清空 rilqueue/eventqueue
    RILBucket.LossAllRIL
    World.Restore(rollbackframe)
      → 删除错误未来 Agent
      → 清理 Salute 的 frame > rollbackframe 记录
      → dirtyactors.Clear()
      → postrestore = true
  LocalDirector.OnTick
    Phase 1: 收集
      drain rilqueue → RILBucket.SetRIL + MarkDirty(ril.actor)
      // postrestore 期间，每条 RIL 的 actor 记入 dirtyactors
      // 同一 (actor, type) 的多条 RIL，后到的覆盖先到的
      // 最终 RILBucket 中存的是重模拟后最后一帧的状态
      drain eventqueue → RILBucket.SetEvent（入待处理队列）
    Phase 2: 表达
      ReconcileAgents()     → 创建新 Actor 的 Agent，移除消失的
      ExpressAgents(dt)     → postrestore=true，dirty actor 走 OnFlash
      FlushEvents()         → 触发 Salute（frame 去重）
      postrestore = false
```

**关键效果**：
- 重模拟产生的中间帧 RIL 被自然覆盖，Agent 只看到最终状态
- 重模拟产生的事件通过 frame 去重，不重复触发
- 只有 dirty actor 的 Agent Flash，未受影响的不抖动

### 3.10 各 Agent 的 OnExpress 实现

**SpatialAgent**（依赖 RIL 重构的 fieldmask 做字段级响应——若 RIL 重构未完成，保留现状行为）：

```csharp
public class SpatialAgent : Agent
{
    public Vector3 position { get; private set; }
    public Quaternion rotation { get; private set; } = Quaternion.Identity;
    public float scale { get; private set; } = 1f;

    private Vector3 prevpos, nextpos;
    private Quaternion prevrot, nextrot;
    private float accumtime;

    protected override void OnExpress(float dt)
    {
        if (false == SeekRIL<RIL_SPATIAL>(out var ril)) return;

        var targetpos = ril.position.ToVector3();
        var targetrot = Quaternion.FromEuler(ril.euler.ToVector3() * MathF.PI / 180f);

        // RIL 重构后：只有 position/euler 变了才重启插值
        // RIL 重构前：fieldmask 不可用，行为同现状（每次都重启）
        if (targetpos != nextpos || targetrot != nextrot)
        {
            prevpos = position;
            prevrot = rotation;
            nextpos = targetpos;
            nextrot = targetrot;
            accumtime = 0f;
        }

        scale = ril.scale.AsFloat();

        accumtime += dt;
        var t = Mathf.Clamp(accumtime / GAME_DEFINE.LOGIC_TICK.AsFloat(), 0f, 1f);
        position = prevpos.Lerp(nextpos, t);
        rotation = prevrot.Normalized().Slerp(nextrot.Normalized(), t);
    }

    protected override void OnFlash()
    {
        if (false == SeekRIL<RIL_SPATIAL>(out var ril)) return;
        var pos = ril.position.ToVector3();
        var rot = Quaternion.FromEuler(ril.euler.ToVector3() * MathF.PI / 180f);
        position = pos; rotation = rot;
        prevpos = pos; prevrot = rot;
        nextpos = pos; nextrot = rot;
        scale = ril.scale.AsFloat();
        accumtime = 0f;
    }
}
```

**ModelAgent**：

```csharp
public class ModelAgent : Agent, IBindDependencies
{
    public Node3D node { get; private set; }
    private int currentmodel;
    private bool loaddirty = true;
    private SpatialAgent spatialnode { get; set; }

    public void BindDependencies()
    {
        spatialnode = world.GetAgent<SpatialAgent>(actor);
    }

    protected override void OnExpress(float dt)
    {
        if (SeekRIL<RIL_FACADE_MODEL>(out var ril))
        {
            if (ril.model != currentmodel) { loaddirty = true; currentmodel = ril.model; }
        }
        if (loaddirty) { Load(); loaddirty = false; }
        if (null == node) return;

        // spatialnode 已在 Phase A 更新
        if (null == spatialnode) return;
        node.Position = spatialnode.position;
        node.Quaternion = spatialnode.rotation;
        node.Scale = Vector3.One * spatialnode.scale;
        node.Visible = true;
    }
}
```

**AnimationAgent**：

```csharp
public class AnimationAgent : Agent, IBindDependencies
{
    private AnimationPlayer animplayer;
    private ModelAgent modelagent { get; set; }
    private string playname;
    private float tarduration;

    public void BindDependencies()
    {
        modelagent = world.GetAgent<ModelAgent>(actor);
    }

    protected override void OnExpress(float dt)
    {
        if (false == SeekRIL<RIL_FACADE_ANIMATION>(out var ril)) return;

        // modelagent 已在 Phase B 更新
        if (null == modelagent?.node) return;

        if (null == animplayer)
            animplayer = modelagent.node.FindChild("AnimationPlayer", true, false) as AnimationPlayer;
        if (null == animplayer) return;

        var animname = ResolveAnimName(ril);
        if (null == animname) return;
        tarduration = ril.animelapsed * Config.Int2Float;

        if (animplayer.CurrentAnimation != animname) animplayer.Play(animname);
        animplayer.SpeedScale = 0;
        animplayer.Seek(Mathf.Clamp(animplayer.CurrentAnimationPosition + dt, 0, tarduration), true);
    }
}
```

**EffectAgent**：

```csharp
public class EffectAgent : Agent, IBindDependencies
{
    private Dictionary<uint, (EffectInfo info, EffectController controller)> effects { get; set; }
    private SpatialAgent spatialnode { get; set; }

    public void BindDependencies()
    {
        spatialnode = world.GetAgent<SpatialAgent>(actor);
    }

    protected override void OnExpress(float dt)
    {
        if (false == SeekRIL<RIL_FACADE_EFFECT>(out var ril)) return;

        // RIL 重构后：ril 带 fieldmask，addedkeys/removedkeys 已由 Diff 填充
        // 直接读，不再自己 diff（痛点 R4 闭环）
        if (0 != (ril.fieldmask & RIL_FACADE_EFFECT.FM_REMOVED))
            foreach (var k in ril.removedkeys) RecycleEffect(k);
        if (0 != (ril.fieldmask & RIL_FACADE_EFFECT.FM_ADDED))
            foreach (var k in ril.addedkeys)
                if (false == effects.ContainsKey(k)) CreateEffect(ril.effectdict[k]);

        foreach (var kv in effects)
        {
            // ... 跟随 spatialnode 更新特效位置 ...
        }
    }
}
```

**EffectAgent 明确依赖 RIL 重构**：渲染层 Phase 1 保留内部 diff（读不到 fieldmask 时回退），Phase 6 改读 `addedkeys`/`removedkeys`。RIL 重构后 `addedkeys`/`removedkeys` 由 `Diff` 填充、`Merge` 后随 RIL 一起分发，Agent 直接消费，痛点 R4 闭环。

### 3.11 并行保留：SpatialBatch 接入 Phase A

原方案 `World.OnTick` 第一遍串行 foreach，砍掉了 `SpatialBatch` 的 `Parallel.ForEach`（现状 `rils.Count >= 32` 时并行）。这是性能退步。

**改为**：Phase A 由 `SpatialBatch` 驱动，保留并行：

```csharp
public class SpatialBatch : Batch
{
    protected override void OnTick(TickEvent e)
    {
        if (false == world.rilbucket.SeekRILS<RIL_SPATIAL>(out var rils)) return;

        var dt = e.tick;

        if (rils.Count >= 32)
            Parallel.ForEach(rils, ril => ProcessRIL(ril, dt));
        else
            foreach (var ril in rils) ProcessRIL(ril, dt);

        rils.Clear();
        ObjectPool.Set(rils);
    }

    private void ProcessRIL(RIL_SPATIAL ril, float dt)
    {
        var agent = world.GetAgent<SpatialAgent>(ril.actor);
        if (null == agent) return;

        var timescale = world.SeekTimeScale(ril.actor);
        agent.Express(dt * timescale);
    }
}
```

`SpatialAgent.Express` 自身做插值（逻辑从 `SpatialBatch.ProcessRIL` 迁入），并行由 `SpatialBatch` 保留。Phase A 不再是 World 的串行 foreach。

### 3.12 删除清单

| 删除 | 原因 |
|------|------|
| `AgentEnchant` 基类及所有子类 | 生命周期由 Agent.ShouldExist 自主决策 + Reconcile 统一管理 |
| `Invoker` / `Invoker<T>` | 不再需要 WatchRIL 回调 |
| `Agent.WatchRIL<T>()` | Agent 改为拉取模式 |
| `Agent.DoRIL()` | 不再需要推送 |
| `RILBucket.RILDispatch()` | 不再分发 |
| `RILBucket.enchantdict` | Enchant 删除后不需要 |
| `RILBucket.crossdict` | RIL 重构后由 Merge 替代 |
| `RILBucket.SetDiff()` | RIL 重构后由 Merge 替代 |
| `RILCross` 及子类 | 合并逻辑移入 RIL.Merge |
| `Agent.Flash()`（公开方法） | 回滚由 World.postrestore 标记 + Agent 内部 OnFlash 替代 |
| `Agent.ChaseStatus` | 不再需要追逐状态机 |
| `Agent.Chase()` / `OnChase()` / `OnArrived()` | 由 `Express()` / `OnExpress()` 替代 |

### 3.13 保留清单

| 保留 | 原因 |
|------|------|
| `RILSalute` 及子类 | 事件处理（加 frame 去重） |
| `RILBucket.SetEvent()` | 事件入口（改为入待处理队列） |
| `SpatialBatch` | 大量 Actor 时并行插值，Phase A 由它驱动 |
| `World.GetAgent<T>()` | Agent 间读取依赖 |
| `World.EnsureAgent<T>()` | 事件 Salute 主动创建 Agent（SoundAgent 等） |
| `World.Snapshot()` / `World.Restore()` | 回滚机制，Restore 内部简化 |

---

## 4. 完整管线时序

### 4.1 正常帧

```
Logic 线程                          主线程
─────────                          ────────
Stage.Step()
  RILSync.Translate()
    Translator × N                    │
      → rilqueue.Enqueue              │
      → eventqueue.Enqueue            │
                                      ▼
                              LocalDirector.OnTick
                                ┌──────────────────────┐
                                │ Phase 1: 收集         │
                                │  drain rilqueue       │ → RILBucket.SetRIL (只存)
                                │  drain eventqueue     │ → RILBucket.SetEvent (入队列)
                                ├──────────────────────┤
                                │ Phase 2: 表达         │
                                │  ReconcileAgents()    │ → 创建/销毁 Agent + 绑定依赖
                                │  ExpressSpatial(dt)   │ → SpatialBatch 并行插值
                                │  ExpressFacade(dt)    │ → Model/PrimitiveMesh
                                │  ExpressAnimAndEffect │ → Animation/Effect
                                │  ExpressRest(dt)      │ → 其余（事件驱动 Agent 等）
                                │  FlushEvents()        │ → 触发 Salute（frame 去重）
                                └──────────────────────┘
```

### 4.2 回滚帧

```
Logic 线程                          主线程
─────────                          ────────
Stage.Restore()                     │
  回滚到帧 N                         │
  rilsync.Translate() (产生事件)    │
for frame in N..current:            │
  Stage.Step()                      │
    → rilqueue.Enqueue (大量)       │
    → eventqueue.Enqueue             │
                                    ▼
                              LocalDirector.OnRestore
                                │  清空 rilqueue/eventqueue
                                │  RILBucket.LossAllRIL
                                │  World.Restore(rollbackframe)
                                │    → 删除错误未来 Agent
                                │    → 清理 Salute frame > N 记录
                                │    → dirtyactors.Clear()
                                │    → postrestore = true
                                ▼
                              LocalDirector.OnTick
                                ┌──────────────────────┐
                                │ Phase 1: 收集         │
                                │  drain rilqueue       │ → RILBucket.SetRIL + MarkDirty
                                │                       │   （postrestore 期间记录 dirty actor）
                                │                       │   （同 key 后面的覆盖前面的）
                                │  drain eventqueue     │ → 事件入待处理队列
                                ├──────────────────────┤
                                │ Phase 2: 表达         │
                                │  ReconcileAgents()    │ → 创建新 Agent，移除消失的
                                │  ExpressAgents:      │
                                │    postrestore=true   │
                                │    → dirty actor Flash │ → 跳到最终状态
                                │    → 非 dirty actor 正常│
                                │  FlushEvents()        │ → Salute（frame 去重，重复丢弃）
                                │  postrestore=false    │
                                └──────────────────────┘
```

---

## 5. 与 RIL 重构的关系（不假装正交）

两个重构**不**正交：

| 渲染层痛点 | 根因 | RIL 重构能解决？ | 渲染层独立先做时 |
|-----------|------|----------------|----------------|
| R1: Agent 无法感知字段变化 | RIL 整体替换 | **是** — fieldmask | 无法解决，保留现状行为 |
| R2: Agent 交叉依赖隐式 | 无依赖图 | 否 | **可独立解决**（分层 pipeline + 依赖注入） |
| R3: Agent 回读 RILBucket | Agent 不信任推送 | 部分 | **可独立解决**（统一拉取模式） |
| R4: EffectAgent 内部 diff | RIL 整体替换 | **是** — Merge 的 added/removed | 无法解决，保留内部 diff |
| R5: RILBucket 职责过多 | 历史演进 | 部分 | **可独立瘦身**（删 Dispatch/enchantdict） |
| R6: Dispatch 链太长 | 多层间接 | 否 | **可独立解决**（删除 Enchant/Invoker） |
| R7: Agent 生命周期散落 | 无统一管理 | 否 | **可独立解决**（Reconcile + OnShouldExist） |

**结论**：R2、R3、R5、R6、R7 可独立解决；R1、R4 必须等 RIL 重构。

**推荐顺序**：

1. **先做 RIL 重构 Phase 1-2**（IRIL 扩展 + 值类迁移，产出 fieldmask 和 Merge）—— 详见 RIL 重构文档
2. **再做渲染层重构**——能用上 fieldmask（R1 闭环）和 Merge（R4 闭环）
3. **若先做渲染层**：R1/R4 标记为"Phase 2 闭环"，做完 RIL 重构后回来改 `SpatialAgent`/`EffectAgent`，不假装一次做完

---

## 6. 迁移计划

### Phase 1：Agent 基类改造

1. Agent 加 `OnShouldExist()`、`OnExpress(float dt)`、`oneventdriven`、`SeekRIL<T>()`
2. 加 `IBindDependencies` 接口，`BindDependencies()` 方法
3. 保留 `WatchRIL`/`Chase` 但标记 `[Obsolete]`

### Phase 2：World 管线改造

1. World.OnTick 改为两阶段：收集 + 分层 Express（Phase A/B/C/D）
2. World 加 `ReconcileAgents()`（先判后建）
3. World 加 `BindAllDependencies()`
4. RILBucket.SetRIL 删除 `RILDispatch`
5. SpatialBatch 接入 Phase A，`SpatialAgent.Express` 内化插值逻辑

### Phase 3：逐 Agent 迁移

1. **SpatialAgent** — 无依赖，先迁移，实现 `IBindDependencies`（空）
2. **ModelAgent / PrimitiveMeshAgent** — 依赖 SpatialAgent，实现 `IBindDependencies`
3. **EffectAgent** — 依赖 SpatialAgent，保留内部 diff（R1/R4 待 RIL 重构后替换）
4. **AnimationAgent / PrimitiveAnimAgent** — 依赖 ModelAgent/MeshAgent
5. **SoundAgent** — 事件驱动，`oneventdriven => true`，由 Salute 创建/销毁

### Phase 4：回滚机制改造

1. `IRIL_EVENT` 加 `frame` 字段
2. `RILSalute` 加 `processed` 集合 + `CleanupAfter(frame)` + `CleanupBefore(frame)` 滑动窗口清理（保留最近 N 帧，见 3.7）
3. `RILBucket.SetEvent` 改为入待处理队列
4. World.OnTick 末尾 `FlushEvents()` 统一触发 Salute
5. World 加 `dirtyactors` 集合 + `MarkDirty`/`IsDirty`
6. LocalDirector.OnTick Phase 1 drain rilqueue 时，postrestore 期间 `MarkDirty(ril.actor)` 收集
7. `World.Restore(rollbackframe)` 设 `postrestore=true` + `dirtyactors.Clear()`（重模拟前调用，不在此处填充）

### Phase 5：清理

1. 删除 `AgentEnchant` 及所有子类
2. 删除 `Invoker` / `WatchRIL` / `DoRIL` / `Chase` / `OnChase` / `OnArrived` / `ChaseStatus`
3. 删除 `RILBucket.RILDispatch` / `enchantdict`

### Phase 6（RIL 重构完成后）：闭环 R1/R4

1. `SpatialAgent.OnExpress` 读 `fieldmask`，只在 position/euler 变化时重启插值
2. `EffectAgent.OnExpress` 删除内部 diff，直接读 Merge 后的 `effectdict`（added/removed）

---

## 7. 开放问题（收敛）

原方案留了 5 个开放问题，本方案处置如下：

| 原开放问题 | 本方案处置 |
|-----------|-----------|
| 1. ReconcileAgents 的频率 | 每帧遍历，但先判后建避免浪费。若性能不足，加 dirty actor 标记按需触发 |
| 2. SpatialBatch 的去留 | **保留**（3.11），Phase A 由 SpatialBatch 驱动，`>=32` 并行 |
| 3. SoundAgent 的特殊性 | `oneventdriven => true`，Reconcile 跳过，由 Salute 创建/销毁（3.3） |
| 4. 回滚时事件的处理 | **必须解决**：`IRIL_EVENT.frame` + Salute `processed` 集合去重（3.7） |
| 5. Flash 的粒度 | **按 dirty actor**：OnTick Phase 1 drain rilqueue 时收集（postrestore 期间 `MarkDirty`），只 Flash 受影响 Agent（3.8） |

**未在本方案解决的**（依赖 RIL 重构）：
- R1 字段级响应：RIL 重构产出 fieldmask 后闭环（Phase 6）
- R4 EffectAgent diff 消除：RIL 重构产出 Merge 的 added/removed 后闭环（Phase 6）
