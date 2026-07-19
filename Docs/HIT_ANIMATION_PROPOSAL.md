# 受击动画独立状态机方案

> 2026-07-19 | 解耦 `ChangeStateExecutor` 中 BEHIT 的 hack，建立统一的 Facade 动画优先级模型

---

## 一、现状：三条动画路径共占一个 Facade

系统已有两条动画驱动路径，再加受击就是第三条，三者在 `Facade` 上共存但**没有优先级模型**。

### 1.1 路径 A：StateMachine → animstate（持久基态）

```
ChangeStateData → ChangeStateExecutor
  → StateMachine.ChangeState(state)
    → facade.SetAnimation(state)         // FacadeInfo.animstate = IDLE/MOVE/CASTING/DEATH
```

生命周期：持久，直到下次状态切换。StateMachine 每次 `ChangeState` 都会写 `animstate`。

### 1.2 路径 B：AnimationData → animname（管道定时命名动画）

```
AnimationData(begin, end) → AnimationExecutor
  OnEnter:  facade.SetAnimation(name, TICK_MANUAL)   // FacadeInfo.animname = "charge_loop"
  OnExecute: facade.info.animelapsed += LOGIC_TICK
  OnExit:   facade.SetAnimation(null, TICK_AUTOMATIC) // 清除 animname
```

生命周期：Pipeline instruct begin→end。用于技能蓄力动画等固定时长的命名动画。

`AnimationAgent` 中的解析逻辑：
```csharp
var animname = ril.animname ?? animcfg?.GetAnimationName(ril.animstate);
// animname 优先 → 命名动画覆盖状态映射动画
```

### 1.3 路径 C：BeHit → ？？？（受击动画，当前缺失）

```
BeHitData → BeHitExecutor
  OnEnter: 仅做朝向+击退位移
  // 没有驱动任何动画！BEHIT 状态从未被设置
```

### 1.4 攻击 → 受击流程（以 S10020 重击为例）

```
S10020 Pipeline (t=200ms)
├── CollisionData (碰撞检测)    → 命中目标写入 FlowCollisionHurtInfo.targets
├── BeHitData (受击位移)        → 转身面对攻击者 + 击退位移（无动画！）
├── HitLagData (顿帧)           → 双方暂停 frames
└── Spark → DamageData (伤害)   → 扣血
```

### 1.5 问题：三路径无优先级模型

目前 `FacadeAnimationTranslator` 只是机械地把 `animstate` 和 `animname` 原样写入 RIL，由 `AnimationAgent` 用 `animname ?? GetName(animstate)` 做二选一。但这只是 **animname 优先于 animstate** 的隐式约定，不是显式的优先级模型。

当三条路径同时活跃时（施法中被打），无法保证正确的动画选择：

| 场景 | animstate | animname | 期望 | 现状 |
|------|-----------|----------|------|------|
| 施法中被重击 | CASTING | "charge_loop" | BEHIT | "charge_loop"（路径 B 覆盖路径 A，路径 C 不存在） |
| 移动中被轻击 | MOVE | null | BEHIT | MOVE（路径 C 不存在） |
| 受击硬直中死亡 | IDLE | null | DEATH | 需额外处理 |

### 1.6 ChangeStateExecutor 中的 Hack

```csharp
// ChangeStateExecutor.cs:31-33
// TODO : HACKER 这里是为了做受击动画, 后续受击动画需要改成独立的动画状态机来处理
if (statemachine.info.current == data.state) statemachine.Break();
statemachine.TryChangeState(data.state);
```

非 force 路径用 `Break()`→NONE→`TryChangeState` 绕路。这暴露了问题：**系统缺少一个独立于 StateMachine 的动画覆盖通道**。

---

## 二、根因：缺少统一的 Facade 动画优先级模型

三条动画路径各自写 Facade 的不同字段，Translator 和 AnimationAgent 靠隐式约定 (`animname ?? GetName(animstate)`) 选择最终动画。**没有显式的优先级规则**。

| 问题 | 说明 |
|------|------|
| 无优先级定义 | 受击动画应该覆盖施法动画，但当前 animname 优先于 animstate，路径 C 缺失 |
| StateMachine 耦合 | BEHIT 必须走 StateMachine，结束后不知道该回 IDLE 还是 MOVE |
| 生命周期冲突 | AnimationData 是 Pipeline 计时，HitStun 是独立计时，两者无协调机制 |
| 打断不完整 | CASTING 被 BEHIT 覆盖后，Skill 逻辑层不知道被打断 |
| 同状态重入绕路 | `Break()`→NONE→`TryChangeState` 是 hack |

### 现有 State 体系（不变）

