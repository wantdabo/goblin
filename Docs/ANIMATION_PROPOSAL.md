# 动画槽位优先级系统

> 2026-07-19 初稿 | 2026-07-22 Phase 1+2 落地，更新为架构文档

---

## 一、目标

- 帧同步 / 状态同步均支持
- 动作游戏 / 休闲小游戏均支持
- Logic 层纯数据 + 定点数，零 Godot 依赖
- 新增动画来源的成本 = 注册一个槽位 + 定一个优先级

---

## 二、现状：三条动画路径

系统有三条动画驱动路径，在 `Facade` 上通过 AnimationSlot 统一仲裁。

### 路径 A：StateMachine → animstate（持久基态）

```
ChangeStateData → ChangeStateExecutor
  → StateMachine.ChangeState(state)
    → facade.SetAnimation(state)           // SLOT_STATE, priority=LOCOMOTION/LIFESTATE
```

生命周期：持久，直到下次状态切换。渲染层通过 AnimationConfig 将 `animstate (byte)` 映射为动画名。

### 路径 B：AnimationData → animhash（管道定时命名动画）

```
AnimationData(begin, end) → AnimationExecutor
  OnEnter:  facade.SetAnimation(name, TICK_MANUAL, layer)   // SLOT_NAMED, priority=ACTION
  OnExecute: facade.info.animelapsed += LOGIC_TICK
  OnExit:   facade.SetAnimation(null, TICK_AUTOMATIC)        // RmvSlot(SLOT_NAMED)
```

生命周期：Pipeline instruct begin→end。动画名经 `AnimHash.Hash` 转为 uint 哈希，Render 层通过 `AnimationConfig.GetAnimationNameByHash` 反查。

### 路径 C：BeHit → SLOT_OVERRIDE（受击硬直）

```
BeHitData → BeHitExecutor
  OnEnter: facade.AddOrUpdateSlot(SLOT_OVERRIDE, REACTION, state=HITSTUN)
           sm.ChangeState(HITSTUN, hitstunduration, fallback=IDLE)
  // StateMachine 计时到期 → ChangeState(IDLE) → SetAnimation(IDLE) → RmvSlot(SLOT_OVERRIDE)
```

生命周期：StateMachine 限时状态驱动，到期自动回落。

### 三路径优先级仲裁

```
                    Logic 层写入                              槽位
                    ──────────                               ──────
StateMachine     → SetAnimation(byte)          → SLOT_STATE    pri=0/800
AnimationData    → SetAnimation(string, layer) → SLOT_NAMED    pri=200
BeHitExecutor    → AddOrUpdateSlot(...)        → SLOT_OVERRIDE pri=400

                    ↓ Facade.animslots 按 priority 降序

                    FacadeAnimationTranslator（逐层选 winner）

                    ↓ RIL_FACADE_ANIMATION

                    AnimationEnchant（路由）

                    ↓                           ↓
              AnimationAgent              PrimitiveAnimAgent
              (AnimationPlayer)           (程序化 mesh 变形)
```

---

## 三、AnimationSlot 模型

```
AnimationSlot
├── key         : byte         // 槽位键（SLOT_STATE / SLOT_NAMED / SLOT_OVERRIDE）
├── priority    : int          // 优先级（越大越优先）
├── animstate   : byte         // 持久状态（STATE_DEFINE 值）
├── animhash    : uint         // 命名动画哈希（AnimHash.FNV-1a）
├── layer       : byte         // 动画层（LAYER_FULLBODY / UPPER / LOWER）
├── active      : bool         // 是否活跃
├── istransient : bool         // true=临时覆盖，到期自动回收
└── duration    : FP           // 临时槽位剩余时间
```

### 优先级编号约定

