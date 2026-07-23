# 双模同步体系 — 总设计愿景

Goblin 需要同时支持**帧同步（Lockstep）**和**状态同步（State Sync）**两种网络模型。RIL 体系 + 渲染层两层重构的最终目的是：**两种模式共享同一套基础设施，差异只在传输层**。

---

## 1. 诉求

### 1.1 双模共享基础设施

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

**核心要求**：RIL 定义和 Translator 两种模式完全共享。差异只在传输层——帧同步是本地函数调用，状态同步是序列化+网络。**传输层必须可插拔。**

### 1.2 具体诉求清单

| # | 诉求 | 说明 |
|---|------|------|
| A | 一份 RIL 定义，两种模式复用 | 新增 RIL 类型时只写一次，帧同步和状态同步自动可用 |
| B | 字段级 Diff | 状态同步带宽优化——只发变化的字段，不是整个 RIL |
| C | 序列化能力 | RIL 能序列化/反序列化，状态同步基础 |
| D | 帧同步零开销 | 本地传输不碰序列化，不 Clone 不必要的快照 |
| E | 传输层可插拔 | 帧同步切状态同步只需换 transport 实现，不动 RIL 和 Translator |
| F | 回滚正确 | 帧同步回滚时渲染不抖动、事件不重复 |
| G | 渲染模式无关 | Agent 不关心数据来自本地推送还是网络反序列化 |
| H | 删除特例路径 | 集合类不再需要独立 Diff 通道，所有 RIL 走同一条管线 |

---

## 2. 当前问题

### 2.1 双模支持缺失

| 能力 | 帧同步现状 | 状态同步现状 | 需要的 |
|------|-----------|-------------|--------|
| RIL 序列化 | 不需要（本地传引用） | **不存在** | RIL 必须可序列化 |
| 字段级 Diff | 不需要（本地整体替换） | **不存在** | 只序列化变化字段 |
| 传输接口 | 硬编码 `stage.onril` | **不存在** | `IRILTransport` 抽象 |
| 集合类 Diff | 走独立路径 `IRIL_DIFF` | **不可序列化** | 统入 Snapshot Diff |

**结论**：当前架构只能跑帧同步。状态同步所需的序列化 + 字段 Diff + 网络传输全部缺失。

### 2.2 RIL 侧：3 条 Diff 路径

```
路径 1 — Full RIL（值类）：hash 比较 → 整体替换
路径 2 — DIFF RIL（集合类）：IRIL_DIFF + RILCross → 手动合并
路径 3 — hash 比较：OnCalcHashCode 手写
```

3 条路径互不兼容。新增集合 RIL 需写 RIL 类 + IRIL_DIFF 类 + RILCross 合并器（3 个文件）。手写 hash 有静默 bug（漏字段、写错变量，不报错只不同步）。无 Serialize 接口。

### 2.3 渲染侧

| 问题 | 影响 |
|------|------|
| RIL 逐条到达、逐条分发 | 靠顺序碰巧正确，非显式时序保证 |
| Agent 生命周期散落 4 个 Enchant | 一个 Actor 该有哪些 Agent，无处看清 |
| Dispatch 7 层间接 | Enchant → Action → Invoker → 委托 |
| Agent 收到黑盒 RIL | 无 fieldmask，无法字段级响应 |
| 回滚 Flash 全局抖动 + 事件重复 | 未受影响 Agent 也 Flash；音效播两遍 |

### 2.4 痛点层级

```
           ┌─────────────────────────────────┐
           │   状态同步完全不可行              │  ← 无序列化 + 无字段 Diff
           │   (2.1)                          │
           └───────────────┬─────────────────┘
                           │ 依赖
           ┌───────────────▼─────────────────┐
           │   3 条 Diff 路径互不兼容          │  ← 值类/集合类/高频类三套机制
           │   (2.2)                          │
           └───────────────┬─────────────────┘
                           │ 导致
           ┌───────────────▼─────────────────┐
           │   渲染层收到黑盒 RIL              │  ← Agent 不知道哪些字段变了
           │   (2.3)                          │
           └─────────────────────────────────┘
```

**根因**：RIL 缺乏统一的 Diff/Serialize 接口 → 状态同步不可行 → 渲染层被黑盒 RIL 拖累。

---

## 3. 核心架构：双模在同一套管道上运行