```
IDLE(3)   → DEATH, MOVE, FALL, CASTING, BEHIT, ROLL
MOVE(4)   → DEATH, IDLE, FALL, CASTING, BEHIT, ROLL
JUMP(5)   → DEATH, FALL, CASTING, BEHIT
FALL(6)   → DEATH, IDLE, CASTING, BEHIT
CASTING(7)→ DEATH, BEHIT
BEHIT(8)  → DEATH, BEHIT
ROLL(9)   → DEATH, IDLE, MOVE, CASTING
```

---

## 三、行业参考

### 3.1 格斗游戏：统一状态机

街霸、罪恶装备——逻辑和动画是一台状态机，出拳是 State，受击硬直是 HITSTUN State，每个 State 自带帧数。状态是对立的，没有"叠加"。

**不适合本项目**：空间可移动的俯视角 ARPG，受击需要和移动/技能共存。

### 3.2 Souls-like / 怪猎：Layer + Slot 分层

动画按优先度分 Slot，高优先度覆盖低优先度：

```
Layer 0 - Base (Full Body)     │ Layer 1 - Override (Full Body)  │ Layer 2 - Additive (Upper Body)
IDLE / WALK / RUN              │ ATTACK / SKILL / DODGE         │ HIT_BACK / HIT_FRONT
priority: 0                    │ priority: 1                    │ priority: 2
```

- **同层互斥**：新攻击打断旧攻击
- **跨层叠加**：受击可与攻击动画混合（上半身后仰 + 下半身保持技能滑步）
- **到期恢复**：受击播放完自然回到基础层，不污染逻辑状态

**最适合本项目**：逻辑层确定性帧同步 + RIL 传输 + 渲染层独立消费的架构天然适配这个模型。

### 3.3 UE Animation Blueprint：变量驱动

逻辑层只写 `Speed=1.0`、`HitDirection=Front`，动画蓝图根据变量自选/混合动画。受击就是设 `HitReaction` 变量。

**部分借鉴**：你的 AnimationConfig JSON 已经有"状态 → 动画名"的映射，接近这个思路。

### 3.4 本项目选型：通用 AnimationSlot 优先级系统

对照行业方案，结合当前架构：

```
                    Goblin 架构                              业内映射
                    ────────────                             ────────
逻辑层              StateMachine (逻辑状态)                    Souls 逻辑状态机
                    HitStun (表现层计时器)                      Souls Slot Override
                    [未来] FrozenEffect / Parry / ...           Souls Multi-Layer
                    
Facade层 ★          AnimationSlot 槽位集合
                    ├── SLOT_STATE(pri=0)    持久基态          UE Animation Blueprint
                    ├── SLOT_NAMED(pri=200)  管道动画           Souls Layer 1
                    ├── SLOT_HITSTUN(pri=400) 受击硬直          Souls Layer 2
                    └── [未来] SLOT_FROZEN(pri=700) ...         Souls Higher Layers

翻译层              FacadeAnimationTranslator
                    └── winner = MaxPriority(animSlots)         Souls Priority Resolver

渲染层              AnimationAgent                              Godot AnimationPlayer
                    └── 零改动 ★
```

核心思路：**不引入新的 RIL 类型，不改造 AnimationAgent**。Facade 层维护一个 `SortedSet<AnimationSlot>`，每个动画来源注册为一个 Slot + 优先级数字。Translator 取最高优先活跃 Slot 输出一条 RIL。新增动画来源只需加一个枚举值，**零字段改动，零 Translator 改动**。渲染层完全无感。

---

## 四、架构设计：动画槽位优先级系统

### 4.1 设计原则

不引入新 RIL 类型，不改 `AnimationAgent`。作为框架，动画来源会持续增加——不能在 `FacadeInfo` 上每来一个来源就加一个字段。

**核心抽象**：任何动画来源都注册为一个 `AnimationSlot`，带优先级数字。Facade 维护一个槽位集合，Translator 取最高优先级活跃槽位解析为一条 RIL。

```
新增动画来源的成本 = 注册一个 Slot + 定一个优先级数字。零字段改动，零 Translator 改动。
```

### 4.2 AnimationSlot 设计

```
AnimationSlot
├── key         : int          // 槽位键（唯一标识，枚举定义）
├── priority    : int          // 优先级（越大越优先）★ 框架扩展核心
├── animstate   : byte         // 状态映射动画（走 AnimationConfig 查表）
├── animname    : string?      // 命名动画（不走查表，直接播放）
├── active      : bool         // 是否活跃
├── isTransient : bool         // true=临时覆盖（到期自动废弃）, false=持久（直到主动移除）
└── duration    : FP           // 临时槽位的剩余时间
```

**优先级编号约定**（框架层定义，具体值由项目配置）：