```
1000    SLOT_PRIORITY_SYSTEM        系统接管
800     SLOT_PRIORITY_LIFESTATE     死亡 / 出生
700     SLOT_PRIORITY_HARDCROWD     硬控
600     SLOT_PRIORITY_KNOCKDOWN     击倒
500     SLOT_PRIORITY_COUNTER       反击
400     SLOT_PRIORITY_REACTION      受击反应
200     SLOT_PRIORITY_ACTION        主动动作
100     SLOT_PRIORITY_INTERACT      交互
0       SLOT_PRIORITY_LOCOMOTION    基础运动
```

### 动画层定义（Phase 2）

```
0  LAYER_FULLBODY    全身
1  LAYER_UPPER       上半身
2  LAYER_LOWER       下半身
3  LAYER_MAX         最大层数
```

---

## 四、RIL 多层架构（Phase 2）

```
RIL_FACADE_ANIMATION
├── animstate      : byte               // layer 0 兼容字段（镜像）
├── animhash       : uint               // layer 0 兼容字段（镜像）
├── animelapsed    : uint               // 流逝时间
├── layeranims     : LayerAnimEntry[]   // 多层动画数据（OnReady 预分配，帧内零分配）
└── layercount     : byte               // 活跃层数

LayerAnimEntry (struct)
├── layer      : byte
├── animstate  : byte
└── animhash   : uint
```

### Translator 逐层仲裁

```csharp
// animslots 已按 priority 降序排列
for (byte l = 0; l < LAYER_MAX; l++)
{
    var winner = FindLayerWinner(info, l);  // 每层取首个 active 槽位
    if (null == winner && 0 != l) continue; // layer 0 必出（fallback 到 info.animstate）

    ril.layeranims[count].layer = l;
    ril.layeranims[count].animstate = winner?.animstate ?? info.animstate;
    ril.layeranims[count].animhash  = winner?.animhash  ?? info.animhash;
    count++;
}
// layer 0 镜像到 ril.animstate / ril.animhash（兼容旧消费者）
```

### 兼容性

- `ril.animstate` / `ril.animhash` 镜像 layer 0，`PrimitiveAnimAgent` 零改动
- `AnimationAgent` 当前只消费 layer 0，Phase 2.5 接入 AnimationTree 后消费多层
- 所有 `AddOrUpdateSlot` 调用点默认 `layer=LAYER_FULLBODY`，零破坏

---

## 五、数据流全景

```
写入侧                                     读取侧
──────                                     ──────
StateMachine.ChangeState ──┐
                           ├─→ Facade.SetAnimation(byte)   ──→ SLOT_STATE  (layer 0)
                           │   Facade.SetAnimation(string)  ──→ SLOT_NAMED  (layer 可配)
AnimationExecutor.OnEnter ─┘   AddOrUpdateSlot(...)         ──→ SLOT_OVERRIDE
BeHitExecutor.OnEnter ────────→ AddOrUpdateSlot(SLOT_OVERRIDE, REACTION)
                           │
                           ↓
                    FacadeInfo.animslots
                    （按 priority 降序，OnTick 过期瞬时槽位）
                           │
                           ↓
              FacadeAnimationTranslator
              （逐层 FindLayerWinner → 填 layeranims）
                           │
                           ↓
                    RIL_FACADE_ANIMATION
                    （layeranims[0..layercount-1]）
                           │
                           ↓
              ┌────────────┴────────────┐
              ↓                         ↓
      AnimationEnchant (路由)
              ↓                         ↓
    AnimationAgent              PrimitiveAnimAgent
    (AnimationPlayer)           (程序化 mesh 变形)
    layer 0 当前消费             ril.animstate 消费
    Phase 2.5: 多层消费
```

---

## 六、设计决策

### 6.1 双字段：animstate + animhash

| 维度 | animstate (byte) | animhash (uint) |
|------|------------------|-----------------|
| 语义 | 持久状态，数量固定 ~10 | 临时命名动画，数量无法穷举 |
| 生命周期 | 直到下次 SetAnimation(byte) | Pipeline begin→end |
| 映射方式 | AnimationConfig 查表（Render 层） | AnimHash 哈希反查（Render 层） |