```
                      ┌──────────────────────────┐
                      │      RIL Definition        │  ← 两种模式共享
                      │  (SPATIAL / STATE_MACHINE  │
                      │   FACADE_MODEL / EFFECT    │
                      │   ACTOR / ATTRIBUTE ...)   │
                      └────────────┬─────────────┘
                                   │
                      ┌────────────▼─────────────┐
                      │       Translator          │  ← 两种模式共享
                      │   BehaviorInfo → RIL 填充 │
                      └────────────┬─────────────┘
                                   │
                      ┌────────────▼─────────────┐
                      │        RILSync            │  ← 两种模式共享
                      │   Snapshot Diff (统一)    │
                      │   fieldmask 产出差量      │
                      └────────────┬─────────────┘
                                   │
                      ┌────────────┴─────────────┐
                      │     IRILTransport          │  ← **模式分叉点**
                      └───┬──────────────────┬───┘
                          │                  │
              ┌───────────▼──────┐  ┌────────▼──────────┐
              │  帧同步 (Local)   │  │ 状态同步 (Network) │
              │  直接 onril      │  │ Serialize → Send   │
              │  → RILBucket     │  │ → Client Deserialize│
              │  零开销          │  │ 按 fieldmask 压缩  │
              └────────┬─────────┘  └────────┬───────────┘
                       │                    │
              ┌────────▼────────────────────▼───────────┐
              │              RILBucket                   │  ← 两种模式共享
              │   Merge(oldril, fieldmask)               │
              │   rildict / historydict (状态同步插值)   │
              └────────┬────────────────────────────────┘
                       │
              ┌────────▼────────────────────────────────┐
              │              Agent                       │  ← 两种模式共享
              │   读 fieldmask 字段级响应                │
              │   读 addedkeys/removedkeys 集合变化      │
              │   不关心 RIL 来源（本地 / 网络）         │
              └─────────────────────────────────────────┘
```

**Agent 模式无关**：同一份 Agent 代码，帧同步收到本地 RIL 直接表达，状态同步收到网络 RIL 也直接表达。Agent 不关心传输路径。

---

## 4. 两个重构如何协同

### 4.1 RIL 重构：建立统一管道

```
现状                              改造后
────                              ──────
值类: hash → 整体替换            值类: Diff 字段比较 → fieldmask → Merge
集合: IRIL_DIFF → RILCross       集合: Diff 集合比较 → added/removed → Merge
高频: hash → 整体替换 (浪费)      高频: ishighfrequency → 跳过 Diff (零开销)
```

**统一接口**：所有 RIL 实现 `Diff(snapshot) → ulong fieldmask` + `Merge(other, mask)`。

### 4.2 渲染层重构：消费管道产出

```
RIL 重构产出                        渲染层消费
───────────                        ──────────
fieldmask (64 位)          →      Agent.OnExpress 只处理变化字段
                                    (SpatialAgent: position/euler 变才重启插值)

Merge 后的 addedkeys       →      EffectAgent 直接读，不再自己 diff
       / removedkeys              (痛点 R4 闭环)

IRILTransport              →      Agent 不感知传输路径
                                    (帧同步/状态同步表达逻辑完全相同)

IRIL_EVENT.frame           →      Salute 回滚去重
                                    (processed 集合防重复触发)
```

### 4.3 依赖关系

```
RIL 重构 Phase 1-3                    渲染层重构 Phase 4-7
──────────────────                    ─────────────────────
IRIL 扩展 (Diff/Merge/Serialize)      Agent 基类改造
值类迁移 (fieldmask 可用)             两阶段管线
集合类迁移 (added/removed 可用)       分层 Express
删除 IRIL_DIFF/RILCross              回滚机制
        ↓                                    ↓
        └──────────── Phase 8 ──────────────┘
        闭环 R1 (字段级响应) + R4 (EffectAgent 减负)

Phase 9 (可选): Source Generator
```

5 个渲染层痛点中 3 个（R2/R3/R5/R6/R7）可独立解决，2 个（R1/R4）**必须等 RIL 重构完成后才有 fieldmask 和 added/removed 可用**。

---

## 5. 好处