```
1000+   系统接管级     Cutscene, ScriptedSequence
800-999 生命状态级     Death, Revive
600-799 硬控状态级     Frozen, Petrified, Stunned
400-599 受击反应级     HitStun, Knockdown, Launch, Grab
200-399 主动动作级     Attack, Skill, Dodge, Parry
100-199 交互动作级     Interact, Push, Climb
0-99    基础运动级     Idle, Walk, Run, Jump, Fall
```

### 4.3 Facade 槽位集合

```csharp
// FacadeInfo
public List<AnimationSlot> animSlots { get; set; }  // 所有活跃的动画槽位

// Facade
public void AddAnimSlot(AnimationSlot slot)    // 注册槽位
public void RemoveAnimSlot(int key)            // 移除槽位
public void UpdateAnimSlot(AnimationSlot slot) // 更新槽位状态
```

Facade 内部维护一个 `SortedSet<AnimationSlot>`（按 priority 降序），`Add/Remove/Update` 时自动重排。

### 4.4 Translator 解析逻辑（通用，永不变）

```csharp
protected override void OnRIL(FacadeInfo info, RIL_FACADE_ANIMATION ril)
{
    // 取最高优先级的活跃槽位
    AnimationSlot winner = null;
    foreach (var slot in info.animSlots)
    {
        if (!slot.active) continue;
        if (winner == null || slot.priority > winner.priority)
            winner = slot;
    }

    if (winner != null)
    {
        ril.animstate = winner.animstate;
        ril.animname = winner.animname;
    }
    else
    {
        ril.animstate = 0;
        ril.animname = null;
    }

    ril.animelapsed = (info.animelapsed * stage.cfg.fp2int).AsUInt();
}
```

**Translator 从此不需要知道有多少种动画来源**。它只做一件事：取最高优先级活跃槽位 → 写 RIL。新来源只需 `facade.AddAnimSlot(...)`。

### 4.5 各来源的槽位定义

```
来源                  slot.key          priority   isTransient   驱动者
──────────────────────────────────────────────────────────────────────
StateMachine         SLOT_STATE         0          false         ChangeStateExecutor
AnimationData        SLOT_NAMED         200        true          AnimationExecutor  
HitStun              SLOT_HITSTUN       400        true          HitStun
Death (via SM)       SLOT_DEATH         800        false         StateMachine (DEATH 时加入)
[未来] Frozen         SLOT_FROZEN        700        true          StatusEffect
[未来] Knockdown      SLOT_KNOCKDOWN    600        true          BeHitExecutor (重击)
[未来] Parry          SLOT_PARRY        500        true          ParryExecutor
[未来] Cutscene       SLOT_CUTSCENE     1000       true          CutsceneManager
```

### 4.6 数据流全景

```
StateMachine          AnimationExecutor       HitStun            [未来] FrozenEffect
     │                       │                    │                      │
     │ AddSlot(SLOT_STATE)   │ AddSlot(SLOT_NAMED)│ AddSlot(SLOT_HITSTUN)│ AddSlot(SLOT_FROZEN)
     ▼                       ▼                    ▼                      ▼
┌────────────────────────────────────────────────────────────────────────────────┐
│                        Facade.animSlots (SortedSet<AnimationSlot>)              │
│                         按 priority 降序自动排序                                  │
│                                                                                │
│  [0] SLOT_HITSTUN   pri=400 active=true   ← 当前最高优先，胜出                   │
│  [1] SLOT_NAMED     pri=200 active=true   ← 被覆盖，休眠中                       │
│  [2] SLOT_STATE     pri=0   active=true   ← 被覆盖，休眠中                       │
└────────────────────────────────────────────────────────────────────────────────┘
                                 │
                    FacadeAnimationTranslator
                    winner = animSlots[0]   // O(1)，已排序
                                 │
                                 ▼
                     RIL_FACADE_ANIMATION (1条)
                                 │
                                 ▼
                         AnimationAgent
                     (零改动，无感知)
```

### 4.7 v1 实际落地：只新增 1 个槽位

v1 只加 `SLOT_HITSTUN`，但 Facade 槽位基础架构一次建好。现有的 `animstate` 和 `animname` 改为通过 `SLOT_STATE` 和 `SLOT_NAMED` 槽位注册：

```
v1 实际活跃槽位：

SLOT_STATE    pri=0    persistent    ← StateMachine 驱动（等价旧 animstate）
SLOT_NAMED    pri=200  transient     ← AnimationExecutor 驱动（等价旧 animname）
SLOT_HITSTUN  pri=400  transient     ← HitStun 驱动（新）
```

**向后兼容**：StateMachine 的 `SetAnimation(state)` 内部改为 `AddOrUpdateSlot(SLOT_STATE, priority=0, animstate=state)`。AnimationExecutor 同理。旧行为完全保留。

