# 穿透式目标查找规则方案

> 2026-07-19 | 替代硬编码 `ET_` 常量，用语义目标枚举 + 引擎搜索，让指令自带天然目标

---

## 一、从代码看现状

### 1.1 当前 `byte et` 体系

`InstructData.et`（`InstructData.cs:20`）是一个字节，由 `Flow.ExecuteInstruct`（`Flow.cs:480-501`）的 switch-case 解析为 `ulong target`：

```csharp
public const byte ET_FLOW        = 1;  // flowinfo.actor
public const byte ET_FLOW_OWNER  = 2;  // flowinfo.owner（默认值）
public const byte ET_FLOW_HIT    = 3;  // foreach FlowCollisionHurtInfo.targets
public const byte ET_MAGIC_OWNER = 4;  // flowinfo.owner → 若是 Magic 则取 magic.owner
```

### 1.2 逐个执行器对 `target` 的真实用法

把 16 个执行器全读一遍，看它们拿 `target` 到底干了什么：

| 指令 | 执行器对 target 的操作 | target 实际是谁 |
|------|----------------------|----------------|
| `Animation` | `facade.SetAnimation(name)` 写动画 | **施法者**（有 Facade 的角色） |
| `SpatialPosition` | `spatial.position += motion` | **施法者/魔法体**（被移动的实体） |
| `CreateMagic` | 读 target 的 spatial 生成子 Magic | **施法者/魔法体**（生成原点） |
| `MagicMotion` | `spatial.position += forward*speed` | **魔法体**（弹道飞行） |
| `LaunchSkill` | `skilllauncher.Launch(skill)` | **施法者** |
| `Effect` | `facade.CreateEffect(...)` | **施法者**（特效挂点） |
| `Collision` | 用 target 的 spatial 摆碰撞盒 | **施法者**（从谁的位置检测） |
| `RmvActor` | `EndPipeline + RmvActor` | 通用（删谁就是谁） |
| `ChangeState` | `statemachine.ChangeState` | **施法者**（切 ROLL）或受击者（切 HIT） |
| `Spark` | **不用 target**，只用 flowinfo | 无关 |
| `HitLag` | `hiteffect.AddHitLag(target)` 冻结 | **受击者**（被冻结的人） |
| `TimeScale` | `ticker.timescale = ...` | **施法者** |
| `BeHit` | 受击朝向+位移 | **受击者** |
| `SkillBreak` | `skilllauncher.Break()` | **受击者**（打断对方的技能） |
| `Damage` | `attrb.ToDamage(from, target)` | **受击者** |
| `Sound` | 发声事件 `actor = target` | **施法者**（声源） |

**结论：脚本里满屏的 `et = ET_MAGIC_OWNER` / `et = ET_FLOW_HIT` 全是废话。** 碰撞天然作用于施法者，受击天然作用于命中者，声源天然是施法者——指令自己知道该找谁，根本不该让脚本手写。

### 1.3 三个代码里已经暴露的问题

**问题 A：`DamageExecutor` 不信 `et`，自己偷偷穿透一遍（`DamageExecutor.cs:28-30`）**

```csharp
var from = stage.SeekBehaviorInfo(flowinfo.owner, out MagicInfo magicinfo)
    ? magicinfo.owner
    : flowinfo.owner;
```

执行器在内部重新找施法者。说明 `et` 不可靠，执行器已经在绕过它。

**问题 B：`BeHitExecutor` 把 `flowinfo.owner` 当攻击者（`BeHitExecutor.cs:22`）**

```csharp
stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo atkspatial);
```

但 `flowinfo.owner` 是 Magic，不是英雄。受击朝向用的是 Magic 的位置——只是因为 Magic 恰好生成在英雄位置才没暴露。这是**潜在 bug**。

**问题 C：火花指令依赖 `targets.Clear()` 的时序（`CollisionExecutor.cs:87-92`）**

```csharp
stage.flow.Spark(flowinfo, data.spark.influence, data.spark.token);  // 同步执行
flowcollision.targets.Clear();                                        // 必须在 Spark 返回后
```

