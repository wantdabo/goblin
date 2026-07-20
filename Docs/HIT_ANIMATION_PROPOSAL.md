# 动画槽位优先级方案

> 2026-07-19 初稿 | 2026-07-21 七轮迭代：泛化解耦 + 命名对齐 | 解耦 `ChangeStateExecutor` 中 HITSTUN 的 hack，建立统一的 Facade 动画优先级模型

---

## 一、现状：三条动画路径共占一个 Facade

系统已有两条动画驱动路径，再加受击就是第三条，三者在 `Facade` 上共存但**没有优先级模型**。

### 1.1 路径 A：StateMachine → animstate（持久基态）

```
ChangeStateData → ChangeStateExecutor
  → StateMachine.ChangeState(state)
    → facade.SetAnimation(state)         // FacadeInfo.animstate = IDLE/MOVE/CASTING/DEATH
```

生命周期：持久，直到下次状态切换。渲染层通过 AnimationConfig 将 `animstate (byte)` 映射为动画名（如 `3 → "idle_01"`）。

### 1.2 路径 B：AnimationData → animname（管道定时命名动画）

```
AnimationData(begin, end) → AnimationExecutor
  OnEnter:  facade.SetAnimation(name, TICK_MANUAL)   // FacadeInfo.animname = "charge_loop"
  OnExecute: facade.info.animelapsed += LOGIC_TICK
  OnExit:   facade.SetAnimation(null, TICK_AUTOMATIC) // 清除 animname
```

生命周期：Pipeline instruct begin→end。name 直接传给渲染层，不走查表。

### 1.3 路径 C：BeHit → ？？？（受击动画，当前缺失）

```
BeHitData → BeHitExecutor
  OnEnter: 仅做朝向+击退位移
  // 没有驱动任何动画！HITSTUN 状态从未被设置
```

### 1.4 攻击 → 受击流程（以 S10020 重击为例，ET 搜索已修复）

> **2026-07-21 修复**：BeHitData/HitLagData/DamageData 原本为帧 200 instruct，与 Collision 同帧但 spark→Clear 后才执行，ET 搜索始终命中空集。已迁移为 spark instruct（`SPARK_INSTR_DEFINE.TOKEN_ON_HIT`）。

```
S10020 Pipeline (t=200ms)
├── CollisionData (碰撞检测)    → 命中目标写入 flowcollision.targets
│   └── Spark(TOKEN_ON_HIT)    → targets 存活期间同步执行命中响应链
│       ├── BeHitData (受击位移) → 转身面对攻击者 + 击退位移（无动画！）
│       ├── HitLagData (顿帧)    → 双方暂停 frames
│       └── DamageData (伤害)    → 扣血
│   └── targets.Clear()
```

### 1.5 问题：三路径无优先级模型

| 场景 | animstate | animname | 期望 | 现状 |
|------|-----------|----------|------|------|
| 施法中被重击 | CASTING | "charge_loop" | HITSTUN | "charge_loop"（路径 C 不存在） |
| 移动中被轻击 | MOVE | null | HITSTUN | MOVE（路径 C 不存在） |
| 受击硬直中死亡 | IDLE | null | DEATH | 需额外处理 |

### 1.6 ChangeStateExecutor 中的 Hack

```csharp
// ChangeStateExecutor.cs:31-33
// TODO : HACKER 这里是为了做受击动画, 后续受击动画需要改成独立的动画状态机来处理
if (statemachine.info.current == data.state) statemachine.Break();
statemachine.TryChangeState(data.state);
```

非 force 路径用 `Break()`→NONE→`TryChangeState` 绕路。**系统缺少一个独立于 StateMachine 的动画覆盖通道**。

---

## 二、根因：缺少统一的 Facade 动画优先级模型

| 问题 | 说明 |
|------|------|
| 无优先级定义 | 受击动画应该覆盖施法动画，但路径 C 缺失 |
| StateMachine 耦合 | HITSTUN 必须走 StateMachine，结束后不知道该回 IDLE 还是 MOVE |
| 生命周期冲突 | AnimationData 是 Pipeline 计时，HITSTUN 是独立计时，两者无协调机制 |
| 打断不完整 | CASTING 被 HITSTUN 覆盖后，Skill 逻辑层不知道被打断 |
| 同状态重入绕路 | `Break()`→NONE→`TryChangeState` 是 hack |

### 现有 State 体系（不变）

```
IDLE(3)   → DEATH, MOVE, FALL, CASTING, HITSTUN, ROLL
MOVE(4)   → DEATH, IDLE, FALL, CASTING, HITSTUN, ROLL
JUMP(5)   → DEATH, FALL, CASTING, HITSTUN
FALL(6)   → DEATH, IDLE, CASTING, HITSTUN
CASTING(7)→ DEATH, HITSTUN
HITSTUN(8)→ DEATH, HITSTUN
ROLL(9)   → DEATH, IDLE, MOVE, CASTING
```

---

## 三、行业参考

### 3.1 格斗游戏：统一状态机

状态对立，没有"叠加"。**不适合本项目**：空间可移动的俯视角 ARPG。

### 3.2 Souls-like / 怪猎：Layer + Slot 分层