两者正交，互不替代。命名动画用哈希而非 string 传输，节省带宽且确定性。

### 6.2 受击不新建 Behavior

HITSTUN 是 StateMachine 已有状态。BeHitExecutor 直接操作 Facade 槽位 + StateMachine 限时状态，零新增 Behavior。

### 6.3 瞬时槽位 OnTick 过期

`AddOrUpdateSlot(duration>0)` 设 `istransient=true`。`Facade.OnTick` 倒序遍历，到期自动回收。与 StateMachine 限时状态双轨并行——槽位管动画覆盖，StateMachine 管逻辑状态。

### 6.4 SetAnimation(byte) 清 animhash

切回状态动画时清 `info.animhash = 0`，防止 Translator fallback 路径泄漏旧哈希。

---

## 七、路线图

### Phase 1 — 槽位优先级系统 ✅ 已落地

- AnimationSlot 模型 + ObjectCache 池化
- Facade.AddOrUpdateSlot / RmvSlot / EnsureSort
- FacadeAnimationTranslator winner 仲裁
- StateMachine 限时状态 + 通用倒计时
- BeHitExecutor 受击硬直 + ROLL/DEATH 守卫
- AnimHash FNV-1a 跨平台确定性哈希

### Phase 2 — 多层 RIL 架构 ✅ 已落地

- ANIM_DEFINE: LAYER_FULLBODY / UPPER / LOWER / MAX
- AnimationSlot.layer 字段
- RIL_FACADE_ANIMATION: LayerAnimEntry[] + layercount
- Translator 逐层 FindLayerWinner 仲裁
- AnimationData.layer 字段（管线可指定层）
- Facade.SetAnimation(string) 支持 layer 参数
- Facade.OnTick 瞬时槽位过期回收
- SetAnimation(byte) 清 animhash 修复
- AnimationConfigCache 死代码清理
- Debug 输出逐层 winner 展示

### Phase 2.5 — 逐层 elapsed + AnimationTree 接入

**问题**：当前 `info.animelapsed` 是单值，所有层共享。多层的动画进度无法独立追踪。

**改动**：
- RIL `LayerAnimEntry` 加 `elapsed` 字段
- AnimationSlot 加 `elapsed` 字段，OnTick 逐槽位递增
- AnimationAgent 接入 Godot AnimationTree，按 layer 索引驱动 blend node
- 上半身攻击 + 下半身走路同时播放

**覆盖**：状态同步动作游戏

### Phase 3 — 槽位 key 复合化

**问题**：`SLOT_NAMED` 是单 key，同帧两个不同 layer 的命名动画互相覆盖。

**改动**：
- key 从 byte 扩展为 `(layer, slottype)` 复合键，或新增 SLOT_NAMED_UPPER / SLOT_NAMED_LOWER
- 支持同层多命名动画共存

**覆盖**：复杂动作游戏（连招 + 多部位并行）

### Phase 4 — 动画事件帧系统

**问题**：动作游戏需要"第 15 帧出伤害、第 8-12 帧可取消"。当前靠 Pipeline 时间线硬编码，与动画实际进度解耦。

**改动**：
- AnimationConfig 加 event frames 定义
- Logic 层按 elapsed 触发事件回调
- 取消窗、命中帧、特效帧与动画进度耦合

**覆盖**：竞技级帧同步动作游戏

### Phase 5 — 确定性混合参数

**问题**：1D/2D BlendSpace（速度→走/跑混合）只在 Render 层。帧同步两端可能分歧。

**改动**：
- RIL 扩展 blend weight 字段
- Logic 层算混合参数，Render 层只执行

**覆盖**：帧同步 + 程序化动画

---

## 八、能力边界评估