`ET_FLOW_HIT` 的火花指令依赖 targets 尚未清空。Spark 一旦改异步，静默失败。

### 1.4 根因

`ET_` 枚举命名的是**结构路径**（OWNER、FLOW_HIT），不是**具体实体**。它回答"怎么走"，而非"找谁"。导致：

1. 脚本必须手写 `et`，因为指令没有默认目标
2. `ET_MAGIC_OWNER` 只穿一层，换层级就废
3. 执行器不信 `et`，各自重新穿透（DamageExecutor 已如此）
4. 同一个 `et` 在不同上下文（主线 vs 火花）含义不一致

---

## 二、Actor 层级与具体实体

当前一次技能调用的 Actor 层级：

```
Hero/Enemy（施法者）
  │
  └── Magic Actor（魔法体，magic.owner = 施法者）
        │
        └── Flow Actor（管线，flowinfo.owner = Magic）
              │
              ├── Pipeline 指令执行
              ├── CollisionData → 碰撞检测
              └── FlowCollisionHurtInfo.targets（命中目标列表）
                    │
                    └── 被打中的 Hero/Enemy（命中目标）
```

这条链上**真正存在的具体实体只有四个**：

| 具体实体 | 在层级中的位置 | 代码里的来源 |
|---------|--------------|-------------|
| **施法者** | 链顶的角色 | Magic.owner → 穿透到 Hero |
| **魔法体** | Magic Actor | flowinfo.owner（当它是 Magic 时） |
| **命中目标** | 碰撞打中的人 | FlowCollisionHurtInfo.targets |
| **管线自身** | Flow Actor | flowinfo.actor |

这四个就是枚举的全部。不多不少，都是代码里真实存在的东西。

---

## 三、方案：语义目标枚举 + 引擎搜索 + 指令自带默认

### 3.1 核心思想

**三步转变：**

```
旧：脚本手写 et（怎么走）     "往上走两层"
新：指令自带 target（找谁）    "找施法者"
   引擎负责搜索（怎么找）     穿透 Magic/Buff/Projectile 直到角色
```

**关键：每个指令天生知道自己该作用于谁。** 脚本 90% 的场景不需要写目标，只在覆盖默认时才写。

### 3.2 语义目标枚举

```csharp
/// <summary>
/// 语义目标 — 声明"找谁"，引擎负责搜索
/// </summary>
public enum TargetType : byte
{
    /// <summary>
    /// 施法者：发起本次技能的角色（Hero/Enemy）
    /// 引擎从 flowinfo.owner 向上穿透 Magic/Buff/Projectile，找到第一个"角色"
    /// </summary>
    CASTER = 1,

    /// <summary>
    /// 魔法体：当前管线所属的 Magic Actor（若有）
    /// </summary>
    MAGIC = 2,

    /// <summary>
    /// 命中目标：碰撞检测打中的所有角色（多目标展开）
    /// </summary>
    HIT_TARGET = 3,

    /// <summary>
    /// 管线自身：flowinfo.actor
    /// </summary>
    SELF = 4,
}
```

### 3.3 引擎搜索规则

引擎拿到 `TargetType` 后，根据当前上下文搜索：

#### `CASTER` — 找施法者

```
输入: flowinfo
搜索:
  current = flowinfo.owner
  loop:
    if current 是 Magic          → current = magic.owner     // 穿透魔法体
    else if current 是 Buff      → current = buff.owner       // 穿透 Buff（未来）
    else if current 是 Projectile → current = projectile.owner // 穿透弹道（未来）
    else                          → 返回 current              // 到达角色，停止
  安全上限: 穿透不超过 4 层（防环）
```

**关键：无论中间隔几层、隔什么类型，都能找到角色。** 这是 `ET_MAGIC_OWNER` 做不到的——它只穿一层 Magic。

#### `MAGIC` — 找魔法体

```
输入: flowinfo
搜索:
  if flowinfo.owner 有 MagicInfo → 返回 flowinfo.owner
  else                            → 返回 0（当前管线不归属 Magic）
```

#### `HIT_TARGET` — 找命中目标