### 4.8 新增结构

```
AnimationSlot (新)
├── key         : int          // 槽位键 (SLOT_STATE / SLOT_NAMED / SLOT_HITSTUN ...)
├── priority    : int          // 优先级（越大越优先）
├── animstate   : byte         // 走 AnimationConfig 映射
├── animname    : string?      // 直接播放的命名动画
├── active      : bool         // 是否活跃
├── isTransient : bool         // 临时槽位
└── duration    : FP           // 剩余时间

HitStunInfo : BehaviorInfo
├── active       : bool        
├── duration     : FP          
├── hitstunlevel : byte        
└── prevstate    : byte        

HitStun : Behavior<HitStunInfo>
├── Apply(duration, level, interruptCast)  → facade.AddAnimSlot(SLOT_HITSTUN, ...)
├── OnTick(tick) → 倒计时, 到点 RemoveSlot(SLOT_HITSTUN)
└── Recover()   → facade.RemoveAnimSlot(SLOT_HITSTUN)
```

---

## 五、详细实现

### 5.1 改动清单

| # | 位置 | 改动 | 行数估算 |
|---|------|------|----------|
| 1 | 新增 `Common/AnimationSlot.cs` | **槽位数据结构定义 + 槽位键枚举** | ~60 行 |
| 2 | 新增 `BehaviorInfos/HitStunInfo.cs` | 受击硬直数据定义 | ~35 行 |
| 3 | 新增 `Behaviors/Sa/HitStun.cs` | 受击硬直行为（通过 Slot 操作 Facade） | ~55 行 |
| 4 | `BehaviorInfos/FacadeInfo.cs` | 旧 `animstate`/`animname` 保留 + 新增 `animSlots` 集合 | +15 行 |
| 5 | `Behaviors/Facade.cs` | 新增 `AddAnimSlot`/`RemoveAnimSlot`/`UpdateAnimSlot`；`SetAnimation` 内部重构 | +40 行 |
| 6 | `Translators/FacadeAnimationTranslator.cs` | `OnRIL` 改为取最高优先级活跃槽位 | +15 行 |
| 7 | `Instructs/BeHitData.cs` | 加 `hitstunduration`、`hitstunlevel`、`interruptcast` | +6 行 |
| 8 | `Executors/BeHitExecutor.cs` | `OnEnter` 触发 `HitStun.Apply()` | +8 行 |
| 9 | `Prefabs/HeroPrefab.cs` | 挂载 `HitStun` 行为 | +2 行 |
| 10 | `Prefabs/EnemyPrefab.cs` | 挂载 `HitStun` 行为 | +2 行 |
| 11 | 各 Scripting 管线 | `BeHitData` 补上硬直时长 | 按需 |

总计：~240 行新代码（比旧方案多 ~100 行，多出来的是槽位基础架构，一次性投资）。

### 5.2 AnimationSlot.cs（框架核心，新增）

```csharp
/// <summary>
/// 动画槽位键枚举 — 每种动画来源一个值，新增来源只需加枚举项
/// </summary>
public enum ANIM_SLOT_KEY : int
{
    STATE           = 0,    // StateMachine 基态
    NAMED           = 1,    // AnimationData 命名动画
    HITSTUN         = 2,    // 受击硬直
    DEATH           = 3,    // 死亡（预留，v1 可能不需要单独槽位）
    // --- 未来扩展 ---
    // FROZEN       = 4,    // 冰冻
    // KNOCKDOWN    = 5,    // 击倒
    // PARRY        = 6,    // 格挡
    // CUTSCENE     = 7,    // 过场
}

/// <summary>
/// 动画优先级定义 — 越大越优先
/// </summary>
public static class ANIM_PRIORITY
{
    public const int LOCOMOTION    = 0;     // IDLE/WALK/RUN/JUMP/FALL
    public const int INTERACT      = 100;   // 开门/推箱/攀爬
    public const int ACTION        = 200;   // 技能/攻击/闪避
    public const int REACTION      = 400;   // 受击硬直
    public const int COUNTER       = 500;   // 格挡/弹反
    public const int KNOCKDOWN     = 600;   // 击倒/击飞
    public const int HARDCROWD     = 700;   // 冰冻/石化/眩晕
    public const int LIFESTATE     = 800;   // 死亡/复活
    public const int SYSTEM        = 1000;  // 过场/剧情接管
}

/// <summary>
/// 动画槽位 — 任何动画来源都注册为一个 Slot
/// </summary>
public class AnimationSlot
{
    public ANIM_SLOT_KEY key;           // 唯一标识
    public int priority;                // 优先级（越大越优先）
    public byte animstate;              // 状态映射（走 AnimationConfig 查表）
    public string animname;             // 命名动画（直接播放），null=走状态映射
    public bool active;                 // 是否活跃
    public bool isTransient;            // true=临时槽位（自动计时），false=持久槽位
    public FP duration;                 // 临时槽位剩余时间

    public AnimationSlot(ANIM_SLOT_KEY key, int priority)
    {
        this.key = key;
        this.priority = priority;
        active = false;
        isTransient = false;
        duration = FP.Zero;
    }

    public void Activate(byte state, string name = null, FP dur = default)
    {
        active = true;
        animstate = state;
        animname = name;
        if (dur > FP.Zero) { isTransient = true; duration = dur; }
    }

    public void Deactivate()
    {
        active = false;
        animstate = 0;
        animname = null;
        isTransient = false;
        duration = FP.Zero;
    }

    public void Tick(FP delta)
    {
        if (!isTransient || !active) return;
        duration -= delta;
        if (duration <= FP.Zero) Deactivate();
    }
}
```

