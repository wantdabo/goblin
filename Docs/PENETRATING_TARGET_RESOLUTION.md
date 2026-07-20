# 穿透式目标查找规则方案

> 2026-07-20 | ET_ 枚举扩展 + SeekETTarget 统一解析，干掉 ET_MAGIC_OWNER

---

## 一、问题现状

### 1.1 当前 ET 体系

```csharp
// FLOW_DEFINE.cs
public const byte ET_FLOW        = 1;  // 管线自身
public const byte ET_FLOW_OWNER  = 2;  // 管线拥有者（默认）
public const byte ET_FLOW_HIT    = 3;  // 命中列表
public const byte ET_CASTER      = 4;  // 施法者（穿透搜索）
public const byte ET_HIT_VICTIM  = 5;  // 受击者（等同 ET_FLOW_HIT，预留逐目标区分）
```

### 1.2 已解决的问题

| 问题 | 状态 |
|------|------|
| `ET_MAGIC_OWNER`（只有一层穿透，语义不对） | **已移除** |
| DamageExecutor 私下穿透 | **已修复**（走 `SeekETTarget(ET_CASTER)`） |
| BeHitExecutor 拿 Magic 当攻击者 | **已修复**（走 `SeekETTarget(ET_CASTER)`） |
| 指令数据类 et 默认值未显式声明 | **已显式**（13 个 `ET_FLOW_OWNER` + 2 个 `ET_FLOW_HIT`） |

### 1.3 核心诉求

```
ET_MAGIC_OWNER → 干掉，用 ET_CASTER 替代  ✅
ET_FLOW / ET_FLOW_OWNER / ET_FLOW_HIT → 保留 ✅
新增 ET_CASTER / ET_HIT_VICTIM           ✅
全部走 SeekETTarget 统一解析              ✅
```

---

## 二、Actor 层级

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

链上存在的具体实体：施法者、魔法体、受击者、管线自身。

---

## 三、ET 枚举

全部在 `FLOW_DEFINE` 中定义，`InstructData.et` 字段不变。

```csharp
// FLOW_DEFINE.cs
public class FLOW_DEFINE
{
    public const byte ET_FLOW       = 1;
    public const byte ET_FLOW_OWNER = 2;
    public const byte ET_FLOW_HIT   = 3;
    public const byte ET_CASTER     = 4;
    public const byte ET_HIT_VICTIM = 5;
}
```

```csharp
// InstructData.cs
public byte et = FLOW_DEFINE.ET_FLOW_OWNER;
```

不设 `et` 时默认发到管线拥有者。

---

## 四、SeekETTarget — 统一解析

定义在 `Flow.cs`，根据 `et` 返回单个目标。

```csharp
// Flow.cs
/// <summary>
/// 根据 ET 枚举搜索目标
/// </summary>
/// <param name="flowinfo">管线信息</param>
/// <param name="et">执行目标类型</param>
/// <returns>目标 ActorID，搜索失败返回 0</returns>
public ulong SeekETTarget(FlowInfo flowinfo, byte et)
{
    switch (et)
    {
        case FLOW_DEFINE.ET_FLOW:       return flowinfo.actor;
        case FLOW_DEFINE.ET_FLOW_OWNER: return flowinfo.owner;
        case FLOW_DEFINE.ET_CASTER:
        {
            var current = flowinfo.owner;
            for (var depth = 0; depth < FLOW_DEFINE.MAX_CASTER_SEARCH_DEPTH; depth++)
            {
                // 检查 Tag 系统的 ACTOR_TYPE
                if (stage.SeekBehavior(current, out Tag tag) && tag.Get(TAG_DEFINE.ACTOR_TYPE, out var actortype))
                {
                    if (ACTOR_DEFINE.CASTER_TYPES.Contains((byte)actortype)) return current;
                    if (ACTOR_DEFINE.NONE == actortype || ACTOR_DEFINE.STAGE == actortype) return 0;
                }
                // 穿透中间层
                if (stage.SeekBehaviorInfo(current, out MagicInfo magic)) { current = magic.owner; continue; }
                if (stage.SeekBehaviorInfo(current, out BuffInfo buff)) { current = buff.owner; continue; }
                return 0;
            }
            return 0;
        }
        default: return 0;
    }
}
```