| 好处 | 机制 |
|------|------|
| **双模共享** | 同一份 RIL 定义 + Translator，帧同步和状态同步只换 transport |
| **一条 Diff 路径** | 值类/集合类/高频类全走 `Diff → fieldmask → Merge`，删 ~8 个特例类 |
| **字段级 Diff** | fieldmask 标记变化字段，状态同步只序列化脏字段，节省带宽 |
| **帧同步零开销** | `LocalTransport` 不碰 Serialize；`ishighfrequency` 跳过 Clone/Diff |
| **回滚正确** | dirty actor Flash（不全局抖动）+ 事件 frame 去重（不重复触发） |
| **Agent 模式无关** | 同一份 Agent，帧同步/状态同步表达逻辑完全相同 |
| **可插拔传输** | 切模式 = 换 `IRILTransport` 实现，不动 RIL/Translator/Agent |
| **路由内聚** | Translator 查配置写入 RIL 字段，Agent 不看配置表 |

---

## 6. 坏处

| 坏处 | 说明 |
|------|------|
| 迁移量大 | RIL 3 Phase + 渲染 5 Phase + 闭环 2 Phase，约 28 天 |
| 集合类每帧 Clone | `FacadeEffectTranslator` 每帧克隆 effectdict（跨线程安全），新增 GC 压力 |
| 状态同步 RIL 整包序列化 | 集合类不搞增量（合并复杂），大字典带宽浪费；Phase 9 上生成器后评估 |
| EffectInfo 需 IEquatable | Diff 字段比较依赖值相等，字段多时实现繁琐 |
| 渲染 4 Phase 硬编码 | 依赖链固定（Spatial→Model→Animation），新增层需改管线 |
| Reconcile 每帧全量遍历 | 先判后建避免创建即销毁，但大量 actor 仍有开销 |

---

## 7. 迁移路线

```
Phase 1 (~3 天): IRIL 扩展 + 双路径过渡
    Diff/Merge/Serialize/Clone 接口，hash 路径保留兼容

Phase 2 (~4 天): 值类 RIL 迁移
    SPATIAL/STATE_MACHINE/FACADE_MODEL 实现 Diff/Merge → fieldmask 可用

Phase 3 (~3 天): 集合类 RIL 迁移
    FACADE_EFFECT/ACTOR Diff 算 added/removed → 删除 IRIL_DIFF/RILCross
────────────────── RIL 重构完成，进入渲染层 ──────────────────

Phase 4 (~5 天): Agent 基类 + World 管线
    两阶段、Reconcile、分层 Express、RILBucket 瘦身

Phase 5 (~4 天): 逐 Agent 迁移
    Spatial→Model→Effect→Animation→SoundAgent 迁到 OnExpress

Phase 6 (~3 天): 回滚机制
    IRIL_EVENT.frame、Salute processed 去重、dirtyactor Flash

Phase 7 (~1 天): 清理废弃代码
    删 Enchant/Invoker/Chase/DoRIL/WatchRIL

Phase 8 (~2 天): 闭环 R1/R4
    SpatialAgent 读 fieldmask、EffectAgent 读 added/removed
────────────────── 整体完工 ──────────────────

Phase 9 (~3 天, 可选): Source Generator
    生成 Diff/Merge/Serialize，消除手写
─────────────────────────────────────────────────
总计：~28 天（含可选 Phase 9）
```

---

## 8. 关键数字

| 项目 | 数值 |
|------|------|
| 删除类/接口 | ~23（RIL 侧 ~8 + 渲染侧 ~15） |
| fieldmask | 32→64 位 |
| Express Phase | 4 层（A: Spatial, B: Facade, C: Animation+Effect, D: Rest） |
| SpatialBatch 并行阈值 | ≥32 条 RIL |
| EVENT_WINDOW_SIZE | 300 帧（5 秒） |
| 帧同步开销 | 零（LocalTransport 直调，ishighfrequency 跳过 Clone） |
| 状态同步节省 | 只序列化变化字段（fieldmask 精确标记） |

---

## 9. 文档索引

| 文档 | 定位 |
|------|------|
| `DUAL_MODE_SYNC_VISION.md`（本文） | 总设计愿景：诉求、问题、双模共享架构 |
| `RIL_UNIFIED_DIFF_DESIGN.md` | RIL 同步体系：IRIL 接口、Snapshot Diff、集合类、传输层、序列化 |
| `RENDER_LAYER_DESIGN.md` | 渲染层：两阶段管线、Agent 生命周期、分层 Express、回滚、SpatialBatch |
| `RIL_AND_RENDER_OVERVIEW.md` | 两份设计的概要总览（含精简总结报告） |