### 5.3 FacadeInfo.cs 改动

```diff
 public class FacadeInfo : BehaviorInfo
 {
+    // 槽位集合 — 框架级的动画优先级系统
+    // SortedSet 按 priority 降序，animSlots[0] 始终是当前最高优先级活跃槽位
+    public SortedSet<AnimationSlot> animSlots { get; set; }
+
+    // 以下两个字段保留做向后兼容，内部通过 SLOT_STATE / SLOT_NAMED 槽位实现
     public byte animstate { get; set; }    // → 内部映射到 SLOT_STATE
     public string animname { get; set; }   // → 内部映射到 SLOT_NAMED
     public FP animelapsed { get; set; }
     
+    // 预分配固定数量槽位（避免每帧分配），按 ANIM_SLOT_KEY 枚举数量
+    private AnimationSlot[] slotCache;
+
+    protected override void OnReady()
+    {
+        animSlots = new SortedSet<AnimationSlot>(new AnimSlotPriorityComparer());
+        slotCache = new AnimationSlot[Enum.GetValues(typeof(ANIM_SLOT_KEY)).Length];
+    }
+}
+
+/// <summary>
+/// 自定义比较器：按 priority 降序排列
+/// </summary>
+public class AnimSlotPriorityComparer : IComparer<AnimationSlot>
+{
+    public int Compare(AnimationSlot a, AnimationSlot b)
+    {
+        int cmp = b.priority.CompareTo(a.priority);  // 降序
+        return cmp != 0 ? cmp : a.key.CompareTo(b.key);
+    }
 }
```

### 5.4 Facade.cs 改动

```diff
+// === 槽位管理 API ===

+/// <summary>
+/// 注册/更新一个动画槽位（框架面向未来设计的核心接口）
+/// </summary>
+public void AddOrUpdateSlot(ANIM_SLOT_KEY key, int priority, byte state, string name = null, FP duration = default)
+{
+    var slot = EnsureSlot(key, priority);
+    slot.Activate(state, name, duration);
+    // SortedSet 自动重排
+}

+public void RemoveSlot(ANIM_SLOT_KEY key)
+{
+    var slot = GetSlot(key);
+    if (slot != null)
+    {
+        animSlots.Remove(slot);
+        slot.Deactivate();
+    }
+}

+/// <summary>
+/// Tick 所有临时槽位（由 Facade.OnTick 调用）
+/// </summary>
+public void TickSlots(FP delta)
+{
+    // 收集到期槽位（避免在遍历中修改集合）
+    List<ANIM_SLOT_KEY> expired = null;
+    foreach (var slot in info.animSlots)
+    {
+        if (!slot.isTransient) continue;
+        slot.duration -= delta;
+        if (slot.duration <= FP.Zero)
+        {
+            (expired ??= new()).Add(slot.key);
+        }
+    }
+    if (expired != null)
+        foreach (var key in expired) RemoveSlot(key);
+}

+// === 向后兼容（内部走槽位） ===

+public void SetAnimation(byte state)
+{
+    info.animstate = state;
+    AddOrUpdateSlot(ANIM_SLOT_KEY.STATE, ANIM_PRIORITY.LOCOMOTION, state);
+}

+public void SetAnimation(string name, byte tickmode)
+{
+    info.animname = name;
+    if (name != null)
+        AddOrUpdateSlot(ANIM_SLOT_KEY.NAMED, ANIM_PRIORITY.ACTION, 0, name);
+    else
+        RemoveSlot(ANIM_SLOT_KEY.NAMED);
+    
+    if (tickmode == TICK_MANUAL) info.tickmode = TICK_MANUAL;
+    else info.tickmode = TICK_AUTOMATIC;
+    info.animelapsed = 0;
+}
```

