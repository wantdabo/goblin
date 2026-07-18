# Goblin 架构文档

> 2026-07-19 | 基于源码分析

---

## 一、设计理念

### 1.1 ECS-like 架构

Goblin 采用**类 ECS** 模式，但做了显著的 C# 化改造，使其更适合动作游戏的开发：

| 概念 | ECS 对应 | Goblin 实现 |
|------|----------|-------------|
| **Actor** | Entity | `ulong` 自增 ID，轻量实体句柄 |
| **Behavior** | System | 拥有 `Assemble → Tick → EndTick → Disassemble` 生命周期 |
| **BehaviorInfo** | Component | 纯数据容器，支持对象池、克隆、快照/恢复 |

与正统 ECS 的关键差异：
- **Behavior 直接持有逻辑**，而非纯函数式的 Job System
- **BehaviorInfo 通过泛型自动绑定**到对应 Behavior，减少样板代码
- **数据与逻辑的分离开销是可选的**：Behavior 可通过 `stage.GetBehaviorInfo<T>()` 按需获取

### 1.2 确定性优先

作为帧同步格斗游戏，**所有逻辑层运算必须是确定性的**：

- 逻辑层**仅使用定点数 `FP`（Fixed Point）**，禁止 `float`/`double`
- 随机数使用 **LCG（线性同余生成器）**，可通过种子重放
- **Tick 时序严格排序**：所有 Behavior 按 `TICK_DEFINE.TICK_TYPE_LIST` 固定顺序执行
- **快照/恢复**：`Stage.Snapshot()` / `Stage.Restore()` 支持状态回滚

### 1.3 表现分离

逻辑层与表现层严格隔离：

```
Logic Layer (确定性、定点数)          Render Layer (表现、浮点数)
┌─────────────────────────┐         ┌──────────────────────────┐
│ Stage → Behaviors → RIL │  ────→  │ RIL Consumer → Godot UI │
│     (int/FP only)       │  翻译层  │    (float/Vector3)       │
└─────────────────────────┘         └──────────────────────────┘
```

- `RIL`（Render Instruction Layer）：渲染指令层，逻辑层产出的最小化表现数据
- `Translator`：将 `BehaviorInfo` 翻译为 `RIL`，通过 **hashcode 比对**实现增量同步
- 表现层**绝不写回逻辑层数据**

---

## 二、框架边界

### 2.1 分层架构

```
┌──────────────────────────────────────────────────┐
│                  Director                         │
│  GameplayDirector → LocalDirector                 │
│  生命周期：Initialize → Run → Destroy              │
├──────────────────────────────────────────────────┤
│                  Logic Layer                       │
│  ┌─────────┐  ┌──────────┐  ┌─────────────────┐  │
│  │  Stage   │  │Commands  │  │   Translators   │  │
│  │ (主循环)  │  │(GM/Input)│  │ (Info → RIL)    │  │
│  ├─────────┤  └──────────┘  └─────────────────┘  │
│  │Behaviors│                                     │
│  │  Sa:    │  Flow, Detection, AttributeBucket    │
│  │         │  Magic, Buff, RILSync, Eventor ...   │
│  │ Actor:  │  Gamepad, Movement, StateMachine    │
│  │         │  SkillLauncher, Facade, Tag ...      │
│  └─────────┘                                     │
├──────────────────────────────────────────────────┤
│                  Config Layer                      │
│  Tables (怪物/技能/管线/动画...)                    │
├──────────────────────────────────────────────────┤
│                  Render Layer (Godot)              │
│  消费 RIL → 更新 Transform/Animation/VFX/UI        │
└──────────────────────────────────────────────────┘
```

### 2.2 模块职责边界