### 4.1 ET_CASTER — 穿透搜索施法者

破两层查找：用 **Tag 系统** 判断是否到达角色，同时**按 Info 类型穿透**中间层。

```
current = flowinfo.owner
loop (上限 MAX_CASTER_SEARCH_DEPTH = 8):
  → 检查 Tag(ACTOR_TYPE)
    → 在 CASTER_TYPES {HERO, ENEMY} 中 → 返回 current（找到施法者）
    → 为 NONE 或 STAGE → 返回 0（无效）
  → 有 MagicInfo  → current = magic.owner, continue
  → 有 BuffInfo   → current = buff.owner, continue
  → 都不是       → 返回 0
```

优势：不依赖固定链长度，用 Tag 精确判断"谁是角色"。

```
Hero 直接创建 Flow         → flowinfo.owner = Hero          → Tag 命中 CASTER_TYPES → Hero ✓
Hero → Magic → Flow        → flowinfo.owner = Magic          → Magic 穿透 → Hero ✓
Hero → Buff → Magic → Flow → flowinfo.owner = Magic          → Magic 穿透 → Buff 穿透 → Hero ✓
```

### 4.2 ET_FLOW_HIT / ET_HIT_VICTIM — 命中列表

不在 `SeekETTarget` 中处理（返回 0）。由 `ExecuteInstruct` 展开 `FlowCollisionHurtInfo.targets`。

当前两者行为**完全相同**，`ET_HIT_VICTIM` 是为未来逐目标区分预留的语义标记。

---

## 五、ExecuteInstruct 改造

```csharp
private bool ExecuteInstruct(ExecuteInstructType type, ..., FlowInfo flowinfo)
{
    var executed = false;
    void Do(ulong target) { /* 不变 */ }

    switch (data.et)
    {
        case FLOW_DEFINE.ET_FLOW_HIT:
        case FLOW_DEFINE.ET_HIT_VICTIM:
            if (stage.SeekBehaviorInfo(flowinfo.actor, out FlowCollisionHurtInfo flowcollision))
                foreach (var target in flowcollision.targets) Do(target.actor);
            break;
        default:
            Do(SeekETTarget(flowinfo, data.et));
            break;
    }

    return executed;
}
```

单目标走 `SeekETTarget`，多目标（碰撞列表）在 `ExecuteInstruct` 里展开。

---

## 六、脚本对比

### S10010 翻滚

**之前：**
```csharp
Instruct(0, 0, new ChangeStateData   { et = ET_MAGIC_OWNER, state = STATE_DEFINE.ROLL });
Instruct(0, 0, new SoundInstructData { et = ET_MAGIC_OWNER, soundid = 1000001 });
Instruct(0, 320, new SpatialPositionData { et = ET_MAGIC_OWNER, position = ... });
```

**之后：**
```csharp
Instruct(0, 0, new ChangeStateData
{
    et = FLOW_DEFINE.ET_CASTER,
    state = STATE_DEFINE.ROLL,
    force = true,
    usedelaybreak = true,
    delaybreak = 320,
});
Instruct(0, 0, new SoundInstructData { et = FLOW_DEFINE.ET_CASTER, soundid = 1000001 });
ScriptMachine.Instruct(0, 320, new SpatialPositionData
{
    et = FLOW_DEFINE.ET_CASTER,
    type = SPATIAL_DEFINE.POSITION_SELF,
    position = new IntVector3(0, 0, 200),
}, checkonce: false);
```

### S10020 重击

**之前：**
```csharp
Instruct(0, 0, new SoundInstructData { soundid = 1000002 });
Instruct(200, 500, new CollisionData { et = ET_MAGIC_OWNER, ... });
Instruct(200, 200, new BeHitData { et = ET_FLOW_HIT, ... });
Instruct(200, 200, new HitLagData { et = ET_FLOW_HIT, ... });
Instruct(SPARK..., new DamageData { et = ET_FLOW_HIT, strength = 3000 });
```

**之后：**
```csharp
Instruct(0, 0, new SoundInstructData { soundid = 1000002 });           // 默认 ET_FLOW_OWNER
Instruct(200, 500, new CollisionData { et = FLOW_DEFINE.ET_CASTER, ... });
Instruct(200, 200, new BeHitData { ... });                              // 默认 ET_FLOW_HIT
Instruct(200, 200, new HitLagData { et = FLOW_DEFINE.ET_HIT_VICTIM, ... });
ScriptMachine.Instruct(SPARK..., new DamageData { strength = 3000 });  // 默认 ET_FLOW_HIT
```

