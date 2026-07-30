# Goblin 架构文档

> 状态：`Current`
>
> 更新日期：2026-07-31 | 基于源码分析。本文件是当前实现的唯一架构权威；其他设计、计划和审计文档中的旧术语或目标架构不代表现状。

---

## 一、设计理念

### 1.1 ECS-like 架构

Goblin 采用**类 ECS** 模式，但做了显著的 C# 化改造，使其更适合动作游戏的开发：

| 概念 | ECS 对应 | Goblin 实现 |
|------|----------|-------------|
| **Actor** | Entity | `ulong` 自增 ID，轻量实体句柄 |
| **Behavior** | System | 拥有 `Assemble → Tick → EndTick → Disassemble` 生命周期 |
| **BehaviorInfo** | Component | 纯数据容器，支持对象池 (ObjectCache)、克隆、快照/恢复 |

与正统 ECS 的关键差异：
- **Behavior 直接持有逻辑**，而非纯函数式的 Job System
- **BehaviorInfo 通过泛型自动绑定**到对应 Behavior，`Behavior<T>` 自动添加绑定 Info
- **SG（Source Generator）生成** `SGReady`/`SGReset`/`SGClone`，接管容器字段生命周期

### 1.2 确定性优先

作为帧同步格斗游戏，**所有逻辑层运算必须是确定性的**：

- 逻辑层**仅使用定点数 `FP`（Fixed Point）**，禁止 `float`/`double`
- 自研定点数学库：`FPVector2/3/4`、`FPQuaternion`、`FPMatrix`、`AABB`、`Box`、`Sphere`
- 随机数使用 **LCG（线性同余生成器）**，可通过种子重放
- **Tick 时序严格排序**：所有 Behavior 按 `TICK_DEFINE.TICK_TYPE_LIST` 固定顺序执行
- **快照/恢复**：`Stage.Snapshot()` / `Stage.Restore()` 支持状态回滚，回滚后标记全部投影字段为脏

### 1.3 表现分离

逻辑层与表现层通过 **Projection 投影系统** 严格隔离：

```
Logic Layer (确定性、定点数)          Render Layer (表现、浮点数)
┌──────────────────────────┐         ┌──────────────────────────┐
│ BehaviorInfo.属性 setter  │         │                          │
│   → projectdirtymask 位标记 │        │                          │
│                          │         │                          │
│ ProjectorSystem.OnEndTick│  Projection  │ Canvas.ApplyPackets       │
│   → 自检脏mask            │  Pipeline │   → Shadow.ApplyTo()     │
│   → ProjectorPacket[]    │ ────────→│     (Spatial/Facade/HUD) │
│                          │         │                          │
└──────────────────────────┘         └──────────────────────────┘
```

