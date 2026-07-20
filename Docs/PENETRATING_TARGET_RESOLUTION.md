# 穿透式目标查找规则方案

> 2026-07-19 | 保留手动挡 ET 常量，新增模糊目标意图，引擎自动搜索

---

## 一、问题现状

### 1.1 当前 ET 体系

```csharp
// FLOW_DEFINE.cs
public const byte ET_FLOW        = 1;  // flowinfo.actor（管线自身）
public const byte ET_FLOW_OWNER  = 2;  // flowinfo.owner（管线拥有者，默认值）
public const byte ET_FLOW_HIT    = 3;  // FlowCollisionHurtInfo.targets（命中列表）
public const byte ET_MAGIC_OWNER = 4;  // 穿透一层 Magic，取 magic.owner
```

### 1.2 当前问题

ET 枚举命名的是**结构路径**（OWNER、FLOW_HIT），不是**具体实体**。回答的是"怎么走"，而非"找谁"。

| 问题 | 表现 |
|------|------|
| `ET_MAGIC_OWNER` 只穿透一层 | 换层级（Buff/Projectile）就失效 |
| 指令没有天然目标 | 每条指令都要手写 `et =`，脚本冗长 |
| DamageExecutor 不信任 et | 执行器自己私下穿透一遍（`DamageExecutor.cs:28-30`）|
| BeHitExecutor 拿 Magic 当攻击者 | `flowinfo.owner` 是 Magic 不是英雄（潜在 bug） |
| 火花依赖 `targets.Clear()` 时序 | 异步化即静默失败 |

### 1.3 核心诉求

```text
旧（手动挡）:  ET_FLOW_OWNER / ET_FLOW_HIT     → "取拥有者" / "取命中列表"
新（自动挡）:  CASTER / HIT_VICTIM              → "找施法者" / "找受击者"

手动挡保留：精确、可控，开发知道自己要什么路径
自动挡新增：模糊、声明式，只说找谁，引擎自己搜
```

---

## 二、Actor 层级回顾

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
                    └── 被打中的 Hero/Enemy（受击者）
```

链上存在的**具体实体**只有四个：施法者、魔法体、受击者、管线自身。

---

## 三、双模式设计

```
         手动挡（精确）              自动挡（模糊）
              │                          │
    ET_FLOW           ──→    SELF        "管线自身"
    ET_FLOW_OWNER     ──→    MAGIC       "魔法体"（flowinfo.owner）
    ET_FLOW_HIT       ──→    HIT_VICTIM   "受击者"（碰撞命中的人）
       -              ──→    CASTER       "施法者"（穿透任意层找角色）
```

### 3.1 手动挡：保留不改

```csharp
// FLOW_DEFINE.cs — 保留，一字不动
public const byte ET_FLOW        = 1;  // 管线自身（精确路径）
public const byte ET_FLOW_OWNER  = 2;  // 管线拥有者（精确路径，默认值）
public const byte ET_FLOW_HIT    = 3;  // 管线命中列表（精确路径）
```

**手动挡的特点**：脚本精确控制路径。`ET_FLOW_OWNER` 就是 `flowinfo.owner`，不做任何穿透。脚本知道自己在什么层级，想要什么结构上的东西。适合特殊场景、调试、以及不信任自动搜索的情况。

### 3.2 自动挡：模糊意图（新增）

```csharp
/// <summary>
/// 模糊目标意图 — 声明"找谁"，引擎负责搜索
/// 与 ET 手动挡并存，intent != 0 时走自动搜索，否则回退到 et
/// </summary>
public enum TargetIntent : byte
{
    /// <summary>未指定，回退到 et 手动挡</summary>
    None = 0,

    /// <summary>
    /// 施法者：发起本次技能的角色（Hero/Enemy）
    /// 引擎从 flowinfo.owner 向上穿透 Magic/Buff/Projectile，直到找到角色
    /// </summary>
    CASTER = 1,

    /// <summary>
    /// 受击者：碰撞检测命中的角色
    /// 逐目标展开，包含火花上下文
    /// </summary>
    HIT_VICTIM = 2,