---

## 七、修复执行器硬编码

### DamageExecutor

```csharp
// 之前（不信 et，私下穿透）
var from = stage.SeekBehaviorInfo(flowinfo.owner, out MagicInfo magicinfo)
    ? magicinfo.owner
    : flowinfo.owner;

// 之后：直接走 SeekETTarget
var from = stage.flow.SeekETTarget(flowinfo, FLOW_DEFINE.ET_CASTER);
```

### BeHitExecutor

```csharp
// 之前（拿 Magic 的 spatial，潜在 bug）
stage.SeekBehaviorInfo(flowinfo.owner, out SpatialInfo atkspatial);

// 之后：找真正的施法者
var caster = stage.flow.SeekETTarget(flowinfo, FLOW_DEFINE.ET_CASTER);
stage.SeekBehaviorInfo(caster, out SpatialInfo atkspatial);
```

---

## 八、火花触发

### 8.1 CollisionExecutor 命中火花

碰撞检测命中后，从 `flowinfo.actor` 向所有命中目标统一触发一次火花：

```csharp
// CollisionExecutor.OnCollision
if (false == data.usespark) return;
if (0 == flowcollision.targets.Count) return;
stage.flow.Spark(flowinfo, data.spark.influence, data.spark.token);
flowcollision.targets.Clear();
```

火花内的 DamageData 等指令 `et = ET_FLOW_HIT`，依赖碰撞刚填充的 `FlowCollisionHurtInfo.targets` 展开执行。

### 8.2 当前局限

- 不逐目标传入 hitactor，而是批量触发一次火花
- `ET_FLOW_HIT` 和 `ET_HIT_VICTIM` 行为相同，语义区分未利用
- 火花内部无法区分"当前在处理哪个受击者"

未来可沿 `Spark(flowinfo, influence, token, hittarget)` 方向细化。

---

## 九、速查表

| ET 枚举 | 值 | 含义 | 搜索方式 |
|---------|---|------|---------|
| `ET_FLOW` | 1 | 管线自身 | `SeekETTarget` → `flowinfo.actor` |
| `ET_FLOW_OWNER` | 2 | 管线拥有者 | `SeekETTarget` → `flowinfo.owner`（默认值） |
| `ET_FLOW_HIT` | 3 | 命中列表 | `ExecuteInstruct` 展开 → `FlowCollisionHurtInfo.targets` |
| `ET_CASTER` | 4 | **施法者** | `SeekETTarget` → Tag 判断 + Magic/Buff 穿透 |
| `ET_HIT_VICTIM` | 5 | **受击者** | 当前等同 `ET_FLOW_HIT`（预留逐目标） |

---

## 十、迁移状态

### 已完成 ✅

- `FLOW_DEFINE` 新增 `ET_CASTER` / `ET_HIT_VICTIM`，移除 `ET_MAGIC_OWNER`
- `Flow` 新增 `SeekETTarget`、`MAX_CASTER_SEARCH_DEPTH`
- `S10010` / `S10020`：`ET_MAGIC_OWNER` → `ET_CASTER`
- `DamageExecutor` / `BeHitExecutor` 用 `SeekETTarget(ET_CASTER)`
- 全部 15 个 InstructData 子类显式声明构造函数默认 `et`
- 移除 `MagicMotionData` / `MagicMotionExecutor` / `INSTR_DEFINE.MAGIC_MOTION`

### 未实现 📋

- `Spark` 加 hittarget 参数、逐目标传入 hitactor
- `ET_HIT_VICTIM` 利用 sparktarget 做逐目标区分
- CollisionExecutor 逐目标循环触发火花

---

## 十一、涉及文件