| 目标 | 当前 | 需补 |
|------|------|------|
| 休闲小游戏 | ✅ 完全够用 | 无 |
| 状态同步动作游戏 | ⚠️ 基本够 | 逐层 elapsed、带宽优化 |
| 帧同步动作游戏 | ⚠️ 骨架够 | 逐层 elapsed、事件帧、确定性混合 |
| 竞技级动作游戏 | ❌ 需扩展 | 取消窗、复合 key、逐层速度控制 |

架构方向正确（Slot/Priority/Layer 是行业标准抽象），无需推翻重来。当前是 v2，能撑住休闲游戏和简单动作游戏。

---

## 九、涉及文件

### Logic 层

| 文件 | 职责 |
|------|------|
| `Common/Defines/ANIM_DEFINE.cs` | 槽位键、优先级、层定义 |
| `Common/Defines/STATE_DEFINE.cs` | 状态枚举 + PASSES 跃迁规则 |
| `Common/AnimHash.cs` | FNV-1a 动画名称哈希 |
| `BehaviorInfos/FacadeInfo.cs` | FacadeInfo + AnimationSlot 定义 |
| `Behaviors/Facade.cs` | 槽位管理 + SetAnimation + OnTick 过期 |
| `Behaviors/StateMachine.cs` | 状态切换 + 限时倒计时 + ChangeStateCore |
| `Translators/FacadeAnimationTranslator.cs` | 逐层仲裁 + RIL 填充 |
| `RIL/RIL_FACADE_ANIMATION.cs` | RIL + LayerAnimEntry |
| `Flows/Executors/Instructs/AnimationData.cs` | 管线动画指令（含 layer） |
| `Flows/Executors/AnimationExecutor.cs` | 管线动画执行器 |
| `Flows/Executors/Instructs/BeHitData.cs` | 受击指令数据 |
| `Flows/Executors/BeHitExecutor.cs` | 受击执行器（SLOT_OVERRIDE + 限时状态） |
| `Flows/Executors/ChangeStateExecutor.cs` | 状态变更执行器 |

### Render 层

| 文件 | 职责 |
|------|------|
| `Common/AnimationConfig.cs` | 配置加载 + 哈希索引 |
| `Agents/AnimationAgent.cs` | AnimationPlayer 消费 RIL（layer 0） |
| `Agents/PrimitiveAnimAgent.cs` | 程序化 mesh 变形 |
| `Resolvers/Enchants/AnimationEnchant.cs` | 按模型类型路由 Agent |

### Debug

| 文件 | 职责 |
|------|------|
| `Debug/GameplayStateProvider.cs` | 槽位状态 + 逐层 winner JSON 导出 |

---

## 十、迭代记录

### 2026-07-19 初稿

受击动画缺失，提出 AnimationSlot 优先级方案。

### 2026-07-21 七轮迭代（Phase 1）

1. SortedSet → List + EnsureSort（对齐项目无 SortedSet 先例）
2. AnimationSlot 走 ObjectCache 池化
3. name 单轨化废弃 → 双字段归正（animstate + animname 正交）
4. 砍受击 Behavior → 复用 StateMachine HITSTUN
5. 术语归位：BeHit 是指令，HitStun 是状态
6. StateMachine 泛化解耦：timerslotkey + timerfallback
7. 命名对齐：RemoveSlot → RmvSlot

### 2026-07-22 Phase 2 多层 RIL

1. ANIM_DEFINE 加 LAYER_FULLBODY/UPPER/LOWER/MAX
2. AnimationSlot 加 layer 字段
3. RIL_FACADE_ANIMATION 加 LayerAnimEntry[] + layercount
4. Translator 逐层 FindLayerWinner 仲裁
5. AnimationData / SetAnimation(string) 支持 layer
6. LayerAnimEntry 属性化（{ get; set; }）
7. LAYER_MAX 4→3 修正
8. RmvSlot 重置 layer
9. StateMachine ChangeStateCore 消除冗余赋值
10. SetAnimation(byte) 清 animhash 修复
11. OnTick 瞬时槽位过期回收
12. AnimationConfigCache 死代码清理