    /// <summary>
    /// 魔法体：当前管线归属的 Magic Actor
    /// 等价于 flowinfo.owner（当它是 Magic 时）
    /// </summary>
    MAGIC = 3,

    /// <summary>
    /// 管线自身：flowinfo.actor
    /// </summary>
    SELF = 4,
}
```

**自动挡的特点**：脚本只说语义意图（"我要施法者"），不管中间隔了几层。引擎负责穿透搜索。适合 90% 的常规场景。

### 3.3 InstructData 双模字段

```csharp
public abstract class InstructData
{
    // ── 手动挡：保留兼容 ──
    public byte et = FLOW_DEFINE.ET_FLOW_OWNER;

    // ── 自动挡：新增 ──
    public TargetIntent intent = TargetIntent.None;
}
```

**解析顺序：`intent` 优先，`et` 兜底。**

```
ExecuteInstruct:
  if data.intent != None
    → 走自动搜索（TargetSearcher）
  else
    → 走手动路径（原 switch-case et）
```

### 3.4 手动挡 vs 自动挡：何时用哪个

| 场景 | 用 | 原因 |
|------|----|------|
| 播放施法者动画 | `CASTER` | 语义明确 |
| 伤害受击者 | `HIT_VICTIM` | 语义明确 |
| 移动魔法体 | `MAGIC` | 语义明确 |
| 火花特殊目标 | `ET_FLOW_OWNER` | 精确知道要走 owner |
| 调试/黑科技 | `et` 手动挡 | 不走自动搜索，手控路径 |
| 新类型 Actor 暂未支持模糊搜索 | `et` 手动挡 | 兜底方案 |

---

## 四、自动搜索规则

### 4.1 CASTER — 找施法者

```
输入: flowinfo
搜索:
  current = flowinfo.owner
  loop (上限 4 层):
    if current 有 MagicInfo      → current = magic.owner     // 穿透魔法体
    else if current 有 BuffInfo  → current = buff.owner      // 穿透 Buff（未来）
    else if current 有 ProjectileInfo → current = proj.owner // 穿透弹道（未来）
    else                          → 返回 current             // 到达角色
```

**关键：无论中间隔了几层、隔什么类型，都能找到角色。** 这是 `ET_MAGIC_OWNER` 做不到的——它只穿一层 Magic。

```
Hero 直接创建 Flow         → flowinfo.owner = Hero          → 搜到 Hero ✓
Hero → Magic → Flow        → flowinfo.owner = Magic          → 穿透到 Hero ✓
Hero → Buff → Magic → Flow → flowinfo.owner = Magic          → 穿透到 Hero ✓
Projectile → Flow          → flowinfo.owner = Projectile     → 穿到 Projectile.owner = Hero ✓
```

### 4.2 HIT_VICTIM — 找受击者

```
输入: flowinfo + sparkctx
搜索:
  if sparkctx.hitActor != 0  → 返回 sparkctx.hitActor（火花逐目标）
  else 取 FlowCollisionHurtInfo.targets → 逐个展开