```
输入: flowinfo + 当前 spark 上下文
搜索:
  if 在 spark 触发链中 → 取 spark 携带的命中 actor（逐个展开）
  else                  → 取 FlowCollisionHurtInfo.targets（逐个展开）
```

**这解决了问题 C**：火花触发时，命中目标由 spark 上下文携带，不再依赖 `targets.Clear()` 时序。

#### `SELF` — 管线自身

```
返回 flowinfo.actor
```

### 3.4 指令自带默认目标

每个 `InstructData` 子类声明自己的**天然目标**。脚本不写就用默认：

| 指令 | 天然目标 | 理由 |
|------|---------|------|
| `AnimationData` | `CASTER` | 播放施法者动画 |
| `SpatialPositionData` | `CASTER` | 移动施法者 |
| `CreateMagicData` | `CASTER` | 在施法者位置生成 |
| `MagicMotionData` | `MAGIC` | 移动魔法体（弹道） |
| `LaunchSkillData` | `CASTER` | 施法者释放技能 |
| `EffectData` | `CASTER` | 特效挂在施法者身上 |
| `CollisionData` | `CASTER` | 从施法者位置检测碰撞 |
| `RmvActorData` | `SELF` | 默认移除管线自身 |
| `ChangeStateData` | `CASTER` | 切施法者状态 |
| `SparkData` | （不用 target） | 只用 flowinfo |
| `HitLagData` | `HIT_TARGET` | 冻结受击者 |
| `TimeScaleData` | `CASTER` | 缩放施法者时间 |
| `BeHitData` | `HIT_TARGET` | 受击者进入受击 |
| `SkillBreakData` | `HIT_TARGET` | 打断受击者技能 |
| `DamageData` | `HIT_TARGET` | 伤害受击者 |
| `SoundInstructData` | `CASTER` | 声源在施法者 |

代码实现：

```csharp
public abstract class InstructData
{
    public abstract ushort id { get; }

    /// <summary>
    /// 语义目标（找谁）。子类可覆盖声明天然目标，默认为施法者
    /// </summary>
    public virtual TargetType target => TargetType.CASTER;

    /// <summary>
    /// [兼容期] 旧版 et，target == 0 时回退到 et 逻辑
    /// </summary>
    public byte et = FLOW_DEFINE.ET_FLOW_OWNER;
}

// 受击类指令天然作用于命中目标
public class BeHitData : InstructData
{
    public override TargetType target => TargetType.HIT_TARGET;
    // ...
}

public class DamageData : InstructData
{
    public override TargetType target => TargetType.HIT_TARGET;
    // ...
}

// 魔法运动天然作用于魔法体
public class MagicMotionData : InstructData
{
    public override TargetType target => TargetType.MAGIC;
    // ...
}
```

### 3.5 脚本对比

**S10010 翻滚 — 现在：**

```csharp
// 三处都要手写 et = ET_MAGIC_OWNER
Instruct(0, 0, new ChangeStateData    { et = FLOW_DEFINE.ET_MAGIC_OWNER, state = STATE_DEFINE.ROLL, ... });
Instruct(0, 0, new SoundInstructData  { et = FLOW_DEFINE.ET_MAGIC_OWNER, soundid = 1000001 });
Instruct(0, 320, new SpatialPositionData { et = FLOW_DEFINE.ET_MAGIC_OWNER, position = ... });
```

**S10010 翻滚 — 之后：**

```csharp
// ChangeState/Sound/SpatialPosition 天然作用于 CASTER，不用写
Instruct(0, 0,   new ChangeStateData    { state = STATE_DEFINE.ROLL, ... });
Instruct(0, 0,   new SoundInstructData  { soundid = 1000001 });
Instruct(0, 320, new SpatialPositionData { position = ... });
```

**S10020 重击 — 现在：**