高优先度覆盖低优先度，到期恢复自然回落。**最适合本项目**：逻辑层确定性帧同步 + RIL 传输天然适配。

### 3.3 UE Animation Blueprint：变量驱动

逻辑层写变量，动画蓝图自选。**部分借鉴**：当前 AnimationConfig JSON 已有状态→动画名映射。

### 3.4 本项目选型：AnimationSlot 优先级 + StateMachine 复用 + 双轨保留

```
                    Goblin 架构                              业内映射
                    ────────────                             ────────
逻辑层              StateMachine (逻辑状态 + 受击 duration)     Souls 逻辑状态机
                    BeHitExecutor (直接管理槽位)                 Souls Slot Override

Facade层 ★          AnimationSlot 槽位集合（双字段）
           ┌────────────────────────────────────────┐
           │  SLOT_STATE    pri=0   state=3  name=-│  ← animstate=IDLE，渲染层查表
           │  SLOT_NAMED    pri=200 state=0  name=+│  ← animname="charge_loop"，直传
           │  SLOT_HITSTUN    pri=400 state=8  name=-│  ← animstate=HITSTUN，BeHitExecutor 直接管理
           └────────────────────────────────────────┘

翻译层              FacadeAnimationTranslator（双字段原样灌入 RIL）
                    winner.animstate → ril.animstate
                    winner.animname  → ril.animname

渲染层              AnimationAgent / PrimitiveAnimAgent
                    ril.animname ?? GetAnimationName(ril.animstate)  ← 现有逻辑零改动
```

> **2026-07-21 决策**：受击硬直本质是 StateMachine 的 HITSTUN 状态，AnimationSlot 槽位由 BeHitExecutor 直接操作，duration 由 StateMachine 管理。零新增 Behavior。

**核心思路**：
- Slot 双字段：`animstate (byte)` 负责持久状态（IDLE/MOVE/HITSTUN），`animname (string)` 负责临时命名动画（"charge_loop"）
- RIL 双字段不变：`{ animstate, animname, animelapsed }` 全部保留
- state→name 映射留在 Render 层：AnimationConfig 照常工作，渲染层现有 fallback 逻辑 `animname ?? GetAnimationName(animstate)` 完美兼容
- **Render 层零改动**：AnimationAgent、PrimitiveAnimAgent、AnimationConfig 全部不动
- **零配置表新增**：不需要 `state2anim` 字典、不需要 `LoadStateMapping` 注入

> **2026-07-21 决策**：animstate 和 animname 是两种正交语义——持久状态 vs 临时指令。双字段是正确建模，不是妥协。

---

## 四、架构设计：动画槽位优先级系统

### 4.1 设计原则

- Slot 双字段：`animstate (byte)` + `animname (string)`。state 驱动写 state，name 驱动写 name
- state→name 解析不发生在 Logic 层——Render 层 AnimationConfig 已经有这套机制，无需在 Logic 层重复
- RIL 双字段不变，渲染层现有 fallback 逻辑不动
- **零注入、零配置**：Facade 不需要知道 AnimationConfig 的存在

```
新增动画来源的成本 = 注册一个 Slot + 定一个优先级数字。零 Translator + 零 Render 改动。
```

### 4.2 AnimationSlot 设计（双字段）

```
AnimationSlot
├── key         : int          // 槽位键（枚举）
├── priority    : int          // 优先级（越大越优先）
├── animstate   : byte         // 持久状态（STATE_DEFINE.IDLE/MOVE/HITSTUN/...），SLOT_NAMED 时为 0
├── animname    : string       // 命名动画（"charge_loop"/"hit_01"/...），SLOT_STATE/HITSTUN 时为 null
├── active      : bool         // 是否活跃
├── istransient : bool         // true=临时覆盖，false=持久
└── duration    : FP           // 临时槽位剩余时间
```

**优先级编号约定**：

```
1000+   系统接管级     Cutscene, ScriptedSequence
800-999 生命状态级     Death, Revive
600-799 硬控状态级     Frozen, Petrified, Stunned
400-599 受击反应级     BeHit, Knockdown, Launch, Grab
200-399 主动动作级     Attack, Skill, Dodge, Parry
100-199 交互动作级     Interact, Push, Climb
0-99    基础运动级     Idle, Walk, Run, Jump, Fall
```

### 4.3 Facade 槽位集合

```csharp
// FacadeInfo
public List<AnimationSlot> animslots { get; set; }  // 新增唯一字段

// Facade
public void AddOrUpdateSlot(ANIM_SLOT_KEY key, int priority, byte state = 0, string name = null)
public void RmvSlot(ANIM_SLOT_KEY key)
```

### 4.4 Translator 解析逻辑（通用，永不变）

