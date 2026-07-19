# 穿透式目标查找规则方案

> 2026-07-19 | 替代硬编码 `ET_` 常量，用规则链动态解析执行目标

---

## 一、问题分析

### 1.1 当前系统：硬编码 `byte et`

所有指令的数据类（`InstructData` 子类）通过一个 `byte et` 字段声明执行目标：

```
InstructData.et (byte)
    │
    ▼
Flow.ExecuteInstruct()  [Flow.cs:480-501]
    │
    ├── ET_FLOW (1)         → Do(flowinfo.actor)
    ├── ET_FLOW_OWNER (2)   → Do(flowinfo.owner)
    ├── ET_FLOW_HIT (3)     → foreach FlowCollisionHurtInfo.targets
    └── ET_MAGIC_OWNER (4)  → flowinfo.owner → MagicInfo.owner (穿透一层)
```

**定义位置：** `FLOW_DEFINE.cs:20-32`

```csharp
public const byte ET_FLOW        = 1;  // 管线自身 Actor
public const byte ET_FLOW_OWNER  = 2;  // 管线拥有者（默认值）
public const byte ET_FLOW_HIT    = 3;  // 碰撞命中的所有目标
public const byte ET_MAGIC_OWNER = 4;  // 魔法体拥有者（穿透 Magic 追溯到施法者）
```

**解析位置：** `Flow.cs:480-501`，一个 `switch-case` 决定如何把 `flowinfo` 和 `data.et` 映射为 `ulong target`。

### 1.2 五个根本缺陷

#### 缺陷 1：不可组合

`ET_MAGIC_OWNER` 写死了 "穿透一层 Magic" 的行为。如果需要 **穿透两层**（例如嵌套的 Magic → Owner → 再穿透一个 Buff Owner），就需要新增一个 `ET_BUFF_OWNER = 5` 并修改 switch-case。

每种新的穿透组合都需要：
- 新增一个 `ET_` 常量
- 修改 `Flow.ExecuteInstruct` 的 switch-case
- 所有使用者学习新的常量语义

#### 缺陷 2：上下文无关

同一个 `InstructData` 的 `et` 在所有执行上下文中永远相同。但实际需求中：

| 指令 | 正常执行时目标 | Spark 触发时目标 |
|------|---------------|-----------------|
| `DamageData` | `ET_FLOW_HIT`（命中目标） | 依赖 Spark 调用者的上下文 |
| `BeHitData` | `ET_FLOW_HIT` | 同上 |

当前 `DamageData` 构造函数写死 `et = ET_FLOW_HIT`（`DamageData.cs:24`），它在 S10020 的火花上下文中碰巧能工作（因为碰撞同步触发火花且 targets 未清空），但这个耦合是脆弱的——换了调用上下文就会出错。

#### 缺陷 3：火花指令目标解析耦合到调用时序

**关键时序问题（`CollisionExecutor.cs:87-92`）：**

```csharp
// 命中火花
stage.flow.Spark(flowinfo, data.spark.influence, data.spark.token);
// spark 同步执行完毕，消费者已处理，清空本帧命中列表
flowcollision.targets.Clear();
```

火花指令的 `ET_FLOW_HIT` 依赖 `flowcollision.targets` 尚未清空。当前能工作是因为 `Spark()` 是同步调用，`Clear()` 在返回后执行。但如果未来：

- Spark 改为异步调度（例如帧末批处理）
- 多个火花在同一个碰撞周期内触发
- 碰撞信息被复用或缓存

`ET_FLOW_HIT` 就会读到空列表，导致指令静默失败。

#### 缺陷 4：脚本与 Actor 层级强耦合

S10010（翻滚）中，所有指令都设置 `et = ET_MAGIC_OWNER`：

```csharp
// S10010.cs:18-38 — 三处都用了 ET_MAGIC_OWNER
Instruct(0, 0, new ChangeStateData  { et = FLOW_DEFINE.ET_MAGIC_OWNER, ... });
Instruct(0, 0, new SoundInstructData { et = FLOW_DEFINE.ET_MAGIC_OWNER, ... });
ScriptMachine.Instruct(0, 320, new SpatialPositionData { et = FLOW_DEFINE.ET_MAGIC_OWNER, ... });
```