- 含 `[Projector]` 特性的 BehaviorInfo 属性 setter 由 SG 自动写入 `projectdirtymask` 位标记
- `ProjectorSystem` 帧末自检所有 `IProjectable` 实例 → 打包 `ProjectorPacket[]`
- `ProjectionPipeline` 按 Observer 裁剪（AOI/频率/权限/可见性）→ `ObserverPacket[]`
- `Canvas` 消费投影数据，调用对应 `Shadow.ApplyTo()` 更新表现数据
- Render 层 View 读取 Shadow 并驱动 Godot 节点，**绝不写回逻辑层数据**

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
│  │  Stage   │  │Commands  │  │   Projection    │  │
│  │ (主循环)  │  │(GM/Input)│  │ (投影同步)       │  │
│  ├─────────┤  └──────────┘  └─────────────────┘  │
│  │Behaviors│                                     │
│  │  Sa:    │  Flow, Detection, AttributeBucket    │
│  │         │  Magic, Buff, Eventor, Herald        │
│  │         │  ProjectorSystem, StepEnd ...        │
│  │ Actor:  │  Gamepad, Movement, StateMachine    │
│  │         │  SkillLauncher, Facade, Tag, HUD     │
│  └─────────┘                                     │
├──────────────────────────────────────────────────┤
│                  Projection Layer                  │
│  ProjectorPacket → Crop Rules → ObserverPacket    │
│  Transport (Local / Network)                       │
│  Canvas → Shadows (Spatial / Facade / HUD)        │
├──────────────────────────────────────────────────┤
│                  Render Layer (Godot)              │
│  View → 读取 Shadow，更新 Transform/Animation/VFX/UI│
│  Projection 层本身不依赖 Godot API                 │
└──────────────────────────────────────────────────┘
```

### 2.2 模块职责边界

| 模块 | 职责 | 不负责 |
|------|------|--------|
| **Stage** | Actor 生命周期、Tick 调度、快照/恢复 | 具体游戏逻辑 |
| **Behavior** | 单一职责的游戏逻辑（移动/状态/输入...） | 跨 System 的编排 |
| **BehaviorInfo** | 纯数据存储，可序列化/克隆，SG 生成生命周期 | 任何逻辑 |
| **Flow** | 时间线驱动的指令管线（Checkers + Executors） | 指令的"含义"由 Executor 决定 |
| **Prefab** | Actor 的工厂方法（装配 Behavior + BehaviorInfo 组合） | 运行时逻辑 |
| **ProjectorSystem** | 帧末自检脏 BehaviorInfo → 打包 ProjectorPacket[] | 裁剪、网络传输 |
| **ProjectionPipeline** | 裁剪规则链 + Transport 分发 | 数据收集 |
| **Canvas** | Projection 侧数据画布，ActorID → Shadow 映射，零反射消费 | 表现逻辑、Godot 节点操作 |
| **Config** | 配置表定位（不可变引用） | 数据修改 |
| **Commands** | 外部输入/GM 指令 | 游戏内逻辑 |

### 2.3 不在框架内的部分

- **AI / 行为树**：当前无 AI 系统，角色由玩家输入驱动（如有 PvE 需求需扩展）
- **寻路 / Navigation**：当前无寻路，移动由摇杆方向 + 速度属性直接驱动
- **UI / HUD**：在 Render Layer 实现，逻辑层 HUD Behavior 产生数据，通过投影同步
- **物理模拟**：不使用 Godot 物理引擎，碰撞检测通过自研 `Detection`（SAT 算法）
- **音效**：`SoundExecutor` 存在于 Flow 管线中，具体 Sound 播放模块待完善

---

## 三、核心子系统

### 3.1 Stage 主循环

```
Stage.Step()
  │
  ├─ 按 TICK_TYPE_LIST 顺序 Tick 所有 Behavior
  │    Detection → Herald → Gamepad → Movement → StateMachine
  │    → Flow → HitEffect → SkillLauncher → Magic → Buff
  │    → SilentMercy → Facade → StageSequence → HUD
  │    → StepEnd → ProjectorSystem
  │
  ├─ 所有 Behavior.EndTick()
  │    → ProjectorSystem.OnEndTick() 自检脏 Info → 出包
  │    → 其他 Behavior EndTick 处理延迟回收等
  │
  └─ Recycle() 清理待删除的 Actor/Behavior/BehaviorInfo
```

`StepEnd`：在 `OnTick` 中保存所有 `SpatialInfo` 的上一帧快照，确保表现层可以做插值。

`ProjectorSystem`：**排在最后**，在所有 Behavior 修改完 BehaviorInfo 后自检脏标记出包。

### 3.2 Actor 体系

```
Stage.GenActor()
  → actor = info.increment++              // 自增 ID
  → eventor.Tell(ActorBornEvent)          // 广播出生事件
  → Prefab.Processing(actor, prefabinfo)  // 工厂装配
      → 添加 Behavior + BehaviorInfo 组合
```

```
Stage.RmvActor(actor)
  → ActorToRecycle(actor) → 加入 rmvactorset
  → eventor.Tell(ActorRmvEvent)
  → 帧末 Recycle() 统一 Disassemble + 回池