```csharp
Instruct(0, 0, new SoundInstructData { soundid = 1000002 });
Instruct(0, 600, new SpatialPositionData { type = ..., position = ... });
Instruct(200, 500, new CollisionData { et = FLOW_DEFINE.ET_MAGIC_OWNER, ... });   // 手写
Instruct(200, 200, new BeHitData     { et = FLOW_DEFINE.ET_FLOW_HIT, ... });      // 手写
Instruct(200, 200, new HitLagData    { et = FLOW_DEFINE.ET_FLOW_HIT, ... });      // 手写
ScriptMachine.Instruct(SPARK..., new DamageData { strength = 3000 });             // 构造函数写死 ET_FLOW_HIT
```

**S10020 重击 — 之后：**

```csharp
Instruct(0, 0,   new SoundInstructData  { soundid = 1000002 });
Instruct(0, 600,  new SpatialPositionData { type = ..., position = ... });
Instruct(200, 500, new CollisionData { ... });         // 天然 CASTER，不写
Instruct(200, 200, new BeHitData    { ... });           // 天然 HIT_TARGET，不写
Instruct(200, 200, new HitLagData   { ... });           // 天然 HIT_TARGET，不写
ScriptMachine.Instruct(SPARK..., new DamageData { strength = 3000 });  // 天然 HIT_TARGET，不写
```

**脚本里彻底看不到目标相关的代码了。** 只有需要覆盖默认时（比如 ChangeState 作用于受击者切 HIT 状态）才显式写：

```csharp
// 罕见：把受击者也切个状态（例如眩晕）
new ChangeStateData { target = TargetType.HIT_TARGET, state = STATE_DEFINE.STUN }
```

### 3.6 修复执行器内部硬编码

**`DamageExecutor` — 不再自己穿透：**

```csharp
// 之前（绕过 et 自己穿透）
var from = stage.SeekBehaviorInfo(flowinfo.owner, out MagicInfo magicinfo)
    ? magicinfo.owner
    : flowinfo.owner;

// 之后：直接用引擎搜索 CASTER
var from = stage.target.Search(flowinfo, TargetType.CASTER);
var damage = stage.attrb.ChargeDamage(from, data.strength * stage.cfg.int2fp);
stage.attrb.ToDamage(from, target, damage);
```

**`BeHitExecutor` — 攻击者位置用 CASTER 而非 flowinfo.owner：**

```csharp
// 之前（拿 Magic 的 spatial，潜在 bug）
stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo atkspatial);

// 之后：搜索真正的施法者
var caster = stage.target.Search(flowinfo, TargetType.CASTER);
stage.SeekBehaviorInfo(caster, out SpatialInfo atkspatial);
```

---

## 四、搜索实现

### 4.1 TargetSearcher

```csharp
/// <summary>
/// 目标搜索器：根据语义目标枚举，在当前上下文中搜索具体 Actor
/// </summary>
public class TargetSearcher
{
    private Stage stage { get; set; }

    /// <summary>
    /// 搜索单个目标（CASTER / MAGIC / SELF）
    /// </summary>
    public ulong Search(FlowInfo flowinfo, TargetType type, SparkContext sparkctx = null)
    {
        switch (type)
        {
            case TargetType.SELF:
                return flowinfo.actor;

            case TargetType.MAGIC:
                return stage.SeekBehaviorInfo(flowinfo.owner, out MagicInfo magic) ? flowinfo.owner : 0;

            case TargetType.CASTER:
                return SearchCaster(flowinfo);

            default:
                throw new Exception($"single-target search not applicable for {type}");
        }
    }

    /// <summary>
    /// 搜索多个目标（HIT_TARGET），逐个回调
    /// </summary>
    public void SearchMulti(FlowInfo flowinfo, TargetType type, Action<ulong> onTarget, SparkContext sparkctx = null)
    {
        switch (type)
        {
            case TargetType.HIT_TARGET:
                // 优先从 spark 上下文取（解决 targets.Clear 时序问题）
                if (sparkctx != null && sparkctx.hitActor != 0)
                {
                    onTarget(sparkctx.hitActor);
                    return;
                }
                // 回退到碰撞信息
                if (stage.SeekBehaviorInfo(flowinfo.actor, out FlowCollisionHurtInfo hurt))
                {
                    foreach (var t in hurt.targets) onTarget(t.actor);
                }
                break;

            default:
                // 单目标类型也兼容多目标调用
                onTarget(Search(flowinfo, type, sparkctx));
                break;
        }
    }

    /// <summary>
    /// 搜索施法者：从 flowinfo.owner 向上穿透中间 Actor，直到角色
    /// </summary>
    private ulong SearchCaster(FlowInfo flowinfo)
    {
        ulong current = flowinfo.owner;
        for (int i = 0; i < FLOW_DEFINE.MAX_TARGET_SEARCH_DEPTH; i++)
        {
            if (current == 0) return 0;
            // 穿透魔法体
            if (stage.SeekBehaviorInfo(current, out MagicInfo magic)) { current = magic.owner; continue; }
            // 未来：穿透 Buff / Projectile ...
            // if (stage.SeekBehaviorInfo(current, out BuffInfo buff)) { current = buff.owner; continue; }
            // 到达角色
            return current;
        }
        return current;
    }
}
```