这意味着脚本 **假设自己永远运行在 Magic → Flow 的层级下**。如果未来想在以下场景复用翻滚 Pipeline：

- Buff 触发的位移（没有 Magic 中间层）
- 道具使用的瞬发技能
- 环境触发的强制位移

`ET_MAGIC_OWNER` 将解析到错误的目标（因为 `flowinfo.owner` 不是 Magic，穿透失败回退到原始 owner）。

#### 缺陷 5：扩展性锁死

要新增目标类型必须改 3 个地方：
1. `FLOW_DEFINE.cs` — 新增常量
2. `Flow.cs:ExecuteInstruct` — 新增 case
3. 所有需要新目标的 `InstructData` 子类或脚本 — 设置 `et`

这违反了开闭原则（Open/Closed Principle）。

---

## 二、Actor 层级模型回顾

在深入方案前，先回顾当前 Actor 层级关系，这决定了目标查找的"穿透"路径：

```
Hero (Caster)
  │
  ├── SkillLauncher.Launch()
  │     │
  │     └── Magic Actor (magic.owner = Caster)
  │           │
  │           └── Flow Actor (flowinfo.owner = Magic)
  │                 │
  │                 ├── Pipeline 指令执行...
  │                 ├── CollisionData (检测碰撞)
  │                 └── FlowCollisionHurtInfo.targets (命中目标列表)
  │
  └── 也可直接创建 Flow Actor（不经过 Magic）
        └── Flow Actor (flowinfo.owner = Caster)
```

**穿透路径的语义：**

| 起点 | 穿透方式 | 终点 | 当前表达 |
|------|---------|------|---------|
| Flow Actor | `.actor` 自身 | Flow Actor | `ET_FLOW` |
| Flow Actor | `.owner` → 直接拥有者 | Magic 或 Caster | `ET_FLOW_OWNER` |
| Flow Actor | `.owner` → `.owner` | Caster | `ET_MAGIC_OWNER`（只穿 Magic 一层） |
| Flow Actor | `FlowCollisionHurtInfo.targets` | 所有碰撞命中者 | `ET_FLOW_HIT` |
| Flow Actor | `.owner` → `BuffInfo.owner` | Buff 施放者 | **无法表达** |

---

## 三、方案设计

### 3.1 核心思想：规则链

将 `byte et` 替换为 **有序规则链**。每条规则是一个原子操作，接受上游传来的 ActorID，产出下游的 ActorID。链首从 `flowinfo` 的某个锚点出发。

```
TargetRule[] chain = [规则1, 规则2, 规则3, ...]

执行时：
  current = flowinfo.actor  (或指定的锚点)
  for rule in chain:
      current = rule.Resolve(current, flowinfo, stage)
  Do(current)
```

### 3.2 规则类型定义

#### 规则原子（`TargetRuleType`）

| 规则 | 含义 | 输入 | 输出 | 示例场景 |
|------|------|------|------|---------|
| `SELF` | 保持当前 Actor | A | A | 占位/显式标识 |
| `OWNER` | 取当前 Actor 的拥有者 | A | A.owner | 穿透一层 |
| `OWNER_OF_TYPE<T>` | 穿透到指定 BehaviorInfo 类型的拥有者 | A | T.owner | 精确穿透 |
| `HIT_TARGETS` | 展开为碰撞命中的所有目标（多目标） | A | A 的 FlowCollisionHurtInfo.targets | 伤害/受击 |
| `STATE` | 查找当前 Actor 的 StateMachine 状态 | A | A 的 StateMachineInfo | 条件判断配合 |

#### 链的起点（`ChainOrigin`）

| 锚点 | 含义 |
|------|------|
| `FLOW_ACTOR` | 从 `flowinfo.actor` 出发（默认） |
| `FLOW_OWNER` | 从 `flowinfo.owner` 出发 |
| `CURRENT_TARGET` | 保持当前上下文传入的 target（用于嵌套调用） |

### 3.3 规则链示例

**与现有 ET 的等价映射：**