```

火花触发时，命中目标由 `SparkContext` 携带，不再依赖 `targets.Clear()` 时序。

### 4.3 MAGIC — 找魔法体

```
if flowinfo.owner 有 MagicInfo → 返回 flowinfo.owner
else                            → 返回 0
```

### 4.4 SELF — 管线自身

```
返回 flowinfo.actor
```

---

## 五、指令天然目标（可选增强）

当前脚本满屏的 `et =` 本质是因为指令不知道自己的天然目标。可以进一步让每个 `InstructData` 子类声明默认的 `intent`：

| 指令 | 天然 intent | 理由 |
|------|-----------|------|
| `AnimationData` | `CASTER` | 播放施法者动画 |
| `SpatialPositionData` | `CASTER` | 移动施法者 |
| `CreateMagicData` | `CASTER` | 在施法者位置生成 |
| `MagicMotionData` | `MAGIC` | 移动魔法体 |
| `LaunchSkillData` | `CASTER` | 施法者释放技能 |
| `EffectData` | `CASTER` | 特效挂在施法者身上 |
| `CollisionData` | `CASTER` | 从施法者位置检测碰撞 |
| `RmvActorData` | `SELF` | 移除管线自身 |
| `ChangeStateData` | `CASTER` | 切施法者状态 |
| `SparkData` | — | 不用 target |
| `HitLagData` | `HIT_VICTIM` | 冻结受击者 |
| `TimeScaleData` | `CASTER` | 缩放施法者时间 |
| `BeHitData` | `HIT_VICTIM` | 受击者进入受击状态 |
| `SkillBreakData` | `HIT_VICTIM` | 打断受击者技能 |
| `DamageData` | `HIT_VICTIM` | 伤害受击者 |
| `SoundInstructData` | `CASTER` | 声源在施法者 |

实现：

```csharp
// 指令自带天然目标，脚本不写 intent 就走这个
public class BeHitData : InstructData
{
    public override TargetIntent DefaultIntent => TargetIntent.HIT_VICTIM;
    // ...
}
```

脚本只需在**覆盖默认**时才显式写：

```csharp
// 罕见：受击者也切个状态（例如击晕）
new ChangeStateData { intent = TargetIntent.HIT_VICTIM, state = STATE_DEFINE.STUN }
```

---

## 六、脚本前后对比

### S10010 翻滚

**现在（全是手动挡）：**
```csharp
Instruct(0, 0, new ChangeStateData    { et = ET_MAGIC_OWNER, state = STATE_DEFINE.ROLL });
Instruct(0, 0, new SoundInstructData  { et = ET_MAGIC_OWNER, soundid = 1000001 });
Instruct(0, 320, new SpatialPositionData { et = ET_MAGIC_OWNER, position = ... });
```

**之后（指令默认 + 模糊意图）：**
```csharp
// 三条指令天然作用于 CASTER，不写任何目标
Instruct(0, 0,   new ChangeStateData    { state = STATE_DEFINE.ROLL });
Instruct(0, 0,   new SoundInstructData  { soundid = 1000001 });
Instruct(0, 320, new SpatialPositionData { position = ... });
```

### S10020 重击

**现在：**
```csharp
Instruct(0, 0, new SoundInstructData { soundid = 1000002 });
Instruct(200, 500, new CollisionData { et = ET_MAGIC_OWNER, ... });   // 手写
Instruct(200, 200, new BeHitData     { et = ET_FLOW_HIT, ... });      // 手写
Instruct(200, 200, new HitLagData    { et = ET_FLOW_HIT, ... });      // 手写
Instruct(SPARK..., new DamageData { et = ET_FLOW_HIT, strength = 3000 });
```

**之后：**
```csharp
// Collision 天然 CASTER，BeHit/HitLag/Damage 天然 HIT_VICTIM，全部不写
Instruct(0, 0,   new SoundInstructData { soundid = 1000002 });
Instruct(200, 500, new CollisionData { ... });
Instruct(200, 200, new BeHitData    { ... });
Instruct(200, 200, new HitLagData   { ... });
Instruct(SPARK..., new DamageData { strength = 3000 });
```

### 手动挡兜底示例

```csharp
// 某些特殊火花：我想直接对 flowinfo.owner 操作，不要穿透
Instruct(SPARK_CUSTOM, new EffectData { et = ET_FLOW_OWNER, effectid = 9999 });
//                                     ↑ 手动挡：明确指定就是 owner，不走模糊搜索
```

---

## 七、修复执行器硬编码

### DamageExecutor — 不再自己穿透

```csharp
// 之前（绕过 et，私下穿透）
var from = stage.SeekBehaviorInfo(flowinfo.owner, out MagicInfo magicinfo)
    ? magicinfo.owner
    : flowinfo.owner;

// 之后：直接用模糊搜索
var from = stage.target.Search(flowinfo, TargetIntent.CASTER);
stage.attrb.ToDamage(from, target, damage);
```

### BeHitExecutor — 攻击者位置用 CASTER

```csharp
// 之前（拿 Magic 的 spatial，潜在 bug）
stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo atkspatial);

// 之后：搜索真正的施法者
var caster = stage.target.Search(flowinfo, TargetIntent.CASTER);
stage.SeekBehaviorInfo(caster, out SpatialInfo atkspatial);
```

---

## 八、SparkContext 解决时序耦合

火花触发时携带命中目标，不再依赖 `targets.Clear()` 时序：

```csharp
public struct SparkContext
{
    /// <summary>当前火花携带的命中目标</summary>
    public ulong hitActor;
}

