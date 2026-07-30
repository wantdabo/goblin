# Gameplay 模块全面审计

> 状态：`Audit`
>
> 审计日期：2026-07-27
> 范围：`godot/Scripts/Goblin/Gameplay/` 全量
> 背景：RIL 改造阶段对 Logic/Render 全模块做代码审查；本文是阶段性记录，当前实现以 [../ARCHITECTURE.md](../ARCHITECTURE.md) 为准。

---

## 一、模块概览

| 模块 | 文件数 | 职责 |
|------|--------|------|
| `Logic/Core` | 3 | Stage、Behavior、BehaviorInfo 引擎核心 |
| `Logic/BehaviorInfos` | 28 | Behavior 数据定义（含 Flows、Sa 子模块） |
| `Logic/Behaviors` | 22 | Behavior 逻辑实现（含 Sa 全局行为） |
| `Logic/Flows` | 61 | 流程系统（Executors、Checkers、Scriptings、Pipeline） |
| `Logic/Translators` | 0 | ✅ **已清理**（RIL 体系移除，UID 残留文件已删除） |
| `Logic/Commands` | 11 | 命令系统（输入指令 + Solider 执行器） |
| `Logic/Prefabs` | 10 | Actor 预制创建器 |
| `Logic/Common` | 45 | 公共：BuildDatas、Defines、Math（FPVector3 等）、GBL 容器 |

---

## 二、严重 Bug（运行时错误 / 数据损坏）

### B1【严重】`PreFab.OnProcessing` 类型转换无 null 检查

**位置**：`Logic/Prefabs/Common/Prefab.cs:89`

```csharp
protected override void OnProcessing(ulong actor, PrefabInfoState state)
{
    if (stage.SeekBehavior(actor, out Tag tag)) tag.Set(TAG_DEFINE.ACTOR_TYPE, type);
    OnProcessing(actor, (state as PrefabInfoState<T>).info);  // ← as 可返回 null
}
```

**问题**：若 `state` 不是 `PrefabInfoState<T>` 实例，`as` 返回 null，`.info` 抛 NRE。

**修复**：
```csharp
if (state is not PrefabInfoState<T> typed) return;
OnProcessing(actor, typed.info);
```

---

### B2【严重】`Solider<T>.OnExecute` 无 null 检查

**位置**：`Logic/Commands/Common/Solider.cs:57`

```csharp
public override void Execute(Command command)
{
    base.Execute(command);
    OnExecute(command as T);  // ← as 可返回 null
}
```

**问题**：类型不匹配时 `command as T` 返回 null。`TimeScaleSolider.OnExecute` 直接 `command.timescale` → NRE。

**修复**：`as` 后 null 检查，或 `if (command is T typed) OnExecute(typed);`

---

### B3【严重】`Detection.Linecast` 零向量产生 NaN

**位置**：`Logic/Behaviors/Sa/Detection.cs:350-351`

```csharp
FPVector3 dire = end - start;
return Raycast(start, dire.normalized, dire.magnitude, layer);
```

**问题**：`start == end` 时 `dire` 为零向量，`.normalized` 产生 NaN。

**修复**：零向量守卫：
```csharp
FPVector3 dire = end - start;
if (FPVector3.zero == dire) return new HitResult();
return Raycast(start, dire.normalized, dire.magnitude, layer);
```

---

### B4【严重】`SkillLauncher.OnTick` foreach+break 丢弃技能指令

**位置**：`Logic/Behaviors/SkillLauncher.cs:70-74`

```csharp
if (false == info.casting && stage.SeekBehavior(actor, out Gamepad gamepad))
{
    foreach (var skillcmd in gamepad.skills)
    {
        Launch(skillcmd.skillid);
        break;   // ← 只 consume 第一个，其余指令永久丢失
    }
}
```

**问题**：多个技能指令入队时，只取第一个，其余在 `OnEndTick` 被清空时永久丢弃。

**修复**：从列表移除已消费指令，或改为 while 消费所有。

---

### B5【严重】`Detection.Overlap` 命中时旧碰撞列表泄漏

**位置**：`Logic/Behaviors/Sa/Detection.cs:165-175`

```csharp
if (result.hit)
{
    List<...> colliders = ObjectCache.Ensure<...>();
    foreach (var c in result.colliders)
    {
        if (collider.actor == c.actor) continue;
        colliders.Add(c);
    }
    result.colliders = colliders;       // ← 旧 colliders 未归还对象池
    stage.cache.AutoRecycle(colliders);
}
```

**问题**：每次命中 `result.colliders` 被替换为新列表，旧列表演失且不回收 → 内存泄漏。

**修复**：替换前将旧列表归还对象池：
```csharp
if (null != result.colliders) ObjectCache.Set(result.colliders);
result.colliders = colliders;
```

---

### B6【严重】`SilentMercy.OnEndTick` 未清理 victimrelations

**位置**：`Logic/Behaviors/Sa/SilentMercy.cs:88-98`