```csharp
protected override int OnCalcHashCode(FacadeInfo info)
{
    int hash = 17;
    hash = hash * 31 + info.actor.GetHashCode();
    // 取最高优先级活跃槽位的 animstate + animname
    byte winnerstate = 0;
    string winnername = null;
    foreach (var slot in info.animslots)
    {
        if (false == slot.active) continue;
        winnerstate = slot.animstate;
        winnername = slot.animname;
        break;
    }
    hash = hash * 31 + winnerstate.GetHashCode();
    hash = hash * 31 + (null != winnername ? winnername.GetHashCode() : 0);
    hash = hash * 31 + info.animelapsed.GetHashCode();
    hash = hash * 31 + info.effectincrement.GetHashCode();

    return hash;
}

protected override void OnRIL(FacadeInfo info, RIL_FACADE_ANIMATION ril)
{
    AnimationSlot winner = null;
    foreach (var slot in info.animslots)
    {
        if (false == slot.active) continue;
        winner = slot;
        break;
    }

    if (null != winner)
    {
        ril.animstate = winner.animstate;
        ril.animname = winner.animname;
        // 同步到 info，确保 Hash diff 正确
        info.animstate = winner.animstate;
        info.animname = winner.animname;
    }
    else
    {
        ril.animstate = 0;
        ril.animname = null;
    }

    ril.animelapsed = (info.animelapsed * stage.cfg.fp2int).AsUInt();
}
```

### 4.5 各来源的槽位定义

```
来源              slot.key        priority  istransient   写入方式
─────────────────────────────────────────────────────────────────────────
StateMachine     SLOT_STATE        0        false         SetAnimation(state) → AddOrUpdateSlot(state=state, name=null)
AnimationData    SLOT_NAMED        200      true          SetAnimation(name) → AddOrUpdateSlot(state=0, name=name) [OnExit 移除]
BeHit            SLOT_HITSTUN        400      false         BeHitExecutor.OnEnter → AddOrUpdateSlot(state=HITSTUN, name=null) [StateMachine 恢复时移除]
[未来] Frozen     SLOT_FROZEN       700      true          StatusEffect → AddOrUpdateSlot(state=FROZEN)
[未来] Cutscene   SLOT_CUTSCENE     1000     true          AddOrUpdateSlot(state=0, name="cs_intro")
```

> **2026-07-21 决策**：受击不新建 Behavior。BeHitExecutor 直接调用 `facade.AddOrUpdateSlot(SLOT_HITSTUN, ...)` + `statemachine.ChangeState(HITSTUN, duration)`。槽位由 StateMachine 在硬直到期时移除。

### 4.6 数据流全景

```
StateMachine              AnimationExecutor       BeHitExecutor
     │                          │                     │
     │ SetAnimation(IDLE)       │ SetAnimation("chg")  │ OnEnter()
     │ → state=3, name=null    │ → state=0, name=+    │ → state=HITSTUN, name=null
     ▼                          ▼                     ▼
┌────────────────────────────────────────────────────────────────┐
│                  Facade.animslots (List<AnimationSlot>)         │
│                  EnsureSort() 按 priority 降序                   │
│                                                                 │
│  [0] SLOT_HITSTUN    pri=400  state=8  name=-    ← 胜出          │
│  [1] SLOT_NAMED    pri=200  state=0  name="chg"← 被覆盖         │
│  [2] SLOT_STATE    pri=0    state=3  name=-    ← 被覆盖         │
└────────────────────────────────────────────────────────────────┘
                               │
                  FacadeAnimationTranslator
                  ril.animstate = 8 (HITSTUN)
                  ril.animname  = null
                               │
                               ▼
                  RIL_FACADE_ANIMATION { animstate: 8, animname: null, animelapsed: 120 }
                               │
                               ▼
                  AnimationAgent (现有逻辑，零改动)：
                  playname = ril.animname ?? animcfg.GetAnimationName(ril.animstate)
                          = null ?? "hit_01"
                          = "hit_01"
```

### 4.7 为什么保持双字段

| 维度 | animstate (byte) | animname (string) |
|------|------------------|-------------------|
| 语义 | 持久状态，数量固定 ~10 | 临时指令，数量无法穷举 |
| 生命周期 | 直到下次 SetAnimation(byte) 覆盖 | Pipeline begin→end |
| 映射方式 | AnimationConfig 查表（Render 层现有） | 直传，不查表 |
| 画同步 | byte 比 string 更小更稳定 | string 仅在命名动画时传 |

两者正交，互不替代。强行合为单字段会导致：
- 全用 string：Logic 层需维护 `state→name` 映射（`state2anim` 字典 + `LoadStateMapping` 注入）
- 全用 byte：命名动画无法表示

### 4.8 v1 实际落地

```
v1 实际活跃槽位：

SLOT_STATE    pri=0    persistent  ← animstate=StateMachine.current, animname=null
SLOT_NAMED    pri=200  transient   ← animstate=0, animname="charge_loop"（AnimationExecutor 控制）
SLOT_HITSTUN    pri=400  persistent  ← animstate=HITSTUN, animname=null（BeHitExecutor 直接写入，StateMachine 恢复时移除）
```

### 4.9 新增结构

