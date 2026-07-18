# 已完成功能验证

> 2026-07-19 | 6 项 ⭐ → ✅

## 1. 死亡流程

**链路**：`DamageExecutor` → `AttributeBucket.ToDamage` → `SilentMercy.Kill` → `Dead` → `RmvActor`

```
DamageExecutor.Apply()
  → stage.attrb.ToDamage(from, target, damage)
    → HP -= damage
    → if HP <= 0: stage.silentmercy.Kill(from, to)
      → 检查目标是否已经 DEATH
      → 记录 kill/victim 关系到 CareerInfo
      → Dead(victim)
        → 有 deathpipelines → GenPipeline（死亡动画/特效）
        → 无 deathpipelines → 直接 RmvActor(deadman)
```

**死亡防护**：`HitLagExecutor`、`BeHitExecutor`、`AttributeBucket.ToDamage` 都在入口检查 DEATH 状态并提前返回。

**终端状态**：DEATH 在 `STATE_DEFINE.PASSES` 中无合法转换目标，进入后无法退出。

**相关文件**：
- `Flows/Executors/DamageExecutor.cs`
- `Behaviors/Sa/AttributeBucket.cs` (`ToDamage`)
- `Behaviors/Sa/SilentMercy.cs`
- `BehaviorInfos/Sa/SilentMercyInfo.cs`
- `BehaviorInfos/CareerInfo.cs`
- `Flows/Executors/RmvActorExecutor.cs`
- `Core/Stage.cs` (`RmvActor`)

## 2. 状态机

**10 个状态**：NONE, BORN, DEATH, IDLE, MOVE, JUMP, FALL, CASTING, BEHIT, ROLL

**转换规则**：`STATE_DEFINE.PASSES` 字典，每个状态明确定义合法目标：

| 当前 | 可转换至 |
|------|---------|
| IDLE | DEATH, MOVE, FALL, CASTING, BEHIT, ROLL |
| MOVE | DEATH, IDLE, FALL, CASTING, BEHIT, ROLL |
| JUMP | DEATH, FALL, CASTING, BEHIT |
| FALL | DEATH, IDLE, CASTING, BEHIT |
| CASTING | DEATH, BEHIT |
| BEHIT | DEATH, BEHIT |
| ROLL | DEATH, IDLE, MOVE, CASTING |
| BORN | 无（终端） |
| DEATH | 无（终端） |

**API**：
- `ChangeState(state)` — 直接切换
- `TryChangeState(state)` — 检查 PASSES 后切换
- `Break()` / `Break(delay)` — 中断当前状态，延迟可选

**管线和动画集成**：`ChangeStateExecutor` 通过管线指令驱动状态切换，切换时自动设动画状态到 RIL。

**相关文件**：
- `Behaviors/StateMachine.cs`
- `BehaviorInfos/StateMachineInfo.cs`
- `Common/Defines/STATE_DEFINE.cs`
- `Flows/Executors/ChangeStateExecutor.cs`

## 3. Flow 事件执行

**三阶段生命周期**：Enter → Execute → Exit

**三目标模式**（`data.et`）：
- `ET_FLOW` — 管线 Actor 自身
- `ET_FLOW_OWNER` — 管线拥有者
- `ET_FLOW_HIT` — 碰撞命中的每个目标

**调度逻辑**（`RunPipeline`）：
```
instruct 不在时间区间 → Exit
instruct 在时间区间 + checkonce + isdoing → Execute only
instruct 在时间区间 + 首次 → CheckCondition → Enter + Execute
```

**15 个已注册 Executor**：Animation, SpatialPosition, CreateMagic, MagicMotion, LaunchSkill, Effect, Collision, RmvActor, ChangeState, Spark, HitLag, TimeScale, BeHit, SkillBreak, Damage

**相关文件**：
- `Behaviors/Sa/Flow.cs`
- `Flows/Executors/Common/Executor.cs`
- `Flows/Executors/Common/InstructData.cs`

## 4. Flow 事件派发

### Spark 系统（管线内事件）

```
Spark(actor, token)
  → 遍历所有 FlowInfo × pipelines × sparkinstructs
    → 匹配 token + influence + actor
    → CheckCondition
    → Enter + Execute + Exit（同步三连，不留 doings 痕迹）
```

**令牌**：`SPARK_INSTR_DEFINE.TOKEN_PIPELINE_GEN` — 管线生成时触发联动

**触发方式**：`SparkExecutor` 在管线内显式调用；`CollisionExecutor` 碰撞后自动触发火花

### Eventor 系统（全局事件）

类型化 pub/sub：`Listen<T>()` → `Tell<T>()` → `UnListen<T>()`

当前用途：`ActorBornEvent`、`ActorRmvEvent`（AttributeBucket 监听以清理属性）

**相关文件**：
- `Behaviors/Sa/Flow.cs` (`Spark`)
- `Behaviors/Sa/Eventor.cs`
- `BehaviorInfos/Sa/EventorInfo.cs`
- `Flows/Executors/SparkExecutor.cs`
- `Flows/Executors/Common/SparkInstruct.cs`

## 5. 顿帧

```
HitLagExecutor.OnEnter
  → 检查目标未死亡
  → 计算 strength × (1 + (targetcnt-1) × additivefactor)，clamp 到 max
  → stage.hiteffect.AddHitLag(target, strength, duration)
    → 创建 HitLagInfo（引用计数 count++）
    → 保存原始 timescale
    → TickerInfo.timescale -= strength  ← 时间减速
    → OnTick 累计 elapsed → 到期 RmvHitLag
      → count-- → 归零时恢复原始 timescale
```

**叠加模式**：`TYPE_ADDITIVE` — 多个命中叠加强度，factor 可配置

**相关文件**：
- `Flows/Executors/HitLagExecutor.cs`
- `Behaviors/Sa/HitEffect.cs`
- `BehaviorInfos/HitLagInfo.cs`
- `Flows/Executors/Instructs/HitLagData.cs`
- `Flows/Defines/HIT_LAG_DEFINE.cs`

## 6. 受击效果

**朝向锁定**（`uselookatattacker`）：目标旋转面向攻击者

**击退位移**（`usehitmotion`）：4 种方向模式

| 模式 | 方向 |
|------|------|
| SELF_FORWARD | 目标自身前方 |
| ATTACK_FORWARD | 攻击者前方 |
| ATTACKER_TO_SELF | 攻击者→目标连线 |
| SELF_TO_ATTACKER | 目标→攻击者连线（拉近） |

动画状态切换由 `ChangeStateExecutor` 单独处理，BeHit 只做空间变化——关注点分离。

**相关文件**：
- `Flows/Executors/BeHitExecutor.cs`
- `Flows/Executors/Instructs/BeHitData.cs`
- `Flows/Defines/BEHIT_DEFINE.cs`

## 代码中剩余 TODO

| 文件 | 内容 | 类型 |
|------|------|------|
| `AttributeBucket.cs:123` | 抗性计算（护甲/暴击/闪避） | 功能缺口 |
| `Flow.cs:213` | Spark 优化查找 | 性能 |
| `Config.cs:25` | 构建环境判断 | 构建设施 |
| `ChangeStateExecutor.cs:31` | 受击动画独立状态机 | 设计债 |