```csharp
protected override void OnEndTick()
{
    base.OnEndTick();
    // 清空击杀列表
    foreach (var kv in info.killrelations)
    {
        kv.Value.Clear();
        ObjectCache.Set(kv.Value);
    }
    info.killrelations.Clear();
    // ← victimrelations 未清空！
}
```

**问题**：`victimrelations` 持续累积，`AskVictim` 返回过期关系。

**修复**：添加 `info.victimrelations.Clear();`

---

## 三、中等 Bug（逻辑问题 / 资源泄漏）

### B7【中】`Stage.Dispose` 非 Stopped 状态静默跳过

**位置**：`Logic/Core/Stage.cs:193`

```csharp
public void Dispose()
{
    if (StageState.Stopped != info.state) return;
    ...
}
```

**问题**：`Initialized`/`Paused` 状态下 Dispose 静默跳过，无日志无异常，调用方以为已销毁但资源未释放。

**修复**：至少打 Warning 日志，或改为 `Stop() + Dispose()` 链式调用。

---

### B8【中】`Stage.Restore` if 条件内嵌赋值

**位置**：`Logic/Core/Stage.cs:327`

```csharp
if (false == cache.behaviorinfodict.TryGetValue(behaviorinfo.actor, out var dict))
    cache.behaviorinfodict.Add(behaviorinfo.actor, dict = ObjectCache.Ensure<...>());
dict.Add(behaviorinfo.GetType(), behaviorinfo);
```

**问题**：`dict = ObjectCache.Ensure<...>()` 嵌在 if 内，极难阅读且容易误以为 `dict.Add` 仍在 if 内。

**修复**：拆分：
```csharp
if (false == cache.behaviorinfodict.TryGetValue(behaviorinfo.actor, out var dict))
{
    dict = ObjectCache.Ensure<GBLDict<Type, BehaviorInfo>>();
    cache.behaviorinfodict.Add(behaviorinfo.actor, dict);
}
dict.Add(behaviorinfo.GetType(), behaviorinfo);
```

---

### B9【中】`PipelineDataSerialize` sparkinstruct.conditions 重复赋值

**位置**：`Logic/Flows/PipelineDataSerialize.cs:131,135`

```csharp
var sparkinstruct = new SparkInstruct
{
    ...
    conditions = new GBLList<Condition>(),   // 行 131：对象初始化器创建
};
sparkinstruct.conditions = new GBLList<Condition>();  // 行 135：立即覆盖
```

**问题**：行 131 的赋值完全无效，紧接着被行 135 覆盖。

**修复**：删除行 131 的 `conditions = new GBLList<Condition>(),`

---

### B10【中】`BytesToInstructData`/`BytesToCondition` 未知类型返回 null

**位置**：`Logic/Flows/PipelineDataSerialize.cs:185,202`

```csharp
default:
    return null;  // ← 未知类型静默返回 null
```

**问题**：调用方 `ToPipelineData` 不检查 null 直接赋值 `instruct.data = ...` / `condition = ...`，后续使用必 NRE。

**修复**：`default` 抛异常或打错误日志。

---

### B11【中】`Stage.AddBehavior` type 非 Behavior 子类未检查

**位置**：`Logic/Core/Stage.cs:676`

```csharp
var behavior = ObjectCache.Ensure(type) as Behavior;
// ... behavior.Assemble(...) // ← behavior 可能为 null
```

**问题**：若 `type` 不是 `Behavior` 子类，`as` 返回 null，`Assemble` NRE。

**修复**：添加 null 检查。

---

## 四、风格违规

| # | 文件 | 行 | 问题 | 规范要求 | 状态 |
|---|------|-----|------|----------|------|
| S1 | `Behaviors/HUD.cs` | 全文 | Tab 缩进 | 4 空格 | ✅ 已修复 |
| S2 | `Behaviors/Tag.cs` | 全文 | Tab 缩进 | 4 空格 | ✅ 已修复 |
| S3 | `Behaviors/Facade.cs` | 全文 | Tab 缩进 | 4 空格 | ✅ 已修复 |
| S4 | `BehaviorInfos/TickerInfo.cs` | 13 | `private FP mtimescale` — `m` 前缀 | 禁止前缀 | |
| S5 | `BehaviorInfos/Sa/SilentMercyInfo.cs` | 19 | `deadths` 拼写错误 | `deaths` | |
| S6 | `Behaviors/Sa/Flow.cs` | 232 | `null != data` | `null == data` 反转 | |
| S7 | `Behaviors/Sa/Detection.cs` | 207,261,311 | `-1 != layer` | `-1 == layer` 反转 | |
| S8 | `Behaviors/Sa/ProjectorSystem.cs` | 51 | `info is not IProjectable` | 禁止 `not` 模式匹配 | |
| S9 | `Behaviors/StateMachine.cs` | 123 | `info.delaybreak <= FP.Zero` | FP 比较常量在左 | |
| S10 | `BehaviorInfos/Sa/RandomInfo.cs` | 39-40 | `OnReady`/`OnReset` 中 `seed = 0; current = 0;` | 冗余（已是 default） | |
| S11 | `BehaviorInfos/Sa/SilentMercyInfo.cs` | 23 | `killrelations` vs `victimrelations` | 命名不对称，建议 `killerrelations` | |