```
AnimationSlot (新，走 ObjectCache 池化，双字段)
├── key         : int
├── priority    : int
├── animstate   : byte        // 持久状态
├── animname    : string      // 命名动画
├── active      : bool
├── istransient : bool
└── duration    : FP

BeHitData 新增字段：
├── hitstunduration   : FP        // 受击硬直时长
├── hitstunlevel      : byte      // 硬直等级（预留）
└── interruptcast   : bool      // 是否打断施法

StateMachine 新增：
├── stateduration   : FP              // 当前限时状态剩余时间
├── timerslotkey   : ANIM_SLOT_KEY   // 到期时清理哪个槽位
├── timerfallback  : byte            // 到期后切回哪个状态
└── ChangeState(state, duration, slotkey, fallback) → 限时切换，通用不硬编码
```

> **2026-07-21 决策**：槽位由 BeHitExecutor 直接管理，duration 由 StateMachine 管理。改动面最小。

---

## 五、详细实现

### 5.1 改动清单

| # | 位置 | 改动 | 行数估算 |
|---|------|------|----------|
| 1 | 新增 `Gameplay/Logic/Common/AnimationSlot.cs` | 槽位枚举 + 优先级常量 + AnimationSlot 类（双字段）| ~55 行 |
| 2 | `Gameplay/Logic/BehaviorInfos/FacadeInfo.cs` | 新增 `animslots` List | +15 行 |
| 3 | `Gameplay/Logic/Behaviors/Facade.cs` | 新增 `AddOrUpdateSlot`、`RmvSlot`、`EnsureSort`；`SetAnimation` 内部重构走槽位 | +40 行 |
| 4 | `Gameplay/Logic/Translators/FacadeAnimationTranslator.cs` | `OnCalcHashCode` 改为取 winner 双字段 + animelapsed + effectincrement；`OnRIL` 改为取最高优先级槽位双字段 | +15/-5 行 |
| 5 | `Gameplay/Logic/Behaviors/StateMachine.cs` | 新增 `stateduration/timerslotkey/timerfallback` + `ChangeState(state, duration, slotkey, fallback)` 重载 + OnTick 通用倒计时 | +18 行 |
| 6 | `Gameplay/Logic/Flows/Executors/Instructs/BeHitData.cs` | 加 `hitstunduration`、`hitstunlevel`、`interruptcast` | +6 行 |
| 7 | `Gameplay/Logic/Flows/Executors/BeHitExecutor.cs` | 加 ROLL/DEATH 守卫 + 直接操作 Facade 槽位 + 调用 StateMachine.ChangeState(HITSTUN, duration) | +10 行 |
| 8 | `Gameplay/Logic/Flows/Executors/ChangeStateExecutor.cs` | 删除 hack 行 | -1 行 |
| 9 | 各 Scripting 管线 | `BeHitData` 补上硬直时长 | 按需 |

**不动的文件**（零改动）：
- `Gameplay/Logic/RIL/RIL_FACADE_ANIMATION.cs` — 双字段不变
- `Gameplay/Render/Agents/AnimationAgent.cs` — 现有 fallback 完美兼容
- `Gameplay/Render/Agents/PrimitiveAnimAgent.cs` — byte switch 不动
- `Gameplay/Render/Common/AnimationConfig.cs` — 不动
- `Gameplay/Logic/Prefabs/HeroPrefab.cs` — 零改动
- `Gameplay/Logic/Prefabs/EnemyPrefab.cs` — 零改动
- `Gameplay/Logic/Common/Defines/TICK_DEFINE.cs` — 零改动（HITSTUN 走 StateMachine 现有 Tick）

总计：~146 行新代码。**零新增 Behavior。零 Prefab 改动。零 TICK_DEFINE 改动。**

### 5.2 AnimationSlot.cs（框架核心，新增）

```csharp
public enum ANIM_SLOT_KEY : int
{
    STATE           = 0,    // StateMachine 基态
    NAMED           = 1,    // AnimationData 命名动画
    HITSTUN           = 2,    // 受击硬直（BeHitExecutor 直接写入，StateMachine 恢复时移除）
    DEATH           = 3,    // 死亡（预留）
}

public static class ANIM_PRIORITY
{
    public const int LOCOMOTION    = 0;
    public const int INTERACT      = 100;
    public const int ACTION        = 200;
    public const int REACTION      = 400;
    public const int COUNTER       = 500;
    public const int KNOCKDOWN     = 600;
    public const int HARDCROWD     = 700;
    public const int LIFESTATE     = 800;
    public const int SYSTEM        = 1000;
}

public class AnimationSlot
{
    public ANIM_SLOT_KEY key;
    public int priority;
    public byte animstate;           // 持久状态：STATE_DEFINE 值，SLOT_NAMED 时为 0
    public string animname;          // 命名动画：SLOT_STATE/HITSTUN 时为 null
    public bool active;
    public bool istransient;
    public FP duration;

    public void Activate(byte state = 0, string name = null, FP dur = default)
    {
        active = true;
        animstate = state;
        animname = name;
        if (dur > FP.Zero) { istransient = true; duration = dur; }
    }

    public void Deactivate()
    {
        active = false;
        animstate = 0;
        animname = null;
        istransient = false;
        duration = FP.Zero;
    }
}
```

### 5.3 FacadeInfo.cs 改动