| 文件 | 改动 |
|------|------|
| `Flows/Defines/FLOW_DEFINE.cs` | 新增 `ET_CASTER`/`ET_HIT_VICTIM`；移除 `ET_MAGIC_OWNER`；加 `MAX_CASTER_SEARCH_DEPTH` |
| `Behaviors/Sa/Flow.cs` | 新增 `SeekETTarget`；`ExecuteInstruct` 改造；移除 `MagicMotionExecutor` 注册 |
| `Executors/CollisionExecutor.cs` | spark 触发逻辑 |
| `Executors/DamageExecutor.cs` | 用 `SeekETTarget(ET_CASTER)` |
| `Executors/BeHitExecutor.cs` | 用 `SeekETTarget(ET_CASTER)` |
| `Flows/Scriptings/S10010.cs` | `ET_MAGIC_OWNER` → `ET_CASTER` |
| `Flows/Scriptings/S10020.cs` | `ET_MAGIC_OWNER` → `ET_CASTER`；移除冗余 et 赋值 |
| `Executors/Instructs/*.cs` | 15 个子类显式构造函数默认 et |
| `Flows/Defines/INSTR_DEFINE.cs` | 移除 `MAGIC_MOTION` |
| `Executors/MagicMotionData.cs` | **已删除** |
| `Executors/MagicMotionExecutor.cs` | **已删除** |
| `Common/Defines/ACTOR_DEFINE.cs` | 新增 `CASTER_TYPES` 集合 |

---

## 十二、InstructData.et 默认值分析

所有指令数据类继承 `InstructData`，基类默认 `et = FLOW_DEFINE.ET_FLOW_OWNER`。仅 BeHitData 和 DamageData 在构造函数中覆盖为 `ET_FLOW_HIT`。

### 12.1 使用默认 ET_FLOW_OWNER（13 个）

作用于**管线拥有者**：

| 指令 | 数据类 | 语义 |
|------|--------|------|
| 动画 | `AnimationData` | 播动画 |
| 切换状态 | `ChangeStateData` | 切状态 |
| 碰撞 | `CollisionData` | 发起碰撞检测 |
| 创建魔法体 | `CreateMagicData` | 创建魔法体 |
| 特效 | `EffectData` | 挂特效 |
| 顿帧 | `HitLagData` | 命中顿帧（通过碰撞列表展开） |
| 释放技能 | `LaunchSkillData` | 释放技能 |
| 移除 Actor | `RmvActorData` | 移除自身 |
| 打断技能 | `SkillBreakData` | 打断技能 |
| 声音 | `SoundInstructData` | 播放声音 |
| 火花 | `SparkData` | 管线火花 |
| 空间位置 | `SpatialPositionData` | 空间位置设置 |
| 时间缩放 | `TimeScaleData` | 时间缩放 |

### 12.2 覆盖 ET_FLOW_HIT（2 个）

作用于**碰撞命中的目标（受击者）**：

| 指令 | 数据类 | 语义 |
|------|--------|------|
| 受击 | `BeHitData` | 受击者播放受击动作 |
| 伤害 | `DamageData` | 受击者承担伤害 |

```csharp
// BeHitData 构造函数
public BeHitData() { et = FLOW_DEFINE.ET_FLOW_HIT; }

// DamageData 构造函数
public DamageData() { et = FLOW_DEFINE.ET_FLOW_HIT; }
```

### 12.3 总结

| ET 值 | 语义 | 覆盖的类 |
|-------|------|---------|
| `ET_FLOW_OWNER` (2) | 管线拥有者（默认） | 其余 13 个 |
| `ET_FLOW_HIT` (3) | 受击目标 | BeHitData, DamageData |

规则：**不设 `et` 时默认打到管线拥有者；需要打到受击目标的指令显式覆盖。**

---

## 十三、总结

在原有 `InstructData.et` 字段上扩展枚举，`SeekETTarget` 统一解析单目标，多目标在 `ExecuteInstruct` 展开。

```
ET_FLOW         (1) → SeekETTarget → flowinfo.actor
ET_FLOW_OWNER   (2) → SeekETTarget → flowinfo.owner
ET_FLOW_HIT     (3) → ExecuteInstruct 展开 → FlowCollisionHurtInfo.targets
ET_CASTER       (4) → SeekETTarget → Tag 判断 + Magic/Buff 穿透
ET_HIT_VICTIM   (5) → ExecuteInstruct 展开 → 当前等同 ET_FLOW_HIT（预留逐目标）
```

`ET_CASTER` 穿透用 Tag 系统精确识别角色类型，不依赖固定链层数，为未来 Buff/Projectile 等中间层扩展留下空间。