---

## 五、设计问题

| # | 文件 | 问题 |
|---|------|------|
| D1 | `BehaviorInfos/SkillCooldownInfo.cs` | 空类，无属性，缺少 TODO/说明注释 |
| D2 | `BehaviorInfos/Flows/FlowCollisionSensorInfo.cs` | 空子类，缺少意图说明注释 |
| D3 | `Behaviors/Sa/StageSequence.cs:Finish(bool win)` | `win` 参数未使用，胜负结果无任何后续逻辑 |
| D4 | `Behaviors/Sa/Config.cs:location` | 属性名 `location` 但类型是 `Tables`，语义不匹配 |
| D5 | `Behaviors/Sa/ProjectorSystem.cs:CollectContainerDiffs` | 空方法体带 3 个未使用参数，编译器警告（CS0219） |
| D6 | `Behaviors/Sa/Detection.cs` | ~960 行单文件，建议拆分为 `Detection.cs` + `DetectionAlgorithms.cs` |
| D7 | `Behaviors/Sa/AttributeBucket.cs:ToDamage` | 直接读 `StateMachineInfo.current`，绕过 `StateMachine` Behavior 封装 |
| D8 | `Behaviors/Sa/Detection.cs:714` | `Raycast(Sphere)` 声明为 `static`，同类的 Box Raycast 是实例方法——不一致 |
| D9 | `Behaviors/HUD.cs:OnAssemble` | 注释说"同步初始值避免首帧 HUD 为空"，但方法体仅有 `base.OnAssemble()`，**未实现** |
| D10 | `Flows/PipelineData.cs:32-35` | `Format()` 实例方法未调用且不完整（无 length 计算），疑似死代码 |

---

## 六、残留 / 死文件

| # | 路径 | 状态 |
|---|------|------|
| R1 | `Logic/Translators/ActorTranslator.cs.uid` | ✅ 已删除（整个 Translators 目录已移除） |
| R2 | `Logic/Translators/AttributeTranslator.cs.uid` | ✅ 同上 |
| R3 | `Logic/Translators/FacadeAnimationTranslator.cs.uid` | ✅ 同上 |
| R4 | `Logic/Translators/FacadeEffectTranslator.cs.uid` | ✅ 同上 |
| R5 | `Logic/Translators/FacadeModelTranslator.cs.uid` | ✅ 同上 |
| R6 | `Logic/Translators/SeatTranslator.cs.uid` | ✅ 同上 |
| R7 | `Logic/Translators/SpatialTranslator.cs.uid` | ✅ 同上 |
| R8 | `Logic/Translators/StageTranslator.cs.uid` | ✅ 同上 |
| R9 | `Logic/Translators/StateMachineTranslator.cs.uid` | ✅ 同上 |
| R10 | `Logic/Translators/TickerTranslator.cs.uid` | ✅ 同上 |
| R11 | `Logic/Translators/Common/Translator.cs.uid` | ✅ 同上 |
| R12 | `Render/Core/RenderWorld.cs` | ✅ 已删除（文件不再存在） |

---

## 七、建议修复优先级

| 优先级 | 条目 | 影响 |
|--------|------|------|
| **P0** | B1 (Prefab NRE)、B2 (Solider NRE)、B3 (Linecast NaN) | 运行时必崩 |
| **P1** | B4 (SkillLauncher 丢指令)、B5 (Detection 泄漏)、B6 (SilentMercy 未清理) | 数据损坏/内存泄漏 |
| **P1** | B7 (Stage.Dispose 静默泄漏) | 资源泄漏 |
| **P2** | B8-B11（可读性/健壮性） | 维护隐患 |
| **P2** | ~~R1-R11（Translators UID 残留）~~ | ✅ 已清理 |
| **P3** | S1-S11（风格违规） | 代码一致性 |
| **P3** | D1-D10（设计改进）、~~R12~~ | ✅ R12 已删除 |

---

## 八、复查状态（2026-07-28 复查）

| 类别 | 总数 | 已修复 | 未修复 |
|------|------|--------|--------|
| 严重 Bug（B1-B6） | 6 | 0 | 6 |
| 中等 Bug（B7-B11） | 5 | 0 | 5 |
| 残留文件（R1-R12） | 12 | 12 | 0 |
| 风格违规（S1-S11） | 11 | 3 | 8 |
| 设计问题（D1-D10） | 10 | 0 | 10 |
| **合计** | **44** | **15** | **29** |

> 2026-07-28 复查：S1-S3（HUD/Tag/Facade Tab 缩进）已在 GBL 容器重构 + Reset 全线优化中附带修复。其余 Bug / 风格 / 设计问题均未处理。