```

**延迟回收**：`AttributeBucket.OnEndTick()` 检查 pending actor 是否还被 Magic 引用，未解除则推迟到下一帧回收。

**Sa 级 Actor**：`Stage` 自身是一个特殊 Actor（`sa = ulong.MaxValue`），持有所有全局 Behavior：
```csharp
stage.cfg          → Config              // 配置
stage.eventor      → Eventor             // 事件系统
stage.seat         → Seat                // 座位
stage.random       → Random              // 随机数
stage.attrb        → AttributeBucket     // 属性系统
stage.detection    → Detection           // 碰撞检测
stage.herald       → Herald              // 指令传令官
stage.flow         → Flow                // 管线流
stage.hiteffect    → HitEffect           // 打击效果
stage.buff         → Buff                // Buff 管理
stage.silentmercy  → SilentMercy         // 生死管理
stage.projector    → ProjectorSystem     // 投影系统（取代旧 RILSync）
```

### 3.3 状态机

状态定义（`STATE_DEFINE.cs`）：

| 状态 | 值 | 含义 | 可切换到 |
|------|-----|------|----------|
| `NONE` | 0 | 空状态（不在 PASSES 中，仅 force 跳转） | — |
| `BORN` | 1 | 出生（不能切出，由预制自动切 IDLE） | — |
| `DEATH` | 2 | 死亡（终态，不可逆） | — |
| `IDLE` | 3 | 待机 | DEATH, MOVE, FALL, CASTING, HITSTUN, ROLL |
| `MOVE` | 4 | 移动 | DEATH, IDLE, FALL, CASTING, HITSTUN, ROLL |
| `JUMP` | 5 | 跳跃 | DEATH, FALL, CASTING, HITSTUN |
| `FALL` | 6 | 下坠 | DEATH, IDLE, CASTING, HITSTUN |
| `CASTING` | 7 | 技能 | DEATH, HITSTUN |
| `HITSTUN` | 8 | 硬直（可连续受击） | DEATH, HITSTUN |
| `ROLL` | 9 | 翻滚（无敌帧，不可被受击打断） | DEATH, IDLE, MOVE, CASTING |

关键设计：
- **PASSES 表**严格控制合法跳转，非表内跳转被拒绝
- `NONE` 状态不在 PASSES 中，只能通过 `force=true` 的 `ChangeState` 或 `Break()` 操作
- `DEATH` 是终态，进入后无法切出
- `BORN` 的 PASSES 为空数组，意味着不能从 BORN 自动切出（由 Prefab 逻辑手动切到 IDLE）

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
  → PipelineData 从配置加载（通过 PipelineDataReader）
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
// 1. 定义条件数据 (继承 Condition)
class XxxCondition : Condition { public ushort id => CONDITION_DEFINE.XXX; }
// 2. 实现检查器 (继承 Checker<T>)
class XxxChecker : Checker<XxxCondition> {
    protected override bool OnCheck(XxxCondition condition, FlowInfo info, ulong target) { ... }
}
// 3. 在 Flow.OnAssemble 中注册
AddChecker<XxxChecker>(CONDITION_DEFINE.XXX);
```

#### Executor 扩展（三阶段）

```csharp
// 1. 定义指令数据 (继承 InstructData)
class XxxData : InstructData { public ushort id => INSTR_DEFINE.XXX; }
// 2. 实现执行器 (继承 Executor<T>)
class XxxExecutor : Executor<XxxData> {
    protected override void OnEnter(...) { }   // 进入时触发一次
    protected override void OnExecute(...) { } // 每帧持续触发
    protected override void OnExit(...) { }    // 退出时触发一次
}
// 3. 在 Flow.OnAssemble 中注册
AddExecutor<XxxExecutor>(INSTR_DEFINE.XXX);
```

#### 执行目标策略（`data.et`）

| 值 | 含义 |
|------|------|
| `ET_FLOW` | 管线自身 Actor |
| `ET_FLOW_OWNER` | 管线拥有者（施法者） |
| `ET_FLOW_HIT` | 碰撞命中的所有目标（逐个条件检查） |

#### 当前 Executor 清单

| Executor | 职责 |
|----------|------|
| `AnimationExecutor` | 播放动画 |
| `BeHitExecutor` | 受击处理 |
| `ChangeStateExecutor` | 状态切换 |
| `CollisionExecutor` | 碰撞检测 → 火花触发 |
| `CreateMagicExecutor` | 生成魔法 Actor |
| `DamageExecutor` | 伤害结算 → AttributeBucket |
| `EffectExecutor` | 特效播放 |
| `HitLagExecutor` | 顿帧效果 |
| `LaunchSkillExecutor` | 发射技能管线 |
| `RmvActorExecutor` | 移除 Actor |
| `SkillBreakExecutor` | 技能打断 |
| `SoundExecutor` | 音效播放 |
| `SparkExecutor` | 火花一次性执行 |
| `SpatialPositionExecutor` | 空间位置修改 |
| `TimeScaleExecutor` | 时间缩放 |