| 模块 | 职责 | 不负责 |
|------|------|--------|
| **Stage** | Actor 生命周期、Tick 调度、快照/恢复 | 具体游戏逻辑 |
| **Behavior** | 单一职责的游戏逻辑（移动/状态/输入...） | 跨 System 的编排 |
| **BehaviorInfo** | 纯数据存储，可序列化/克隆 | 任何逻辑 |
| **Flow** | 时间线驱动的指令管线 | 指令的"含义"由 Executor 决定 |
| **Prefab** | Actor 的工厂方法（装配 Behavior 组合） | 运行时逻辑 |
| **RILSync** | BehaviorInfo → RIL 翻译调度 | RIL 的具体含义 |
| **Config** | 配置表定位（不可变引用） | 数据修改 |
| **Commands** | 外部输入/GM 指令 | 游戏内逻辑 |

### 2.3 不在框架内的部分

- **AI / 行为树**：当前无 AI 系统，角色由玩家输入驱动（如有 PvE 需求需扩展）
- **寻路 / Navigation**：当前无寻路，移动由摇杆方向 + 速度属性直接驱动
- **UI / HUD**：在 Render Layer 实现，逻辑层仅通过 RIL_EVENT 传递结果
- **物理模拟**：不使用 Godot 物理引擎，碰撞检测通过自研 `Detection`（SAT 算法）
- **音效**：Sound 模块未实现（TODO）

---

## 三、核心子系统

### 3.1 Stage 主循环

```
Stage.Tick(deltaTime)
  │
  ├─ 按 TICK_TYPE_LIST 顺序 Tick 所有 Behavior
  │    Detection → Herald → Gamepad → Movement → StateMachine
  │    → Flow → HitEffect → SkillLauncher → Magic → Buff
  │    → SilentMercy → Facade → StepEnd → RILSync → StageSequence
  │
  ├─ 所有 Behavior.EndTick()
  │    → Recycle() 清理待删除的 Actor/Behavior/BehaviorInfo
  │
  └─ RILSync.Translate() → 发送 RIL 给表现层
```

`StepEnd` 注意：它在 `OnTick` 中保存所有 `SpatialInfo` 的上一帧快照，确保表现层可以做插值。

### 3.2 Actor 体系

```
Stage.GenActor()
  → actorid = info.increment++          // 自增 ID
  → Send ActorBornEvent()
  → Prefab.Processing(actorid, prefabid)  // 工厂装配
      → 添加 Behavior + BehaviorInfo 组合
  → Send RIL_DIFF_ACTOR(DIFF_NEW)
```

```
Stage.RmvActor(actorid)
  → Send ActorRmvEvent()
  → Disassemble 所有 Behavior
  → Send RIL_DIFF_ACTOR(DIFF_DEL)
  → 资源延迟回收（pending 队列，等待 Magic 引用解除）
```

**Sa 级 Actor**：`Stage` 自身是一个特殊 Actor（`sa = ulong.MaxValue`），持有所有全局 Behavior：
```csharp
stage.attrb      → AttributeBucket   // 属性系统
stage.flow       → Flow              // 管线流
stage.detection  → Detection         // 碰撞检测
stage.eventor    → Eventor           // 事件系统
stage.buff       → Buff              // Buff 管理
stage.magic      → Magic             // 魔法体管理
stage.rilsync    → RILSync           // 渲染同步
stage.herald     → Herald            // 指令传令官
stage.silentmercy → SilentMercy      // 生死管理
stage.seat       → Seat              // 座位
```

### 3.3 状态机

状态定义（`STATE_DEFINE.cs`）：

| 状态 | 含义 | 可切换到 |
|------|------|----------|
| `BORN` (1) | 出生（不能切出，由预制自动切 IDLE） | — |
| `DEATH` (2) | 死亡（终态，不可逆） | — |
| `IDLE` (3) | 待机 | DEATH, MOVE, FALL, CASTING, BEHIT, ROLL |
| `MOVE` (4) | 移动 | DEATH, IDLE, FALL, CASTING, BEHIT, ROLL |
| `JUMP` (5) | 跳跃 | DEATH, FALL, CASTING, BEHIT |
| `FALL` (6) | 下落 | DEATH, IDLE, CASTING, BEHIT |
| `CASTING` (7) | 施法（只能被击杀或受击打断） | DEATH, BEHIT |
| `BEHIT` (8) | 受击（可连续受击） | DEATH, BEHIT |
| `ROLL` (9) | 翻滚（无敌帧，不可被受击打断） | DEATH, IDLE, MOVE, CASTING |

