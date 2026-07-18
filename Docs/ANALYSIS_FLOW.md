# Flow.cs 深度分析

> 2026-07-19 | 491 行 | 逐路径追踪 + 正确性验证

## 一、数据模型

```
FlowInfo (per Actor)
  owner: ulong                    — 管线拥有者 ActorID
  pipelines: List<uint>           — 管线数据 ID 列表（指向 PipelineData）
  length: ulong                   — 管线总时长 (ms) = max(instruct.end)
  timeline: ulong                 — 当前播放位置 (ms)
  framepass: ulong                — 累计帧时间 (ms)，受 owner timescale 影响
  doings: Dict<pipelineid, List<index>> — 正在执行的指令追踪

PipelineData (管线蓝图，全局缓存)
  length: ulong                   — 指令最大 end
  instructs: List<Instruct>       — 按 begin 排序的常规指令
  sparkinstructs: List<SparkInstruct> — 事件触发指令

Instruct (单条指令)
  begin/end: ulong                — 时间区间
  checkonce: bool                 — 条件只检查一次
  conditions: List<Condition>     — 前置条件
  data: InstructData              — 指令数据 { id, et }
```

## 二、核心路径逐条追踪

### 路径 1: 管线创建 → 首帧执行

```
GenPipeline(owner, pipelines)
  → spawn FlowActor → FlowPrefab.OnProcessing:
      active=true, 设置 owner/pipelines/length
  → Gen2RunPipeline:
      RunPipeline(flowinfo)
        → 遍历 instructs, timeline=0
        → 所有 begin=0 的指令: Enter + Execute
      Spark(PIPELINE_GEN)
        → 遍历所有 FlowInfo → 匹配 sparkinstructs token=TOKEN_PIPELINE_GEN
        → Enter + Execute + Exit (同步，一次性)
```

**验证**: 正确。Spark(PIPELINE_GEN) 触发的是同 actor 上其他管线的火花指令，用于"管线创建时联动"。Enter+Execute+Exit 三连意味着火花指令不留 doings 痕迹。

### 路径 2: 正常 Tick 流程

```
OnTick(tick):
  foreach FlowInfo:
    framepass += tick × owner.timescale   // 帧时间累积
    queue.Enqueue(flowinfo)
  
  while queue.TryDequeue:
    if framepass >= LOGIC_TICK_MS:
      timeline += LOGIC_TICK_MS          // 推进一个逻辑帧
      framepass -= LOGIC_TICK_MS
      RunPipeline(flowinfo)
        → 遍历 instructs (按 begin 排序):
          - 不在时间区间: Exit (if isdoing)
          - checkonce + isdoing: Execute (不重查条件)
          - 首次: CheckCondition → Enter + Execute（失败则记录到 insidenotexebacks）
      if framepass 还够 → re-queue

OnEndTick:
  InsideNotExeToExecute (重试条件不满足的指令)
  遍历 FlowInfos: timeline >= length → EndPipeline
```

**验证**: 正确。
- 全部 FlowInfo 先累积 framepass 再逐个处理——新管线不会在同一 Tick 被处理 ✓
- 条件失败不阻塞——记录到 backs，OnEndTick 重试 ✓
- re-queue 正确按 timeline 逐帧推进 ✓

### 路径 3: EndPipeline 清理

```
EndPipeline(flowinfo):
  foreach pipeline in pipelines:
    copy doings[pipeline] indexes → 遍历副本:
      ExecuteInstruct(Exit, ...)
        → Do(target): indexes.Remove(index) ← 移除原始 list 中的条目
    recycle 副本
  RmvActor(flowinfo.actor)
```

**验证**: 先拷贝再遍历——Exit 回调即使修改 doings 也不会破坏迭代。正确 ✓

### 路径 4: InsideNotExeToExecute 递归

```
InsideNotExeToExecute():
  swap(fronts, backs)           // backs 变 fronts，fronts 变 backs
  遍历 fronts:
    CheckCondition → Enter + Execute
  fronts.Clear()
  if backs.Count > 0: 递归
```

**递归何时触发**: Execution 中 Spark → GenPipeline → RunPipeline → 条件失败 → 写入 backs。
**是否会无限递归**: 否。RunPipeline 写入 backs 的条件失败指令数量有限，每层递归处理有限条目，最终 backs 为空。
**无递归上限保护**: 理论上恶意管线可构造长链，但实际管线人工编写，可控。

⚠️ **低风险**: 条件持续不满足时，指令在 RunPipeline→InsideNotExeToExecute 间来回，但每 Tick 最多处理一次。

### 路径 5: Spark 跨管线事件

```
Spark(actor, token):
  遍历所有 FlowInfo (O(F)):
    遍历 pipelines (O(P)):
      遍历 sparkinstructs (O(S)):
        匹配 token + influence + actor
        CheckCondition → Enter + Execute + Exit (三连)
```

**索引计算**:
```csharp
uint index = (uint)data.instructs.Count + (uint)i + 2;
```
- 常规指令索引: 1..N（RunPipeline 中从 1 开始递增）
- 火花指令索引: N+2, N+3, ...
- 间隙 N+1 是故意的，为可能的扩展留空
- 每个 pipeline 独立 doings 空间，索引不跨 pipeline 碰撞 ✓

**Spark 再入**: Spark → ExecuteInstruct → Executor → Spark → ... 可形成链式触发。
- 深度受管线配置限制，人工编写不可无限循环 ✓
- doings 不受影响——火花指令 Enter+Exit 成对，净效果为零 ✓

## 三、已确认的问题

### P0: InsideNotExeToExecute 无递归上限

```csharp
if (0 != insidenotexebacks.Count) InsideNotExeToExecute();
```