```diff
 public class FacadeInfo : BehaviorInfo
 {
+    public List<AnimationSlot> animslots { get; set; }

     public byte animstate { get; set; }     // 保留，Translator 同步 winner 双字段写入
     public string animname { get; set; }    // 保留，Translator 同步 winner 双字段写入
     public FP animelapsed { get; set; }

     protected override void OnReady()
     {
+        animslots = ObjectCache.Ensure<List<AnimationSlot>>();
     }

     protected override void OnReset()
     {
+        animslots.Clear();
+        ObjectCache.Set(animslots);
     }

     protected override BehaviorInfo OnClone()
     {
         var clone = ObjectCache.Ensure<FacadeInfo>();
         clone.Ready(actor);
+        foreach (var slot in animslots)
+        {
+            var s = ObjectCache.Ensure<AnimationSlot>();
+            s.key = slot.key; s.priority = slot.priority;
+            s.animstate = slot.animstate; s.animname = slot.animname;
+            s.active = slot.active; s.istransient = slot.istransient;
+            s.duration = slot.duration;
+            clone.animslots.Add(s);
+        }
         return clone;
     }
 }
```

### 5.4 Facade.cs 改动

```diff
+// === 槽位管理 API（双字段） ===

+public void AddOrUpdateSlot(ANIM_SLOT_KEY key, int priority, byte state = 0, string name = null, FP duration = default)
+{
+    var slot = GetSlot(key);
+    if (null == slot)
+    {
+        slot = ObjectCache.Ensure<AnimationSlot>();
+        slot.key = key;
+        info.animslots.Add(slot);
+    }
+    slot.priority = priority;
+    slot.Activate(state, name, duration);
+    EnsureSort();
+}

+public void RmvSlot(ANIM_SLOT_KEY key)
+{
+    var slot = GetSlot(key);
+    if (null != slot)
+    {
+        info.animslots.Remove(slot);
+        slot.Deactivate();
+        ObjectCache.Set(slot);
+    }
+}

+private void EnsureSort() => info.animslots.Sort((a, b) => b.priority.CompareTo(a.priority));

+private AnimationSlot GetSlot(ANIM_SLOT_KEY key)
+{
+    foreach (var slot in info.animslots)
+        if (slot.key == key) return slot;
+    return null;
+}

+// === SetAnimation 内部走槽位 ===

 public void SetAnimation(byte state)
 {
     info.animstate = state;
+    AddOrUpdateSlot(ANIM_SLOT_KEY.STATE, ANIM_PRIORITY.LOCOMOTION, state: state);
 }

 public void SetAnimation(string name, byte tickmode)
 {
     info.animname = name;
     if (null != name)
+        AddOrUpdateSlot(ANIM_SLOT_KEY.NAMED, ANIM_PRIORITY.ACTION, name: name);
     else
         RmvSlot(ANIM_SLOT_KEY.NAMED);
     info.animelapsed = 0;
     info.animticktype = tickmode;
 }
```

### 5.5 FacadeAnimationTranslator.cs 改动

```csharp
protected override int OnCalcHashCode(FacadeInfo info)
{
    int hash = 17;
    hash = hash * 31 + info.actor.GetHashCode();
    // 取最高优先级活跃槽位的双字段
    byte winnerstate = 0;
    string winnername = null;
    foreach (var slot in info.animslots)
    {
        if (false == slot.active) continue;
        winnerstate = slot.animstate;
        winnername = slot.animname;
        break;
    }
    hash = hash * 31 + winnerstate.GetHashCode();
    hash = hash * 31 + (null != winnername ? winnername.GetHashCode() : 0);
    hash = hash * 31 + info.animelapsed.GetHashCode();
    hash = hash * 31 + info.effectincrement.GetHashCode();

    return hash;
}

protected override void OnRIL(FacadeInfo info, RIL_FACADE_ANIMATION ril)
{
    AnimationSlot winner = null;
    foreach (var slot in info.animslots)
    {
        if (false == slot.active) continue;
        winner = slot;
        break;
    }

    if (null != winner)
    {
        ril.animstate = winner.animstate;
        ril.animname = winner.animname;
        info.animstate = winner.animstate;   // 同步 Hash
        info.animname = winner.animname;
    }
    else
    {
        ril.animstate = 0;
        ril.animname = null;
    }

    ril.animelapsed = (info.animelapsed * stage.cfg.fp2int).AsUInt();
}
```

### 5.6 StateMachine.cs 改动