### 5.5 FacadeAnimationTranslator.cs 改动

```csharp
protected override void OnRIL(FacadeInfo info, RIL_FACADE_ANIMATION ril)
{
    // 取最高优先级的活跃槽位 — 框架通用逻辑，新增动画来源时此行不变
    AnimationSlot winner = null;
    foreach (var slot in info.animSlots)
    {
        if (!slot.active) continue;
        winner = slot;  // SortedSet 已按 priority 降序排列，第一个活跃的就是最高优先
        break;
    }

    if (winner != null)
    {
        ril.animstate = winner.animstate;
        ril.animname = winner.animname;
    }
    else
    {
        ril.animstate = 0;
        ril.animname = null;
    }

    ril.animelapsed = (info.animelapsed * stage.cfg.fp2int).AsUInt();
}
```

**Translator 完全通用化**——不需要知道有多少种动画来源，不需要 switch-case，不需要 priority 常量。新增动画来源时这 15 行代码不动。

### 5.6 HitStunInfo.cs

```csharp
public class HitStunInfo : BehaviorInfo
{
    public bool active { get; set; }
    public FP duration { get; set; }
    public byte hitstunlevel { get; set; }

    protected override void OnReady()
    {
        active = false;
        duration = FP.Zero;
        hitstunlevel = 0;
    }

    protected override void OnReset()
    {
        active = false;
        duration = FP.Zero;
        hitstunlevel = 0;
    }

    protected override BehaviorInfo OnClone()
    {
        var clone = ObjectCache.Ensure<HitStunInfo>();
        clone.Ready(actor);
        clone.active = active;
        clone.duration = duration;
        clone.hitstunlevel = hitstunlevel;
        return clone;
    }
}
```

### 5.7 HitStun.cs

```csharp
public class HitStun : Behavior<HitStunInfo>
{
    public void Apply(FP duration, byte level, bool interruptCast = false)
    {
        // combo 连击：重置计时
        info.duration = duration;
        info.hitstunlevel = level;

        // 不能在无敌/翻滚状态下受击
        if (stage.SeekBehavior(actor, out StateMachine sm))
        {
            if (sm.info.current == STATE_DEFINE.ROLL) return;
        }

        if (info.active)
        {
            // 已在受击中，只刷新槽位持续时间
            if (stage.SeekBehavior(actor, out Facade facade))
                facade.AddOrUpdateSlot(ANIM_SLOT_KEY.HITSTUN, ANIM_PRIORITY.REACTION, 
                    STATE_DEFINE.BEHIT, null, duration);
            return;
        }

        info.active = true;

        // 打断 CASTING
        if (interruptCast && stage.SeekBehavior(actor, out StateMachine sm2))
        {
            if (sm2.info.current == STATE_DEFINE.CASTING)
                sm2.Break();
        }

        // 注册受击槽位
        if (stage.SeekBehavior(actor, out Facade facade2))
            facade2.AddOrUpdateSlot(ANIM_SLOT_KEY.HITSTUN, ANIM_PRIORITY.REACTION,
                STATE_DEFINE.BEHIT, null, duration);
    }

    public void Recover()
    {
        if (!info.active) return;
        info.active = false;
        info.hitstunlevel = 0;

        if (stage.SeekBehavior(actor, out Facade facade))
            facade.RemoveSlot(ANIM_SLOT_KEY.HITSTUN);
        // 槽位移除后，Translator 自动取下一个最高优先级活跃槽位
    }

    protected override void OnTick(FP tick)
    {
        base.OnTick(tick);
        if (!info.active) return;

        // Death 优先级更高 — 槽位系统自然处理，但 HitStun 仍需主动退出
        if (stage.SeekBehavior(actor, out StateMachine sm) && 
            sm.info.current == STATE_DEFINE.DEATH)
        {
            Recover();
            return;
        }

        info.duration -= tick;
        if (info.duration <= FP.Zero) Recover();
    }
}
```

### 5.8 BeHitData.cs 改动

```diff
 public class BeHitData : InstructData
 {
     public bool uselookatattacker = true;
     public bool usehitmotion = false;
     public byte hitmotiontype = BEHIT_DEFINE.MOTION_SELF_FORWARD;
     public IntVector3 hitmotion;
+
+    /// <summary>受击硬直时长 (ms)</summary>
+    public FP hitstunduration = FP.Zero;
+    /// <summary>硬直等级 (1=轻击, 2=重击)</summary>
+    public byte hitstunlevel = 1;
+    /// <summary>是否打断 CASTING</summary>
+    public bool interruptcast = false;
 }
```

### 5.9 BeHitExecutor.cs 改动