```
ET_FLOW         → [SELF]                                起点: FLOW_ACTOR
ET_FLOW_OWNER   → [OWNER]                               起点: FLOW_ACTOR
ET_MAGIC_OWNER  → [OWNER, OWNER_OF_TYPE<MagicInfo>]     起点: FLOW_ACTOR
ET_FLOW_HIT     → [HIT_TARGETS]                         起点: FLOW_ACTOR
```

**无法用现有 ET 表达的新组合：**

```
// 找到 Buff 的施放者
[OWNER, OWNER_OF_TYPE<BuffInfo>]

// 找到碰撞命中目标中第一个存活的、有特定 Tag 的
[HIT_TARGETS, FILTER_ALIVE, FILTER_TAG("enemy"), FIRST]

// 找到 Magic Owner，再找它的 StateMachine
[OWNER, OWNER_OF_TYPE<MagicInfo>, STATE]
```

### 3.4 数据结构设计

```csharp
/// <summary>
/// 目标查找规则链 — 替代 byte et
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class TargetChain
{
    /// <summary>
    /// 规则链起点
    /// </summary>
    public ChainOrigin origin { get; set; } = ChainOrigin.FLOW_ACTOR;

    /// <summary>
    /// 规则列表（有序执行）
    /// </summary>
    public List<TargetRule> rules { get; set; } = new List<TargetRule>();
}

/// <summary>
/// 单条目标查找规则
/// </summary>
[Serializable]
[MessagePackObject(true)]
public class TargetRule
{
    /// <summary>
    /// 规则类型
    /// </summary>
    public TargetRuleType type { get; set; }

    /// <summary>
    /// 类型参数（用于 OWNER_OF_TYPE 的 BehaviorInfo 类型标识）
    /// </summary>
    public ushort typeArg { get; set; }
}

public enum ChainOrigin : byte
{
    FLOW_ACTOR = 0,    // 从 flowinfo.actor 出发
    FLOW_OWNER = 1,    // 从 flowinfo.owner 出发
}

public enum TargetRuleType : byte
{
    SELF = 0,          // 不穿透，保持当前
    OWNER = 1,         // 取 .owner（通用穿透一层）
    OWNER_OF_TYPE = 2, // 精确穿透到指定 BehaviorInfo.owner
    HIT_TARGETS = 3,   // 展开为碰撞命中目标
}
```

### 3.5 `InstructData` 改动

```csharp
public abstract class InstructData
{
    public abstract ushort id { get; }

    /// <summary>
    /// [兼容期] 旧版执行目标，TransitionFromLegacy() 可转为 TargetChain
    /// </summary>
    public byte et = FLOW_DEFINE.ET_FLOW_OWNER;

    /// <summary>
    /// 新版穿透式目标链（非 null 时优先于 et）
    /// </summary>
    public TargetChain targetChain { get; set; }
}
```

### 3.6 `ExecuteInstruct` 改动

```csharp
private bool ExecuteInstruct(...)
{
    // ... executor / conditions setup ...

    var executed = false;

    void Do(ulong target)
    {
        // 不变
    }

    // 新版：规则链优先
    if (data.targetChain != null && data.targetChain.rules.Count > 0)
    {
        ResolveAndExecute(data.targetChain, flowinfo, Do);
    }
    else
    {
        // 兼容：旧版 et 逻辑
        switch (data.et) { /* 不变 */ }
    }

    return executed;
}

private void ResolveAndExecute(TargetChain chain, FlowInfo flowinfo, Action<ulong> doAction)
{
    // Step 1: 确定起点
    ulong current = chain.origin switch
    {
        ChainOrigin.FLOW_OWNER => flowinfo.owner,
        _ => flowinfo.actor,
    };

    // Step 2: 逐条执行规则
    for (int i = 0; i < chain.rules.Count; i++)
    {
        var rule = chain.rules[i];
        bool isLast = i == chain.rules.Count - 1;

        switch (rule.type)
        {
            case TargetRuleType.SELF:
                // 不操作
                break;

            case TargetRuleType.OWNER:
                current = stage.GetOwner(current);
                break;

            case TargetRuleType.OWNER_OF_TYPE:
                current = stage.GetOwnerOfType(current, rule.typeArg);
                break;

            case TargetRuleType.HIT_TARGETS:
                if (!isLast) throw new Exception("HIT_TARGETS must be the last rule in the chain");
                if (stage.SeekBehaviorInfo(current, out FlowCollisionHurtInfo hurt))
                {
                    foreach (var target in hurt.targets)
                        doAction(target.actor);
                }
                return; // 多目标分支结束
        }
    }

    // Step 3: 单目标执行
    doAction(current);
}
```