```diff
 public class StateMachine : Behavior<StateMachineInfo>
 {
+    /// <summary>
+    /// 状态持续时长（FP.Zero = 无限，限时状态到期自动切 fallback）
+    /// </summary>
+    public FP stateduration { get; set; }
+    public ANIM_SLOT_KEY timerslotkey { get; set; }   // duration 到期时清理的槽位
+    public byte timerfallback { get; set; }             // duration 到期后切换的目标状态

     public void Break()
     {
+        stateduration = FP.Zero;
         ChangeState(STATE_DEFINE.NONE);
     }
+
+    /// <summary>
+    /// 切换到限时状态（duration > 0 才启用计时器）
+    /// slotkey: 状态结束时清理的槽位；fallback: 到期后切回的状态
+    /// </summary>
+    public void ChangeState(byte state, FP duration, ANIM_SLOT_KEY slotkey = ANIM_SLOT_KEY.STATE, byte fallback = STATE_DEFINE.IDLE)
+    {
+        stateduration = duration;
+        timerslotkey = slotkey;
+        timerfallback = fallback;
+        ChangeState(state);
+    }

     public void ChangeState(byte state)
     {
         info.last = info.current;
         info.current = state;
         info.usedelaybreak = false;
         info.delaybreak = FP.Zero;

         if (false == stage.SeekBehavior(actor, out Facade facade)) return;
         if (STATE_DEFINE.CASTING == info.current)
         {
             facade.SetAnimation(STATE_DEFINE.CASTING);
             return;
         }

         facade.SetAnimation(info.current);
     }

     protected override void OnTick(FP tick)
     {
         base.OnTick(tick);
+
+        // 限时状态倒计时（通用，不硬编码任何具体状态）
+        if (stateduration > FP.Zero)
+        {
+            stateduration -= tick;
+            if (stateduration <= FP.Zero)
+            {
+                stateduration = FP.Zero;
+                if (stage.SeekBehavior(actor, out Facade facade))
+                    facade.RmvSlot(timerslotkey);
+                ChangeState(timerfallback);
+            }
+        }
+
         if (false == info.usedelaybreak) return;
         info.delaybreak -= tick;
         if (info.delaybreak <= FP.Zero) Break();
     }
 }
```

### 5.7 BeHitData.cs 改动

```diff
 public class BeHitData : InstructData
 {
     public bool uselookatattacker = true;
     public bool usehitmotion = false;
     public byte hitmotiontype = BEHIT_DEFINE.MOTION_SELF_FORWARD;
     public IntVector3 hitmotion;
+
+    public FP hitstunduration = FP.Zero;
+    public byte hitstunlevel = 1;
+    public bool interruptcast = false;
 }
```

### 5.8 BeHitExecutor.cs 改动

```diff
 protected override void OnEnter(..., ulong target)
 {
-    if (stage.SeekBehaviorInfo(target, out StateMachineInfo statemachine) && STATE_DEFINE.DEATH == statemachine.current) return;
+    if (stage.SeekBehaviorInfo(target, out StateMachineInfo statemachine))
+    {
+        if (STATE_DEFINE.DEATH == statemachine.current) return;
+        if (STATE_DEFINE.ROLL == statemachine.current) return;
+    }

     // ... 现有朝向+击退位移逻辑不变 ...

+    // 受击硬直动画（直接操作 Facade 槽位 + StateMachine）
+    if (data.hitstunduration > FP.Zero && stage.SeekBehavior(target, out StateMachine sm))
+    {
+        // 写入受击槽位（优先级 400，覆盖 AnimationData）
+        if (stage.SeekBehavior(target, out Facade facade))
+            facade.AddOrUpdateSlot(ANIM_SLOT_KEY.HITSTUN, ANIM_PRIORITY.REACTION,
+                state: STATE_DEFINE.HITSTUN);
+
+        // 切换限时状态（到期自动清槽位 + 切回 IDLE）
+        sm.ChangeState(STATE_DEFINE.HITSTUN, data.hitstunduration,
+            slotkey: ANIM_SLOT_KEY.HITSTUN,
+            fallback: STATE_DEFINE.IDLE);
+    }
+
+    // 打断施法管线（可选）
+    if (data.interruptcast && stage.SeekBehavior(target, out SkillBreak sb))
+        sb.Break();
 }
```

### 5.9 Prefab 组装 — 零改动

HeroPrefab / EnemyPrefab 无需任何修改。HITSTUN 是 StateMachine 已有状态，槽位由 BeHitExecutor 直接操作。

### 5.10 RIL_FACADE_ANIMATION.cs — 零改动

双字段不变：`animstate (byte)` + `animname (string)` + `animelapsed (uint)`。

### 5.11 渲染层 — 零改动

| 文件 | 状态 |
|------|------|
| `AnimationAgent.cs` | 第 57 行 `ril.animname ?? animcfg?.GetAnimationName(ril.animstate)` 现有 fallback 完美兼容双字段 |
| `PrimitiveAnimAgent.cs` | `switch (animstate)` 用 byte 判断，不动 |
| `AnimationConfig.cs` | `GetAnimationName(byte state)` 不动 |
| `AnimationConfig.json` | 不动 |

---

## 六、交互规则

### 6.1 状态跃迁表

| 当前状态 | 事件 | 槽位变化 | 结果 |
|---------|------|----------|------|
| SLOT_HITSTUN 未激活 | 受击 | +SLOT_HITSTUN(pri=400, state=HITSTUN) | HITSTUN 覆盖 |
| SLOT_HITSTUN 激活中 | 再次受击 | 刷新 duration | combo 连击重置 |
| SLOT_HITSTUN 激活中 | duration 到期 | -SLOT_HITSTUN | 回落下一个优先级 |
| SLOT_HITSTUN 激活中 | 死亡 | SM→DEATH | DEATH 胜出 |
| SLOT_HITSTUN 激活中 | 冰冻（未来）| +SLOT_FROZEN(700) | 冻结覆盖受击 |

### 6.2 ROLL / DEATH 不可被 HITSTUN