### 4.2 ExecuteInstruct 改造

```csharp
private bool ExecuteInstruct(ExecuteInstructType type, ..., SparkContext sparkctx = null)
{
    // ...
    var executed = false;

    void Do(ulong target) { /* 不变 */ }

    // 新版：语义目标优先
    if (data.target != 0)
    {
        stage.target.SearchMulti(flowinfo, data.target, Do, sparkctx);
    }
    else
    {
        // 兼容：旧版 et 逻辑（未迁移的指令走这里）
        switch (data.et) { /* 原逻辑不变 */ }
    }

    return executed;
}
```

### 4.3 SparkContext 解决时序耦合

火花触发时携带命中目标，不再依赖 `targets.Clear()` 时序：

```csharp
public struct SparkContext
{
    /// <summary>
    /// 当前火花携带的命中目标（0 表示无）
    /// </summary>
    public ulong hitActor;
}

// Flow.Spark 增加 context 参数
public void Spark(ulong actor, string token, SparkContext ctx = default)
{
    // ... 遍历火花指令时，把 ctx 传给 ExecuteInstruct ...
    ExecuteInstruct(..., sparkctx: ctx);
}

// CollisionExecutor 触发火花时携带命中目标
if (data.usespark && flowcollision.targets.Count > 0)
{
    foreach (var (hitActor, _) in flowcollision.targets)
    {
        stage.flow.Spark(flowinfo.owner, data.spark.token, new SparkContext { hitActor = hitActor });
    }
    flowcollision.targets.Clear();
}
```

这样 `DamageData`（天然 `HIT_TARGET`）在火花上下文中能直接拿到命中的那个角色，不再依赖 `targets` 列表是否还在。

---

## 五、迁移方案

### 5.1 三阶段

```
Phase 1（兼容）              Phase 2（迁移）              Phase 3（清理）
┌─────────────────┐        ┌─────────────────┐        ┌─────────────────┐
│ et 保留不变      │  →    │ 指令加 target   │  →    │ 只剩 target     │
│ 新增 TargetType  │        | 默认目标优先      │        | 移除 et 字段    │
│ 新增 Searcher   │        | 脚本去掉 et     │        | 移除 switch-case│
│ 脚本不用改       │        | 修执行器硬编码   │        |                 │
└─────────────────┘        └─────────────────┘        └─────────────────┘
```

### 5.2 Phase 1 清单

1. 新增 `TargetType` 枚举到 `Flows/Defines/`
2. 新增 `TargetSearcher` 到 `Behaviors/Sa/`（或 `Core/`）
3. `InstructData` 加 `virtual TargetType target => TargetType.CASTER`（默认施法者）
4. 各 `InstructData` 子类覆盖 `target` 声明天然目标（见 3.4 表）
5. `ExecuteInstruct` 加 `data.target != 0` 优先分支
6. `Flow.Spark` 加 `SparkContext` 参数（带默认值，旧调用不受影响）
7. 现有脚本**一行不用改**

### 5.3 Phase 2 清单

1. 给所有 `InstructData` 子类补上 `target` 覆盖
2. `S10010` / `S10020` 去掉所有 `et =` 赋值
3. `DamageExecutor` 改用 `Search(CASTER)` 替代内部穿透
4. `BeHitExecutor` 改用 `Search(CASTER)` 替代 `flowinfo.owner`
5. `CollisionExecutor` 的 spark 触发改为逐目标携带 `SparkContext`
6. 验证全部脚本行为不变