```diff
 protected override void OnEnter(..., ulong target)
 {
     // ... 现有逻辑不变 (朝向 + 击退位移) ...

+    // 触发受击硬直动画
+    if (data.hitstunduration > FP.Zero && stage.SeekBehavior(target, out HitStun hitstun))
+    {
+        hitstun.Apply(data.hitstunduration, data.hitstunlevel, data.interruptcast);
+    }
 }
```

### 5.10 StateMachine & AnimationExecutor 向后兼容

- `StateMachine.ChangeState()` 调用 `Facade.SetAnimation(state)` → 内部走 `SLOT_STATE` 槽位。
- `AnimationExecutor` 调用 `Facade.SetAnimation(name, ...)` → 内部走 `SLOT_NAMED` 槽位。
- 外部 API 不变，内部机制升级为槽位系统。
- DEATH 时 StateMachine 正常写 `SLOT_STATE(pri=0, animstate=DEATH)`，但因 HitStun 主动 Recover 移除 `SLOT_HITSTUN(pri=400)`，DEATH 自然成为最高优先活跃槽位。

---

## 六、交互规则（槽位系统下的自然语义）

### 6.1 状态跃迁表

| 当前状态 | 事件 | 槽位变化 | 结果 |
|---------|------|----------|------|
| SLOT_HITSTUN 未激活 | 受击 | +SLOT_HITSTUN(pri=400) | BEHIT 动画立即覆盖 |
| SLOT_HITSTUN 激活中 | 再次受击 | 刷新 SLOT_HITSTUN duration | combo 连击，计时重置 |
| SLOT_HITSTUN 激活中 | duration 到期 | -SLOT_HITSTUN | 回落 SLOT_NAMED 或 SLOT_STATE |
| SLOT_HITSTUN 激活中 | 死亡 | SM→DEATH, HitStun.Recover() 移除 SLOT_HITSTUN | DEATH 动画（SLOT_STATE 胜出） |
| SLOT_HITSTUN 激活中 | 冰冻（未来） | +SLOT_FROZEN(pri=700) | 冻结动画覆盖受击 |

### 6.2 Death 优先级（槽位系统自然解决）

在槽位系统中，Death 不需要特殊处理：

```
HitStun.Recover() → RemoveSlot(SLOT_HITSTUN)
                    ↓
        活跃槽位变为 SLOT_STATE(DEATH, pri=0)
                    ↓
        Translator 选取 DEATH 动画
```

`HitStun.OnTick()` 检测到 DEATH 时主动 Recover，移除 `SLOT_HITSTUN(pri=400)` 后，唯一活跃的是 `SLOT_STATE(pri=0, animstate=DEATH)`，Translator 自然选它。**未来如果 Death 需要专门的死亡动画槽位（带镜头特效等），只需注册 `SLOT_DEATH(pri=800)`，HitStun 无需改动**。

### 6.3 CASTING 打断

| interruptcast | 行为 |
|--------------|------|
| `false`（默认）| 只通过槽位覆盖动画，StateMachine 保持 CASTING。技能逻辑正常完成 |
| `true` | 调用 `StateMachine.Break()` → NONE → IDLE，技能被打断 |

### 6.4 ROLL 不可被 BEHIT

在 `HitStun.Apply()` 入口统一检查：
```csharp
if (stage.SeekBehavior(actor, out StateMachine sm) && 
    sm.info.current == STATE_DEFINE.ROLL) return;
```
ROLL 时直接跳过，不注册 SLOT_HITSTUN。

### 6.5 多槽位并发覆盖（槽位系统的核心威力）

场景：施法中被打 → 又在硬直中被冰冻。槽位系统自动处理：

```
t=0:  活跃槽位 [SLOT_NAMED "charge_loop"(200)]
t=100: +SLOT_HITSTUN(400) → 活跃 [SLOT_HITSTUN(400), SLOT_NAMED(200)] → BEHIT 胜出
t=200: +SLOT_FROZEN(700)  → 活跃 [SLOT_FROZEN(700), SLOT_HITSTUN(400)] → FROZEN 胜出
t=500: SLOT_FROZEN 到期    → 活跃 [SLOT_HITSTUN(400)] → BEHIT 恢复
t=600: SLOT_HITSTUN 到期   → 活跃 [SLOT_NAMED(200)] → 蓄力动画恢复
t=700: SLOT_NAMED 到期     → 活跃 [SLOT_STATE(0)] → IDLE
```

**全程无需居中协调逻辑**。每个系统只管自己的槽位注册/移除，Translator 自动解析。这就是框架化的价值。

---

## 七、管线脚本示例

### 轻击（小硬直，不打断技能）