在 `BeHitExecutor.OnEnter()` 入口统一检查，ROLL/DEATH 时直接跳过，不写槽位不切状态。

### 6.3 多槽位并发覆盖

```
t=0:  活跃 [SLOT_NAMED "charge_loop"(200)]
t=100: +SLOT_HITSTUN state=HITSTUN(400) → HITSTUN 胜出
t=600: SLOT_HITSTUN 到期 → "charge_loop" 恢复
t=700: SLOT_NAMED 到期 → IDLE(0) 胜出
```

---

## 七、管线脚本示例

```csharp
// 重击（大硬直，打断技能）
ScriptMachine.Instruct(SPARK_INSTR_DEFINE.FLOW, SPARK_INSTR_DEFINE.TOKEN_ON_HIT, new BeHitData
{
    uselookatattacker = true,
    usehitmotion = true,
    hitmotiontype = BEHIT_DEFINE.MOTION_ATTACKER_TO_SELF,
    hitmotion = new IntVector3(0, 0, 1200),
    hitstunduration = FP.FromMillis(600),
    hitstunlevel = 2,
    interruptcast = true,
});
```

---

## 八、ChangeStateExecutor 清理

```diff
- // TODO : HACKER 这里是为了做受击动画, 后续受击动画需要改成独立的动画状态机来处理
- if (statemachine.info.current == data.state) statemachine.Break();
  statemachine.TryChangeState(data.state);
```

---

## 九、不做的事（v1 范围）

| 不做 | 原因 |
|------|------|
| 新增受击 Behavior | HITSTUN 是 StateMachine 已有状态，槽位由 BeHitExecutor 直接操作 |
| 新 RIL 类型 | RIL_FACADE_ANIMATION 双字段不变 |
| RIL 单轨化 | animstate 和 animname 是正交语义，保持双字段 |
| state2anim 字典 / LoadStateMapping | 不需要——渲染层已有映射 |
| AnimationAgent 改动 | 现有 `animname ?? GetAnimationName(animstate)` 完美兼容 |
| PrimitiveAnimAgent 改动 | byte switch 不动 |
| AnimationConfig 改动 | 不动 |
| Prefab 改动 | 零改动 |
| TICK_DEFINE 改动 | 零改动（HITSTUN 走 StateMachine 现有 Tick）|
| 上下半身分离 | 2.5D 需求不强 |
| 方向性受击动画 | v1 单一 HITSTUN |
| hitstun decay/scaling | 后续数值细化 |
| StateMachine.PASSES 改动 | 不动现有迁移逻辑 |

---

## 十、后续扩展

有了槽位系统后，新增动画来源成本极低：

```
冰冻效果：  +ANIM_SLOT_KEY.FROZEN → Apply() 时 facade.AddOrUpdateSlot(FROZEN, 700, state=FROZEN)
过场接管：  facade.AddOrUpdateSlot(CUTSCENE, 1000, name="cs_intro", duration=...)
```

扩展成本对比：

```
                    旧方案（加字段）         新方案（加槽位）
新增动画来源         加 overridestate2       +ANIM_SLOT_KEY 枚举值
                    FacadeInfo 加字段        FacadeInfo 零字段改动
                    Translator 加 else if    Translator 零改动
                    Render 层加 switch       Render 层零改动
```

### v2：多轨混合

槽位当前 "winner takes all"，未来若需同时播放：

```
winner = animslots[0]  // 最高优先：HITSTUN (上半身)
runner = animslots[1]  // 次高优先：MOVE  (下半身)
// RIL 扩展两个 slot → AnimationTree Blend
```

槽位数据模型无需改动。

---

## 十一、版本迭代记录

### 2026-07-21 第一轮：代码级验证

| # | 修正 | 原因 |
|---|------|------|
| 1 | `SortedSet` → `List` + `EnsureSort` | 项目无 SortedSet 先例 |
| 2 | AnimationSlot 走 ObjectCache | 对齐项目池化模式 |
| 3 | Translator 同步 `info.animname` | Hash diff 不丢 |
| 4 | DEATH/ROLL 守卫 | 防御性编程 |
| 5 | TICK_DEFINE 注册受击 Behavior（已废弃）| 本轮砍掉 |
| 6 | 管线示例改为 spark instruct | ET 搜索已修复 |

### 2026-07-21 第二轮：name 单轨化（已废弃）

曾尝试 RIL 只传 `animname`，在 Logic 层完成 state→name 解析。暴露的问题：

| # | 问题 | 说明 |
|---|------|------|
| 7 | 需要 `state2anim` 字典 + `LoadStateMapping` 注入 | 在 Logic 层重复了 Render 层已有的映射能力 |
| 8 | 需要改 RIL 定义 | 删 `animstate` 字段 |
| 9 | 需要改 AnimationAgent | 移除 fallback 逻辑 |
| 10 | 需要改 PrimitiveAnimAgent | byte → string 类型迁移 |
| 11 | 需要改 AnimationConfig | 瘦身为 name→mix |
| 12 | Prefab 多 2 行注入代码 | 跨层引用 Render 类型 |

### 2026-07-21 第三轮：双轨归正

基于讨论确认 animstate 和 animname 是正交语义，保持双字段：