关键设计：
- **PASSES 表**严格控制合法跳转，非表内跳转被拒绝
- `NONE` 状态不在 PASSES 中，只能通过 `force=true` 的 `ChangeState` 或 `Break()` 操作
- `DEATH` 是终态，进入后无法切出

### 3.4 管线流系统（Flow）

这是 Goblin 的核心机制，驱动所有技能、动画和伤害结算。

#### 架构

```
Flow (Behavior<FlowInfo>)
  ├── checkers: Dictionary<ushort, Checker>    // 条件检查器注册表
  ├── executors: Dictionary<ushort, Executor>   // 指令执行器注册表
  └── sparkindex                                  // 火花指令倒排索引
```

#### 管线生存周期

```
GenPipeline(actor, pipelineid)
  → PipelineData 从配置加载
  → FlowInfo.pipelines[id] = new Pipeline
  → 管线开始执行

RunPipeline(actor, flowinfo)
  → 遍历 pipelines[].instructs
  → 每个 Instruct 在指定时间区间进入 Enter/Execute/Exit
  → 条件检查（Checker.Check）决定是否执行
  → 所有 Instruct 完成后管线标记结束
```

#### Checker 扩展

```csharp
// 1. 定义条件数据
class XxxCondition : Condition { public ushort id => CONDITION_DEFINE.XXX; }
// 2. 实现检查器
class XxxChecker : Checker<XxxCondition> {
    protected override bool OnCheck(XxxCondition condition, FlowInfo info, ulong target) { ... }
}
// 3. 注册
Checker<XxxChecker>(CONDITION_DEFINE.XXX);
```

#### Executor 扩展（三阶段）

```csharp
// 1. 定义指令数据
class XxxData : InstructData { public ushort id => INSTR_DEFINE.XXX; }
// 2. 实现执行器
class XxxExecutor : Executor<XxxData> {
    protected override void OnEnter(...) { }  // 进入时触发一次
    protected override void OnExecute(...) { } // 每帧持续触发
    protected override void OnExit(...) { }    // 退出时触发一次
}
// 3. 注册
Executor<XxxExecutor>(INSTR_DEFINE.XXX);
```

#### 执行目标策略

| `data.et` | 含义 |
|-----------|------|
| `ET_FLOW` | 管线自身 Actor |
| `ET_FLOW_OWNER` | 管线拥有者（施法者） |
| `ET_FLOW_HIT` | 碰撞命中的所有目标（逐个条件检查） |

#### 火花（Spark）机制

基于事件触发的指令，独立于时间线：

```
Flow.Spark(actor, TOKEN_PIPELINE_GEN)
  → 查找所有活跃管线中匹配 token 的 SparkInstruct
  → 按 influence 过滤目标
  → Enter → Execute → Exit 一次性执行
```

典型使用：技能流水线生成的子弹命中时，触发伤害火花。

### 3.5 数值系统（AttributeBucket）

```
AttributeBucket (Sa 级 Behavior)
  └── attributes: Dictionary<ulong, Dictionary<ushort, int>>
        actor → (attrkey → value)

编码规则：
  mainkey  = key * 2 + 1    → 基础值
  scalekey = key * 2 + 2    → 千分比缩放（1000 = 100%）

最终值 = clamp(value * scale / 1000, 0, int.MaxValue)
```

伤害结算路径：
```
ChargeDamage(from, strength)
  → 基础伤害 = strength * ATTACK 属性值
  → 返回 DamageInfo { crit, value }

ToDamage(from, to, damage)
  → 检查目标是否已死亡
  → DischargeDamage(to, damage)   // 抗性计算
  → HP -= damage.value            // 夹在 [0, MAXHP]
  → 发送 RIL_EVENT_DAMAGE
  → HP <= 0 → silentmercy.Kill(from, to)
```