虽然实际不会无限递归，但无硬性上限。极端情况下（如 100 条指令条件同帧失败），可导致深度递归。

**修法**: 加 `maxDepth` 参数，默认 10。

### P1: ET_FLOW_HIT — doings per-target 追踪自净

```
Spark → ET_FLOW_HIT → foreach(target) Do(target.actor)
  Do → Enter: indexes.Add(index)  ← N 个 target 各 Add 一次
  Do → Exit:  indexes.Remove(index) ← N 个 target 各 Remove 一次 → 自净 ✓
```

同一 index N 次 Add/N 次 Remove——净效果为零。不改。

### P2: Spark 遍历效率 O(F×P×S)

三层嵌套循环，每次都从头扫所有 instruct 的 sparkinstructs。
当前流量下可接受，但管线数和火花指令数增长后成平方退化。

**修法**: 按 token 建索引 `Dictionary<string, List<(pipeline, SparkInstruct)>>`，O(1) 定位。

### P3: RunPipeline 每帧全量扫描

即使 timeline 已超过大部分 instruct 的 end，每帧仍从 index=0 开始遍历，遇 begin > timeline 才 break。指令多的管线有浪费。

**修法**: 记录 `lastCompletedIndex`，跳过已结束的指令。

## 四、ET_FLOW_HIT — 设计矛盾与解决方案

### 矛盾

`CheckCondition` 放在 `RunPipeline` 里——它不知道 targets 是谁。而 `ExecuteInstruct` 知道 targets（通过 `data.et`），但它是个调度器，不该管条件。

```
RunPipeline:            CheckCondition(flowinfo) → 不知道 target
ExecuteInstruct:        foreach target in targets → 知道 target，但条件已查完
```

ET_FLOW 和 ET_FLOW_OWNER 只有一个 target，矛盾不显现。ET_FLOW_HIT 有 N 个 target，每个 target 可能需要不同条件——比如"只伤害敌方"需要查 target 的阵营。这种 per-target 条件只能在 ExecuteInstruct 内部查，因为只有它知道 targets。

所以 `// HACKER` 不是冗余代码——它是在没有更好架构的情况下，正确放置的逻辑。问题是它让 `ExecuteInstruct` 有了双重职责。

### 当前 CheckCondition 局限

```csharp
bool CheckCondition(InstructData data, List<Condition> conditions, FlowInfo flowinfo)
//                                                                    ^^^^^^^^  没有 target
```

`InputChecker` 读的是 `flowinfo.owner` 的 Gamepad——与 target 无关。今天 per-target 条件不存在，但设计需要支持。

### 方案：CheckCondition 加 target 参数

```csharp
// Step 1: CheckCondition 增加 target 参数
private bool CheckCondition(InstructData data, List<Condition> conditions,
    FlowInfo flowinfo, ulong target)
{
    foreach (var condition in conditions)
    {
        if (false == checkers.TryGetValue(condition.id, out var checker))
            throw new Exception($"id : {condition.id} cannot find checker.");
        if (false == checker.Check(condition, flowinfo, target)) return false;
    }
    return true;
}

// Step 2: Checker 接口加 target（默认值 0，现有 Checker 不受影响）
public abstract class Checker
{
    public abstract bool Check(Condition condition, FlowInfo flowinfo, ulong target = 0);
}

// Step 3: ExecuteInstruct 中 ET_FLOW_HIT 改为 per-target 检查
case FLOW_DEFINE.ET_FLOW_HIT:
    if (false == stage.SeekBehaviorInfo(flowinfo.actor, out FlowCollisionHurtInfo flowcollision)) break;
    foreach (var target in flowcollision.targets)
    {
        if (ExecuteInstructType.Exit != type &&
            false == CheckCondition(data, conditions, flowinfo, target.actor))
            continue;
        Do(target.actor);
    }
    break;

// Step 4: RunPipeline 调用 CheckCondition 时传 flowinfo.actor（行为不变）
if (false == CheckCondition(instruct.data, instruct.conditions, flowinfo, flowinfo.actor))
```

### 两阶段落地

**现在**：
- 加 `target` 参数到 `CheckCondition` 和 `Checker.Check`（默认值 0）
- 现有 Checker 不读 target，行为不变
- `// HACKER` 注释去掉——per-target checking 是正确的架构

**以后**：
- 实现"只伤害敌方"Condition 时，Checker.ReadCondition(target) 读 target 的阵营
- per-target 过滤自然生效

### 遗留问题

RunPipeline 外层也调了 `CheckCondition`（对所有 ET 类型）。ET_FLOW_HIT 时，外层通过不等于 per-target 全通过——但当前无 per-target 条件，两层永远等值。未来有 per-target 条件后，外层相当于一个"跳过全部无用的"快速通道；真正的过滤在 ExecuteInstruct 内。insidenotexebacks 记录外层失败——不会漏掉 per-target 失败的重试（因为外层通过才会进 ExecuteInstruct）。

## 五、正确性总结

| 路径 | 状态 | 备注 |
|------|------|------|
| 管线创建→首帧 | ✅ | 正确 |
| Tick 驱动 + re-queue | ✅ | timeline 正确推进 |
| 条件失败→重试 | ✅ | 双缓冲正确，最多丢失一个 Tick |
| EndPipeline 清理 | ✅ | 先拷贝再遍历，安全 |
| Spark 跨管线 | ✅ | 再入可控，doings 净效果为零 |
| ET_FLOW_HIT | ⚠️ | CheckCondition 缺 target 参数，加后 HACKER 变正确架构(§四) |
| 递归保护 | ⚠️ | 无上限，实际可控 |