| # | 修正 | 说明 |
|---|------|------|
| 13 | **RIL 双字段保留** | `animstate (byte)` + `animname (string)` 不变 |
| 14 | **AnimationSlot 双字段** | state 驱动写 state，name 驱动写 name |
| 15 | **删除 state2anim / LoadStateMapping** | 不需要在 Logic 层重复查表 |
| 16 | **Render 层零改动** | AnimationAgent/PrimitiveAnimAgent/AnimationConfig 不动 |
| 17 | **Prefab 零注入** | BeHitExecutor 直接操作 Facade 槽位 |
| 18 | **Translator Hash 改为双字段** | winner.animstate + winner.animname + animelapsed + effectincrement |
| 19 | **改动文件数 15→12** | 删 RIL、AnimationAgent、PrimitiveAnimAgent、AnimationConfig 改动 |

### 2026-07-21 第四轮：砍受击 Behavior 复用 StateMachine

| # | 修正 | 说明 |
|---|------|------|
| 20 | **删除受击 Behavior** | 不新增 Behavior。HITSTUN 是 StateMachine 已有状态，槽位由 BeHitExecutor 直接操作，duration 由 StateMachine 管理 |
| 21 | BeHitExecutor 直接操作 Facade 槽位 | `facade.AddOrUpdateSlot(SLOT_HITSTUN, 400, state=HITSTUN)` 替代独立 Behavior |
| 22 | StateMachine 新增 stateduration | `ChangeState(state, duration)` 重载 + OnTick 自动恢复 IDLE |
| 23 | 零 Prefab / TICK_DEFINE 改动 | 无需 `AddBehavior<受击>`，无需插入 Tick 顺序 |
| 24 | 改动文件 12→9 | 删 2 个新增文件 + 1 个 TICK_DEFINE + 2 个 Prefab，总计 ~140 行（第六轮泛化后 ~146 行）|

### 2026-07-21 第五轮+第六轮：术语归位——BeHit 是指令，HitStun 是状态

| # | 修正 | 说明 |
|---|------|------|
| 25 | 区分指令层与状态层 | BeHitData/BeHitExecutor/BEHIT_DEFINE = 受击指令；STATE_DEFINE.HITSTUN/SLOT_HITSTUN = 硬直状态 |
| 26 | `STATE_DEFINE.BEHIT` → `HITSTUN` | 状态应该叫 HITSTUN（硬直），BEHIT 只是触发它的指令 |
| 27 | `SLOT_HITSTUN` 保持 | 槽位代表硬直状态，用 HITSTUN 正确 |
| 28 | `hitstunduration/hitstunlevel` 保持 | BeHitData 携带的字段描述的是"硬直多久/什么等级"，不是"受击多久" |

### 2026-07-21 第七轮：解耦 + 命名对齐

| # | 修正 | 说明 |
|---|------|------|
| 29 | StateMachine 泛化解耦 | `timerslotkey` + `timerfallback` 消除 OnTick 硬编码；Frozen 等限时状态复用同一逻辑 |
| 30 | `ChangeState` 重载签名扩展 | `ChangeState(state, duration, ANIM_SLOT_KEY slotkey, byte fallback)` |
| 31 | `Break()` 重置 `stateduration` | 避免 Break→NONE 后残留倒计时 |
| 32 | `RmvSlot` 对齐命名规范 | `RemoveSlot` → `RmvSlot`（Rmv 是 Remove 的缩写） |

### 涉及文件（最终）

```
新增：
  Gameplay/Logic/Common/AnimationSlot.cs                     (~55 行)

修改：
  Gameplay/Logic/BehaviorInfos/FacadeInfo.cs                  (+15 行, +animslots List)
  Gameplay/Logic/Behaviors/Facade.cs                          (+40 行, 槽位管理 + SetAnimation 重构)
  Gameplay/Logic/Behaviors/StateMachine.cs                    (+18 行, stateduration + timerslotkey + timerfallback + ChangeState 重载)
  Gameplay/Logic/Translators/FacadeAnimationTranslator.cs     (+15/-5 行, Hash + OnRIL 双字段)
  Gameplay/Logic/Flows/Executors/Instructs/BeHitData.cs       (+6 行)
  Gameplay/Logic/Flows/Executors/BeHitExecutor.cs             (+10 行)
  Gameplay/Logic/Flows/Executors/ChangeStateExecutor.cs       (-1 行, 删 hack)
  Gameplay/Logic/Flows/Scriptings/S10020.cs                   (按需)

不动：
  Gameplay/Logic/RIL/RIL_FACADE_ANIMATION.cs                  (零改动)
  Gameplay/Render/Agents/AnimationAgent.cs                    (零改动)
  Gameplay/Render/Agents/PrimitiveAnimAgent.cs                (零改动)
  Gameplay/Render/Common/AnimationConfig.cs                   (零改动)
  Gameplay/Logic/Prefabs/HeroPrefab.cs                        (零改动)
  Gameplay/Logic/Prefabs/EnemyPrefab.cs                       (零改动)
  Gameplay/Logic/Common/Defines/TICK_DEFINE.cs                (零改动)
```