// Flow.Spark 增加 context 参数
public void Spark(ulong actor, string token, SparkContext ctx = default)
{
    ExecuteInstruct(..., sparkctx: ctx);
}

// CollisionExecutor 逐目标触发火花
if (data.usespark && flowcollision.targets.Count > 0)
{
    foreach (var (hitActor, _) in flowcollision.targets)
    {
        stage.flow.Spark(flowinfo.owner, data.spark.token,
            new SparkContext { hitActor = hitActor });
    }
    flowcollision.targets.Clear();
}
```

`DamageData`（天然 `HIT_VICTIM`）在火花上下文中直接拿到命中的目标，不再依赖 targets 列表。

---

## 九、TargetSearcher 实现

```csharp
public class TargetSearcher
{
    private Stage stage { get; set; }

    /// <summary>搜索单个目标</summary>
    public ulong Search(FlowInfo flowinfo, TargetIntent intent, SparkContext sparkctx = default)
    {
        switch (intent)
        {
            case TargetIntent.SELF:   return flowinfo.actor;
            case TargetIntent.MAGIC:  return SeekMagic(flowinfo);
            case TargetIntent.CASTER: return SeekCaster(flowinfo);
            default: return 0;
        }
    }

    /// <summary>搜索多目标，逐个回调</summary>
    public void SearchMulti(FlowInfo flowinfo, TargetIntent intent,
        Action<ulong> onTarget, SparkContext sparkctx = default)
    {
        if (intent == TargetIntent.HIT_VICTIM)
        {
            // 优先火花上下文
            if (sparkctx.hitActor != 0) { onTarget(sparkctx.hitActor); return; }
            // 回退碰撞列表
            if (stage.SeekBehaviorInfo(flowinfo.actor, out FlowCollisionHurtInfo hurt))
                foreach (var t in hurt.targets) onTarget(t.actor);
            return;
        }
        // 单目标 intent 兼容多目标调用
        onTarget(Search(flowinfo, intent, sparkctx));
    }

    private ulong SeekCaster(FlowInfo flowinfo)
    {
        ulong current = flowinfo.owner;
        for (int i = 0; i < FLOW_DEFINE.MAX_TARGET_SEARCH_DEPTH; i++)
        {
            if (current == 0) return 0;
            if (stage.SeekBehaviorInfo(current, out MagicInfo magic))
                { current = magic.owner; continue; }
            // 未来: BuffInfo / ProjectileInfo ...
            return current;
        }
        return current;
    }