### 5.4 Phase 3 清单

1. `InstructData` 移除 `et` 字段
2. `Flow.ExecuteInstruct` 移除 et switch-case
3. `FLOW_DEFINE` 移除 `ET_*` 常量
4. 全面回归测试

---

## 六、与旧 ET 的等价映射

| 旧 ET | 新 TargetType | 说明 |
|-------|--------------|------|
| `ET_FLOW` | `SELF` | 管线自身 |
| `ET_FLOW_OWNER` | `MAGIC` 或 `CASTER` | 旧的不分 Magic/角色，新的精确区分 |
| `ET_MAGIC_OWNER` | `CASTER` | 旧的只穿一层，新的穿透任意层 |
| `ET_FLOW_HIT` | `HIT_TARGET` | 命中目标 |

---

## 七、涉及文件清单

| 文件 | 改动 | 阶段 |
|------|------|------|
| `Flows/Defines/TargetType.cs` | **新增** 枚举 | Phase 1 |
| `Behaviors/Sa/TargetSearcher.cs` | **新增** 搜索器 | Phase 1 |
| `Executors/Common/InstructData.cs` | 加 `virtual target` | Phase 1 |
| `Executors/Instructs/*.cs` (16 个) | 覆盖 `target` 声明天然目标 | Phase 1/2 |
| `Behaviors/Sa/Flow.cs` | `ExecuteInstruct` 加新分支；`Spark` 加 `SparkContext` | Phase 1/2 |
| `Executors/CollisionExecutor.cs` | spark 逐目标携带 context | Phase 2 |
| `Executors/DamageExecutor.cs` | 用 `Search(CASTER)` 替代内部穿透 | Phase 2 |
| `Executors/BeHitExecutor.cs` | 用 `Search(CASTER)` 替代 `flowinfo.owner` | Phase 2 |
| `Flows/Scriptings/S10010.cs` | 去掉 `et =` | Phase 2 |
| `Flows/Scriptings/S10020.cs` | 去掉 `et =` | Phase 2 |
| `Flows/Defines/FLOW_DEFINE.cs` | 移除 `ET_*` | Phase 3 |

---

## 八、风险与注意事项

1. **序列化兼容**：`target` 是 `virtual` 属性，不参与序列化；`et` 字段保留到 Phase 3，旧存档可读
2. **帧同步安全**：搜索全部走 `stage.SeekBehaviorInfo` / `stage.cache.Valid`，确定性不变；穿透深度上限 `MAX_TARGET_SEARCH_DEPTH` 防环
3. **性能**：`CASTER` 搜索穿透层数 ≤ 4，等价于当前 `ET_MAGIC_OWNER` 的一次 if；`HIT_TARGET` 的展开与当前 foreach 等价
4. **SparkContext 传递**：需要确认所有 `Spark` 调用链路都把 context 透传到 `ExecuteInstruct`
5. **回滚零风险**：Phase 1 完全兼容，`et` 路径不动；Phase 2 迁移后可逐脚本验证

---

## 九、总结

| 维度 | 旧 `byte et` | 新 `TargetType` |
|------|-------------|----------------|
| 枚举语义 | 结构路径（OWNER/FLOW_HIT） | 具体实体（CASTER/MAGIC/HIT_TARGET/SELF） |
| 谁定目标 | 脚本手写 `et` | 指令自带 `virtual target` |
| 脚本工作量 | 每条指令都要写 | 90% 不用写，只在覆盖时写 |
| 穿透能力 | `ET_MAGIC_OWNER` 只穿一层 | `CASTER` 穿透任意层任意类型 |
| 上下文感知 | 否 | 是（SparkContext 携带命中目标） |
| 执行器信任 | 不信，DamageExecutor 自己穿透 | 信，统一走 Searcher |
| 扩展新实体 | 改 3 处代码 | 加一个枚举值 + 一条搜索规则 |