---

## 四、迁移方案

### 4.1 三阶段迁移

```
Phase 1 (兼容)          Phase 2 (共存)           Phase 3 (清理)
┌──────────────┐       ┌──────────────┐       ┌──────────────┐
│ et (byte)    │  →    │ et + target   │  →    │ targetChain  │
│              │       │ Chain 共存    │       │  only        │
│ switch-case  │       │ 新链优先      │       │ 移除 switch  │
└──────────────┘       └──────────────┘       └──────────────┘
```

### 4.2 Phase 1 实现要点

1. 新增 `TargetChain`、`TargetRule`、枚举类型到代码库
2. 在 `Stage` 上添加 `GetOwner()` 和 `GetOwnerOfType()` 辅助方法
3. 在 `InstructData` 上添加 `targetChain` 字段（不影响现有序列化）
4. `ExecuteInstruct` 中添加 `ResolveAndExecute` 路径（`targetChain != null` 时走新路径）
5. 所有现有脚本无需修改——`et` 路径完全保留

### 4.3 Phase 2 迁移示例

**S10010（翻滚）— 当前：**

```csharp
new ChangeStateData { et = FLOW_DEFINE.ET_MAGIC_OWNER, state = STATE_DEFINE.ROLL, ... }
new SoundInstructData { et = FLOW_DEFINE.ET_MAGIC_OWNER, soundid = 1000001 }
new SpatialPositionData { et = FLOW_DEFINE.ET_MAGIC_OWNER, ... }
```

**S10010（翻滚）— 迁移后：**

```csharp
// 定义规则链：从 Flow Actor 出发 → 取 owner(Magic) → 再取 owner(Caster)
var casterChain = new TargetChain
{
    origin = ChainOrigin.FLOW_ACTOR,
    rules = { TargetRule.Owner(), TargetRule.Owner() }
};

new ChangeStateData    { targetChain = casterChain, state = STATE_DEFINE.ROLL, ... }
new SoundInstructData  { targetChain = casterChain, soundid = 1000001 }
new SpatialPositionData{ targetChain = casterChain, ... }
```

或者提供静态快捷工厂：

```csharp
// TargetChain 静态工厂
public static TargetChain FlowOwner => new() { rules = { TargetRule.Owner() } };
public static TargetChain MagicOwner => new() { rules = { TargetRule.Owner(), TargetRule.Owner() } };
public static TargetChain HitTargets => new() { rules = { TargetRule.HitTargets() } };

// 使用
new ChangeStateData { targetChain = TargetChain.MagicOwner, ... }
```

### 4.4 S10020（重击）关键修正

当前 S10020 的碰撞 → Spark → Damage 流程存在**架构脆弱性**（见 1.2 缺陷 3）：

```
CollisionData(et=ET_MAGIC_OWNER) → 碰撞检测 → targets 填充
  → Spark(FLOW, TOKEN_PIPELINE_GEN)
    → DamageData(et=ET_FLOW_HIT) → 依赖 targets 未清空
  → targets.Clear()
```

**修正后**：Spark 触发时，将命中目标作为参数传入规则链上下文，`DamageData` 使用 `HIT_TARGETS` 规则直接读取传入的目标列表，不再依赖 `FlowCollisionHurtInfo.targets` 的时序状态。

```csharp
// CollisionExecutor — 新版 Spark 触发
// 将每个命中目标的 actor 作为 Spark 的逐目标触发
if (data.usespark && flowcollision.targets.Count > 0)
{
    foreach (var (hitActor, _) in flowcollision.targets)
    {
        stage.flow.SparkPerTarget(flowinfo, data.spark.influence, data.spark.token, hitActor);
    }
    flowcollision.targets.Clear();
}
```