    private ulong SeekMagic(FlowInfo flowinfo)
        => stage.SeekBehaviorInfo(flowinfo.owner, out MagicInfo _) ? flowinfo.owner : 0;
}
```

---

## 十、ExecuteInstruct 改造

```csharp
private bool ExecuteInstruct(ExecuteInstructType type, ..., SparkContext sparkctx = default)
{
    var executed = false;
    void Do(ulong target) { /* 不变 */ }

    // 自动挡优先
    if (data.intent != TargetIntent.None)
    {
        stage.target.SearchMulti(flowinfo, data.intent, Do, sparkctx);
    }
    else
    {
        // 手动挡兜底（原 switch-case et 不变）
        switch (data.et)
        {
            case FLOW_DEFINE.ET_FLOW:        Do(flowinfo.actor); break;
            case FLOW_DEFINE.ET_FLOW_OWNER:  Do(flowinfo.owner); break;
            case FLOW_DEFINE.ET_FLOW_HIT:    /* 原逻辑 */ break;
        }
    }
    return executed;
}
```

---

## 十一、手动挡与自动挡映射速查

| 手动挡 (et) | 自动挡 (intent) | 区别 |
|------------|----------------|------|
| `ET_FLOW` | `SELF` | 等价，都是 flowinfo.actor |
| `ET_FLOW_OWNER` | `MAGIC` | 等价，都是 flowinfo.owner |
| `ET_FLOW_HIT` | `HIT_VICTIM` | 自动挡多了 spark 上下文感知 |
| `ET_MAGIC_OWNER` | `CASTER` | 自动挡穿透任意层，手动挡只穿一层 Magic |

---

## 十二、迁移方案

### Phase 1 — 并存（零风险）

- 新增 `TargetIntent` 枚举
- 新增 `TargetSearcher`
- `InstructData` 加 `intent` 字段（默认 `None`）
- `ExecuteInstruct` 加 `intent != None` 优先分支
- `Flow.Spark` 加 `SparkContext` 参数（默认值，旧调用不受影响）
- **脚本一行不用改**，手动挡 et 路径原封不动

### Phase 2 — 指令声明默认

- 各 `InstructData` 子类覆盖 `DefaultIntent`
- `S10010` / `S10020` 去掉 `et =` 赋值
- `DamageExecutor` / `BeHitExecutor` 改用 `Search(CASTER)`
- `CollisionExecutor` spark 触发改为逐目标携带 `SparkContext`

### Phase 3 — 清理（可选）

- 评估是否移除 `ET_MAGIC_OWNER`（已被 `CASTER` 覆盖）
- `InstructData.et` 保留，作为永久手动挡
- `ET_FLOW` / `ET_FLOW_OWNER` / `ET_FLOW_HIT` **永久保留**

---

## 十三、涉及文件清单

| 文件 | 改动 | 阶段 |
|------|------|------|
| `Flows/Defines/TargetIntent.cs` | **新增** 模糊意图枚举 | Phase 1 |
| `Flows/Defines/FLOW_DEFINE.cs` | 加 `MAX_TARGET_SEARCH_DEPTH` | Phase 1 |
| `Behaviors/Sa/TargetSearcher.cs` | **新增** 搜索器 | Phase 1 |
| `Executors/Common/InstructData.cs` | 加 `intent` 字段 + `virtual DefaultIntent` | Phase 1/2 |
| `Behaviors/Sa/Flow.cs` | `ExecuteInstruct` 加模糊分支；`Spark` 加 context | Phase 1/2 |
| `Executors/Instructs/*.cs` | 覆盖 `DefaultIntent` | Phase 2 |
| `Executors/CollisionExecutor.cs` | spark 逐目标携带 context | Phase 2 |
| `Executors/DamageExecutor.cs` | 用 `Search(CASTER)` | Phase 2 |
| `Executors/BeHitExecutor.cs` | 用 `Search(CASTER)` | Phase 2 |
| `Flows/Scriptings/S10010.cs` | 去掉 `et =` | Phase 2 |
| `Flows/Scriptings/S10020.cs` | 去掉 `et =` | Phase 2 |

---

## 十四、风险与注意事项

1. **序列化兼容**：`intent` 是新字段，默认 `None`，旧数据不受影响
2. **帧同步安全**：搜索全部走 `stage.SeekBehaviorInfo`，确定性不变；穿透深度上限防环
3. **性能**：`CASTER` 搜索 ≤ 4 层，等价于当前一次 `ET_MAGIC_OWNER` 判断
4. **SparkContext 传递**：需确认所有 `Spark` 调用链路都能透传 context
5. **手动挡永久保留**：不删 `et` 字段和 `ET_FLOW` / `ET_FLOW_OWNER` / `ET_FLOW_HIT`，作为精确控制手段

---

## 十五、总结

```
          精确控制 ←→ 声明意图
              │          │
        手动挡(et)   自动挡(intent)
        保留不变       新增
              │          │
         ┌────┴──────────┴────┐
         │  intent 优先        │
         │  intent=0 → 走 et  │
         └─────────────────────┘
```

| 维度 | 手动挡 `byte et` | 自动挡 `TargetIntent` |
|------|-----------------|----------------------|
| 语义 | 结构路径（OWNER/FLOW_HIT） | 具体实体（CASTER/HIT_VICTIM） |
| 控制粒度 | 精确，脚本知道每层结构 | 模糊，只说"找谁" |
| 穿透 | `ET_MAGIC_OWNER` 只穿一层 | `CASTER` 穿透任意层任意类型 |
| 上下文感知 | 否 | 是（SparkContext） |
| 适用场景 | 调试、黑科技、特殊路径 | 90% 常规业务 |
| 删除计划 | **永久保留** | 持续扩展 |