#### 火花（Spark）机制

基于事件触发的指令，独立于时间线：

```
Flow.Spark(actor, TOKEN_PIPELINE_GEN)
  → 查找所有活跃管线中匹配 token 的 SparkInstruct
  → 按 influence 过滤目标
  → Enter → Execute → Exit 一次性执行
```

典型使用：技能流水线生成的子弹命中时，触发伤害火花。

#### 管线脚本（Scriptings）

`Scriptings/` 目录下 `S10000/S10001/S10010/S10020` 等为管线脚本类，继承 `Scripting`，通过 `ScriptMachine` 驱动。将复杂管线逻辑从数据配置中剥离为可编程脚本。

### 3.5 数值系统（AttributeBucket）

```
AttributeBucket (Sa 级 Behavior)
  └── attributes: Dictionary<ulong, GBLDict<ushort, int>>
        actor → (attrkey → value)

编码规则：
  mainkey  = key * 2 + 1    → 基础值
  scalekey = key * 2 + 2    → 千分比缩放（1000 = 100%）

最终值 = clamp(value * scale / 1000, 0, int.MaxValue)
```

属性清单（`ATTRIBUTE_DEFINE`）：
- `HP` / `MAXHP` — 生命值当前/最大
- `MOVESPEED` — 移动速度
- `ATTACK` — 攻击力
- `ARMOR` — 护甲（固定值减伤）
- `MAGIC_RESIST` — 魔法抗性（固定值魔伤减免）
- `CRIT_RATE` — 暴击率（千分比，如 500 = 50%）
- `DODGE_RATE` — 闪避率（千分比，如 200 = 20%）

伤害结算路径：
```
ChargeDamage(from, strength)
  → 暴击判定 (CRIT_RATE)
  → 基础伤害 = strength * ATTACK 属性值
  → 返回 DamageInfo { crit, magic, value }

ToDamage(from, to, damage)
  → 检查目标是否已死亡
  → DischargeDamage(to, damage)   // 闪避判定 + 抗性计算
  → HP -= damage.value            // 夹在 [0, MAXHP]
  → HP <= 0 → silentmercy.Kill(from, to)
```

### 3.6 投影系统（Projection）

这是 2026-07 替换旧 RIL 系统的新同步架构。

#### 数据产生端（Logic 层）

```
[Projector] 特性  →  SG 生成 backing field + 脏标记
IProjectable 接口 →  SG 生成 IProjectable 实现（含 projectdirtymask, TakeProjectValues, SetProjectValues, MarkAllDirty）
ProjectorSystem  →  BehaviorInfo 属性变更 → projectdirtymask 位标记
                   OnEndTick 自检所有 IProjectable → ProjectorPacket[]
```

关键实现细节：
- **懒初始化**：无脏数据时 `ProjectorSystem.packets = Array.Empty`，零分配
- **零分配快路径**：99% 的 BehaviorInfo 帧末 `projectdirtymask == 0`，直接跳过
- **位图粒度**：`projectdirtymask` 是 `ulong`（64 位），每位对应一个 `[Projector(index)]` 字段

#### 裁剪层（ProjectionPipeline）

```
ProjectorPacket[]
  → Crop.Process(packets, observers)
    → 每条 ProjectorPacket × 每个 Observer
    → Observer.crop 规则链逐条裁剪 fieldmask
    → mask == 0 时丢弃
  → ObserverPacket[]
  → Transport.Send()
```

**5 种裁剪规则**：

| 规则 | 职责 | Fail-Open |
|------|------|-----------|
| `GodRule` | 全通过（Phase 1 默认） | — |
| `AOIRule` | 距离裁剪，超出 Observer.radius 返回 0 | 位置不可用时放行 |
| `FrequencyRule` | 按字段独立推送间隔，抑制高频推送 | 未注册间隔的字段放行 |
| `PermissionRule` | 按 (ObserverType, InfoType) 查表，允许部分字段 | 未注册权限的组合放行 |
| `VisibilityRule` | 不可见实体裁剪 | Canvas 无数据时放行（首帧同步） |