```csharp
Instruct(200, 200, new BeHitData
{
    et = FLOW_DEFINE.ET_FLOW_HIT,
    uselookatattacker = true,
    usehitmotion = true,
    hitmotiontype = BEHIT_DEFINE.MOTION_ATTACKER_TO_SELF,
    hitmotion = new IntVector3(0, 0, 400),
    hitstunduration = FP.FromMillis(200),  // 200ms 轻硬直
    hitstunlevel = 1,
    interruptcast = false,                  // 不打断施法
});
```

### 重击（大硬直，打断技能）

```csharp
Instruct(200, 200, new BeHitData
{
    et = FLOW_DEFINE.ET_FLOW_HIT,
    uselookatattacker = true,
    usehitmotion = true,
    hitmotiontype = BEHIT_DEFINE.MOTION_ATTACKER_TO_SELF,
    hitmotion = new IntVector3(0, 0, 1200),
    hitstunduration = FP.FromMillis(600),  // 600ms 重硬直
    hitstunlevel = 2,
    interruptcast = true,                   // 打断施法
});
```

---

## 八、ChangeStateExecutor 清理

HitStun 上线后，`ChangeStateExecutor` 中 BEHIT 相关的 hack 代码可移除：

```diff
- // TODO : HACKER 这里是为了做受击动画, 后续受击动画需要改成独立的动画状态机来处理
- if (statemachine.info.current == data.state) statemachine.Break();
  statemachine.TryChangeState(data.state);
```

`Break()` 绕路不再需要——受击动画通过 `SLOT_HITSTUN` 槽位驱动，不走 StateMachine。

---

## 九、不做的事（v1 范围）

| 不做 | 原因 |
|------|------|
| 新 RIL 类型 | 不需要，复用现有 `RIL_FACADE_ANIMATION` + Translator 槽位解析 |
| AnimationAgent 多轨混合 | v1 BEHIT 是全 Anim 替换，到期回落 |
| Avatar Mask / 上下半身分离 | 俯视角 2.5D 需求不强，且需要改 AnimationAgent/AnimationTree |
| 方向性受击动画 (HIT_FRONT/BACK) | v1 先做单一 BEHIT 动画 |
| hitstun decay/scaling | 后续数值系统细化 |
| StateMachine.PASSES 规则表改动 | 不动现有状态迁移逻辑 |

---

## 十、后续扩展（槽位架构的框架价值）

有了槽位系统后，新增动画来源的成本极低：

### 10.1 冰冻效果（示例：新增状态效果动画）

```csharp
// 1. 加枚举
ANIM_SLOT_KEY.FROZEN = 4

// 2. FrozenEffect.OnApply():
facade.AddOrUpdateSlot(ANIM_SLOT_KEY.FROZEN, ANIM_PRIORITY.HARDCROWD,
    STATE_DEFINE.FROZEN, null, duration);

// 3. FrozenEffect.OnExpire():
facade.RemoveSlot(ANIM_SLOT_KEY.FROZEN);

// 完成。Hierarchy 自动处理：FROZEN(700) > HITSTUN(400) > NAMED(200) > STATE(0)
```

**零改动**：Translator、AnimationAgent、HitStun、StateMachine 全部不需要动。

### 10.2 击倒/击飞

同一模式：`SLOT_KNOCKDOWN(pri=600)`。比 HitStun 优先级高（击倒覆盖受击），比 Frozen 低（冰冻覆盖击倒）。

### 10.3 过场动画接管

```csharp
facade.AddOrUpdateSlot(ANIM_SLOT_KEY.CUTSCENE, ANIM_PRIORITY.SYSTEM, 0, "cs_intro", csDuration);
// pri=1000 覆盖一切，到期自动回落
```

### 10.4 槽位系统的扩展成本对比

```
                    旧方案（加字段）              新方案（加槽位）
新增动画来源         加 overridestate2 字段        加一个 ANIM_SLOT_KEY 枚举值
                    FacadeInfo 加字段             Facade 零字段改动
                    Translator 加 else if         Translator 零改动
                    影响 4 个文件                  影响 1 个枚举文件
                    约 20 行改动                  约 3 行改动
```

### 10.5 v2：多轨混合（未来）

槽位系统当前取 "winner takes all"，未来若需要"受击动画和跑步动画同时播放"：

```
// Translator 改为输出 top N 而非 top 1
winner = animSlots[0]   // 最高优先：BEHIT (上半身)
runner = animSlots[1]   // 次高优先：MOVE  (下半身)
// ril 扩展两个 slot 给 AnimationAgent 做 AnimationTree Blend
```

这需要改 RIL 结构和 AnimationAgent，但槽位数据模型无需改动——仍是同一个 `SortedSet<AnimationSlot>`。

---