### 3.6 渲染指令层（RIL）

```
IRIL (基类：actor + hashcode)
  ├── IRIL_DIFF   → 差异指令（Actor 新建/删除）
  └── IRIL_EVENT  → 事件指令（伤害/治疗/结果）

RILSync.Translate() 流程：
  1. Parallel.ForEach 遍历所有 BehaviorInfo 类型
  2. Translator 计算当前 hashcode vs 缓存 hashcode
  3. 仅差异时生成 RIL ➜ 加入发送队列
  4. ProcessDiffQueue() → 处理 Actor 新建/删除
  5. ProcessEventQueue() → 处理伤害/治疗等一次性事件
  6. CleanupLostRILs() → 清理已销毁 Actor 的残留
```

RIL 类型（`RIL_DEFINE`）：`STAGE, TICKER, SEAT, TAG, SPATIAL, STATE_MACHINE, ATTRIBUTE, ACTOR, MOTION, FACADE_MODEL, FACADE_ANIMATION, FACADE_EFFECT`

### 3.7 碰撞检测（Detection）

自研 SAT（分离轴定理）碰撞系统，不依赖 Godot 物理引擎：

- 形状：Box 和 Sphere
- 算法：Box vs Box / Box vs Sphere / Sphere vs Sphere Overlap
- 射线：Raycast / Linecast
- AABB：快速包围盒预筛选
- 线程安全：请求队列模式

### 3.8 其他子系统

| 子系统 | 职责 |
|--------|------|
| **Buff** | 添加/移除/设置 Buff，层数、生命周期、属性增幅（Enchant），每 Tick 倒计时 |
| **Eventor** | 泛型事件系统：`Listen<T>/UnListen<T>/Tell<T>`，按注册顺序排序回调 |
| **Magic** | 魔法体管理：每 Tick 检查关联管线结束 → 自动移除 Magic Actor |
| **Herald** | 从全局 Command 队列接收指令，分发给 Solider 执行 |
| **Random** | LCG 确定性随机器，支持 int 和 FP 范围随机 |
| **Seat** | Actor 与座位的双向映射，跟随/脱离 |
| **SilentMercy** | 出生/死亡/击杀关系管理 |
| **HitEffect** | 顿帧（HitLag）效果：叠加强度和时间缩放 |
| **StageSequence** | 胜负条件监控，派发关卡结果事件 |

---

## 四、数据流概览

```
输入（Gamepad/MoveFrame）
  │
  ▼
Movement.OnTick()           // 读输入 → 移动角色
  │
  ▼
Flow.RunPipeline()          // 管线驱动
  ├── ChangeStateExecutor   // 状态切换（ROLL → delaybreak）
  ├── AnimationExecutor     // 播放动画
  ├── CreateMagicExecutor   // 生成魔法 Actor
  ├── LaunchSkillExecutor   // 发射技能管线
  ├── CollisionExecutor     // 碰撞检测 → 火花触发
  ├── DamageExecutor        // 伤害结算 → AttributeBucket
  └── ...
  │
  ▼
AttributeBucket             // 属性变更
  → HP 归零 → SilentMercy.Kill → ChangeState(DEATH)
  │
  ▼
RILSync.Translate()         // Info → RIL（增量同步）
  │
  ▼
Render Layer (Godot)        // 消费 RIL → 更新表现
```

---

## 五、关键设计决策

| 决策 | 理由 |
|------|------|
| 逻辑层仅用定点数 | 帧同步确定性要求 |
| 碰撞检测自研 | 避免浮点不确定性 + 精简物理开销 |
| 管线指令时间线驱动 | 技能/动画需要精确帧级编排 |
| Hash 增量 RIL 同步 | 减少表现层数据传输量 |
| Sa 级全局 Behavior | 跨 Actor 共享逻辑集中管理，避免 Godot 的 Global/Singleton 模式 |
| Actor 延迟回收 | 等待 Magic 引用解除，避免悬垂引用 |
| PASSES 表控制状态 | 有限状态机，防止非法跳转导致逻辑错误 |