**Fail-Open 设计哲学**：裁剪规则在数据不足时（位置不可查、权限未注册、Canvas 无数据）默放行，避免因漏注册导致黑屏/数据断流。

#### 传输层（Transport）

| Transport | 适用场景 |
|-----------|----------|
| `LocalTransport` | 帧同步/单机模式，直接写入 Canvas（不序列化） |
| `NetworkTransport` | 网络模式，序列化 ObserverPacket 后走网络 |

#### 数据消费端（Projection 层）

```
Canvas（数据画布）
  ├── datas: ActorID → (ShadowType → Shadow)
  ├── infotoshadow: BehaviorInfo 类型 → Shadow 类型映射
  ├── applymap: Shadow 类型 → ApplyTo 静态委托（零反射）
  └── factorymap: Shadow 类型 → 工厂委托（对象池创建）

Canvas.ApplyPackets(ObserverPacket[])
  → 对每条 ObserverPacket
    → 查询 infotoshadow 获取 Shadow 类型
    → 工厂创建或复用已有 Shadow
    → 调用 Shadow.ApplyTo(values, fieldmask)
```

`Canvas` 是纯 C# 数据副本；`Shadow` 是被动数据容器；Render 层的 View 读取 Shadow 并驱动 Godot 节点。

**Shadow 清单**：

| Shadow | 映射源 Info | 数据 |
|--------|------------|------|
| `SpatialShadow` | `SpatialInfo` | position, euler, scale |
| `FacadeShadow` | `FacadeInfo` | model, anim*, effect* |
| `HUDShadow` | `HUDInfo` | hp, maxhp, movespeed, attack |

**注册方式**：
```csharp
canvas.Register<SpatialInfo, SpatialShadow>();
```

`[Projector]` 描述 Logic 字段；Source Generator 生成 Shadow、映射和 `ApplyTo` 消费代码。

#### Observer 类型

```csharp
enum ObserverType { GM, Editor, Replay, Player, Spectator, AI }
```

每个 Observer 有独立的裁剪规则链，不同角色看到的数据可以不同（如敌方只看到位置，队友看到全部）。

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
| **Eventor** | 泛型事件系统：`Listen<T>/UnListen<T>/Tell<T>`，按注册顺序排序回调，支持 `SaEventor`（Sa 级专用） |
| **Magic** | 魔法体管理：每 Tick 检查关联管线结束 → 自动移除 Magic Actor |
| **Herald** | 从全局 Command 队列接收指令，分发给 Solider 执行（GM/TimeScale 等） |
| **Random** | LCG 确定性随机器，支持 int 和 FP 范围随机 |
| **Seat** | Actor 与座位的双向映射，跟随/脱离 |
| **SilentMercy** | 出生/死亡/击杀关系管理 |
| **HitEffect** | 顿帧（HitLag）效果：叠加强度和时间缩放 |
| **StageSequence** | 胜负条件监控，派发关卡结果事件 |
| **Config** | Sa 级配置管理 Behavior |
| **HUD** | Actor 级 Behavior，收集 HP/ATK 等属性写入 HUDInfo 供投影同步 |
| **Facade** | 外观收集，将动画/模型/特效状态写入 FacadeInfo |
| **Ticker** | 每个 Actor 独立的 timescale，`Behavior.Tick(tick * ticker.timescale)` |
| **ObjectCache** | 泛型对象池，`Ensure<T>()` / `Set(T)`，配合 IGBL.Reset/Clone 管理生命周期 |
| **Coroutine** | 并行指令系统，支持 `WaitForFrames` / `WaitForSeconds` |

---

## 四、数据流概览