`SparkPerTarget` 将当前命中的 actor 注入 Spark 指令的目标解析上下文，`DamageData` 使用 `[HIT_TARGETS]` 规则链即可正确解析。

---

## 五、规则链高级特性（Phase 3）

### 5.1 条件规则

```csharp
public enum TargetRuleType : byte
{
    SELF = 0,
    OWNER = 1,
    OWNER_OF_TYPE = 2,
    HIT_TARGETS = 3,
    
    // Phase 3 新增
    FILTER_ALIVE = 4,     // 过滤已死亡单位
    FILTER_TAG = 5,       // 按 Tag 过滤（如 "enemy", "ally"）
    FILTER_DISTANCE = 6,  // 按距离过滤（最近/最远 N 个）
    FIRST = 7,            // 取第一个结果
    RANDOM = 8,           // 随机取一个
}
```

### 5.2 使用示例

```csharp
// 找到最近的存活的敌人（AoE 技能自动索敌）
var nearestEnemy = new TargetChain
{
    rules =
    {
        TargetRule.HitTargets(),
        TargetRule.FilterAlive(),
        TargetRule.FilterTag("enemy"),
        TargetRule.FilterNearest(1),
        TargetRule.First(),
    }
};
```

### 5.3 Debug 支持

规则链是可序列化的数据结构，天然便于 Debug：

- 日志输出每一步的中间结果：`[SELF] flow.actor=123 → [OWNER] magic=456 → [OWNER] caster=789`
- 可视化编辑器中展示为节点链图
- 单元测试可直接构造 `TargetChain` 验证解析结果

---

## 六、涉及文件清单

| 文件 | 改动类型 | 说明 |
|------|---------|------|
| `FLOW_DEFINE.cs` | 保留（兼容） | ET_ 常量继续存在 |
| `Flows/Defines/` 或新建 | **新增** | `TargetChain.cs` + `TargetRule.cs` + 枚举定义 |
| `InstructData.cs` | 修改 | 新增 `targetChain` 字段 |
| `Flow.cs` | 修改 | `ExecuteInstruct` 增加 `ResolveAndExecute` 路径 |
| `Stage.cs` | 修改 | 新增 `GetOwner()` / `GetOwnerOfType()` 辅助 |
| `CollisionExecutor.cs` | 修改（Phase 2） | `SparkPerTarget` 逐目标触发 |
| `Flow.cs` (Spark) | 新增 | `SparkPerTarget(flowinfo, influence, token, targetActor)` |
| `S10010.cs` | 迁移示例 | Phase 2 时改为规则链 |
| `S10020.cs` | 迁移示例 | Phase 2 时改为规则链 |

---

## 七、风险与注意事项

1. **序列化兼容**：`TargetChain` 使用 MessagePack，新增字段不影响旧数据反序列化（`et` 字段保留）
2. **帧同步安全**：规则链解析必须使用确定性 API（`stage.SeekBehaviorInfo`、`stage.cache.Valid`），不引入非确定性操作
3. **性能**：规则链长度通常 ≤ 3，解析开销可忽略。`HIT_TARGETS` 多目标展开与当前 `ET_FLOW_HIT` 的 foreach 等价
4. **回滚兼容**：旧脚本不设 `targetChain` 时完全走 `et` 逻辑，零风险
5. **SparkPerTarget 的正确性**：需确保每个目标的 spark 指令独立执行，避免状态污染

---

## 八、总结

| 维度 | 当前 `byte et` | 规则链 `TargetChain` |
|------|---------------|---------------------|
| 表达能力 | 4 种硬编码 | 无限可组合 |
| 扩展性 | 改 3 处代码 | 只加规则类型枚举 |
| 上下文感知 | 否 | 是（起点可在 FLOW_ACTOR / FLOW_OWNER 间选择） |
| 脚本复用 | 与 Actor 层级耦合 | 规则链声明意图，与层级解耦 |
| Debug 可见性 | 看一个字节 | 完整规则链可日志输出 |
| 序列化体积 | 1 byte | 通常 3-6 bytes |
| 迁移风险 | N/A | 零（et 保留，targetChain 可选） |