```
输入（Gamepad/MoveFrame/KeyFrame/SkillFrame）
  │
  ▼
Movement.OnTick()           // 读输入 → 移动角色
  │
  ▼
StateMachine.OnTick()       // 状态机更新
  │
  ▼
Flow.RunPipeline()          // 管线驱动
  ├── AnimationExecutor     // 播放动画
  ├── CollisionExecutor     // 碰撞检测 → Spark(token) 触发
  ├── CreateMagicExecutor   // 生成魔法 Actor
  ├── LaunchSkillExecutor   // 发射技能管线
  ├── DamageExecutor        // 伤害结算 → AttributeBucket
  ├── ChangeStateExecutor   // 状态切换
  └── ...
  │
  ▼
AttributeBucket             // 属性变更
  → HP 归零 → SilentMercy.Kill → ChangeState(DEATH)
  │
  ▼
Facade / HUD .OnTick()     // 收集表现数据写入 Info
  │
  ▼
StepEnd.OnTick()            // 保存 SpatialInfo 上一帧快照
  │
  ▼
ProjectorSystem.OnEndTick() // 自检脏 Info → ProjectorPacket[]
  │
  ▼
ProjectionPipeline.Process()
  → Crop Rules 裁剪 (AOI/Freq/Perm/Vis)
  → Transport.Send()
  │
  ▼
Canvas.ApplyPackets()        // → 更新 Shadow 数据
  │
  ▼
Render Layer (Godot)        // View 消费 Shadow → 更新 Transform/Animation/VFX/UI
```

---

## 五、关键设计决策

| 决策 | 理由 |
|------|------|
| 逻辑层仅用定点数 | 帧同步确定性要求 |
| 碰撞检测自研 | 避免浮点不确定性 + 精简物理开销 |
| 管线指令时间线驱动 | 技能/动画需要精确帧级编排 |
| Projection 脏标记位图同步 | 粗粒度标记（ulong）+ 帧末批量自检，单帧零分配快路径 |
| 来源生成器（SG）生成 IProjectable | 减少手写样板，setter 自动写位标记 |
| 裁剪规则 Fail-Open | 数据不足时放行，避免因漏注册导致黑屏/断流 |
| Sa 级全局 Behavior | 跨 Actor 共享逻辑集中管理 |
| Actor 延迟回收（AttributeBucket.pendings） | 等待 Magic 引用解除，避免悬垂引用 |
| PASSES 表控制状态 | 有限状态机，防止非法跳转导致逻辑错误 |
| Canvas 零反射 Shadow 创建 | 工厂委托 + ApplyTo 静态委托，避免运行期反射开销 |

---

## 六、编码规范速查

- 属性全小写（`public ulong actor`），常量 SCREAMING_SNAKE_CASE
- `null == x` / `false == condition`，不用 `!` 取反
- 动词缩写：`Rmv`、`Gen`、`Seek`、`Tell`
- `SeekXxx` 返回 `bool`，数据走 `out` 参数
- 4 空格缩进，CRLF，UTF-8 BOM，文件级命名空间
- 中文注释，禁止行尾注释

---

## 七、相关文档

| 文档 | 内容 |
|------|------|
| [CODING_STYLE.md](CODING_STYLE.md) | 编码规范 |
| [RENDER_LAYER_DESIGN.md](RENDER_LAYER_DESIGN.md) | Render 层设计草案（Design，部分术语过时） |
| [ANIMATION_PROPOSAL.md](ANIMATION_PROPOSAL.md) | 动画槽位优先级系统 |
| [Projection/CORE.md](Projection/CORE.md) | Projection 设计哲学 |
| [Projection/PROPERTY_SYNC_DESIGN.md](Projection/PROPERTY_SYNC_DESIGN.md) | 属性同步完整设计 |
| [Projection/SNAPSHOT_RESTORE_DESIGN.md](Projection/SNAPSHOT_RESTORE_DESIGN.md) | 快照/恢复机制 |
| [Projection/IMPLEMENTATION_PLAN.md](Projection/IMPLEMENTATION_PLAN.md) | 实施计划与进度 |
| [Projection/GAMEPLAY_AUDIT.md](Projection/GAMEPLAY_AUDIT.md) | Gameplay 模块审计 |
| [Projection/PROJECTION_AUDIT.md](Projection/PROJECTION_AUDIT.md) | 投影系统审计 |
| [Projection/BEHAVIORINFO_LIFECYCLE_REPORT.md](Projection/BEHAVIORINFO_LIFECYCLE_REPORT.md) | BehaviorInfo 生命周期分析 |
