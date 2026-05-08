# Goblin Demo 开发计划：MVP

> Goblin 框架试水 demo 的**主线开发计划**。
> 任务讨论、决策记录都围绕本文件。冲突的口头讨论以本文更新为准。
> 范围只定 1 周 MVP，跑通后再判断下一步。

最后更新：2026-05-09

---

## 0. 文档约定

- 任务状态：`TODO` / `WIP(yyyy-mm-dd)` / `DONE` / `DROP` / `PAUSE`
- 决策记录区只增不删，废弃的画删除线保留行 + 注明何时为何废弃。
- Demo 是框架试金石，不是完整游戏。**MVP 跑通后**再决定要不要做翻滚 / Boss / 词条 / 回放等扩展。

---

## 1. 定位

**俯视角 3D 单关 Action**，全 Godot PrimitiveMesh，零美术资源。
一句话："剑士站在小平台上，把直冲怪砍光算赢，被撞死算输。"

意图：
- 用最小闭环把 Stage / Behavior / RIL / Director / Agent 这套全跑一遍
- 暴露框架短板，立刻补
- 不做 Roguelite、不做多关、不做翻滚、不做 Boss、不做词条、不做回放、不做多职业

---

## 2. MVP 范围（1 周）

### 2.1 玩法
- 1 张 8m×8m 平台 + 圆柱外墙
- 玩家：剑士，3 段轻击 combo
- 操作：左摇杆移动 / A 轻击
- 敌人：直冲怪 1 种，看到玩家就直线冲撞
- 胜负：怪清空胜 / 玩家 HP=0 败 → 极简结算面板
- 视角：俯视斜角 45° 跟随，不旋转
- 全 PrimitiveMesh

### 2.2 不做（明确排除）
- 翻滚 / 重击 / 职业技能
- 多关 / 关卡晋升 / 难度爬升
- 精英 / Boss / 多种敌人
- 词条 buff / 道具 / 商店
- 死亡回放 / 跨局养成
- 多职业 / 联机
- HUD 美化 / 音效 / 镜头摇晃

---

## 3. Logic 层新增

### 3.1 `ChargeAI` Behavior — TODO
当前 Logic 层无任何 AI Behavior，新增这一份。

最小职责：
- OnTick 找最近的 SA_HERO actor
- 距离 > attackrange：朝向 hero，写 MovementInfo（推进）
- 距离 ≤ attackrange：触发冲撞技能（SkillLauncher.Launch）
- HP=0 由 AttributeCalc 自然销毁，AI 不管

**前置依赖**：[§5.1 Movement 输入解耦](#51-movement-输入抽象-todo) 必须先做。

### 3.2 `EnemyPrefab` — TODO
组装：Spatial + StateMachine + Movement + ChargeAI + AttributeCalc + Facade。

### 3.3 `StageRule` Behavior（挂 SA） — TODO
挂在 Stage Actor (ulong.MaxValue)。
- OnReady：刷 N 个直冲怪
- OnTick：怪清空 → 派发 `RIL_EVENT_LEVEL_CLEAR`；玩家 HP=0 → 派发 `RIL_EVENT_LEVEL_FAIL`

新增 2 个 RIL_EVENT，走 Salute 通道。

---

## 4. Render 层新增

### 4.1 `PrimitiveMeshAgent` — TODO
代替 ModelAgent 在没有 .glb 时的角色。监听 `RIL_FACADE_MODEL`（**同一 RIL 不新增**），通过 ModelInfo 表的 `type` 字段区分 glb / primitive。primitive 路径下创建对应 PrimitiveMesh 节点挂在 SpatialAgent 根节点。

复用 RIL 而非新立一种：Facade.SetModel 一行不动，未来切真模型只换 Agent 实现。

### 4.2 `PrimitiveAnimAgent` — TODO（动画语义方案 A）
PrimitiveMesh 没有 AnimationPlayer。决议：**Logic 层抽象动画 ID 不变，Render 层负责映射**。

监听 `RIL_FACADE_ANIMATION`：

| 动画名 | 表现 |
|---|---|
| IDLE | 静止 |
| MOVE | 上下浮 0.05m，频率 2Hz |
| ATTACK_1/2/3 | z 轴瞬刺（scale.z 1.0→1.3→1.0，0.15s） |
| HURT | material albedo 闪红 0.1s |
| DEAD | 垂直 scale.y 缩到 0，0.5s |

未来切真模型时，AnimationAgent 监听同一 RIL，二者择一启用即可。

### 4.3 `HUDView` + `ResultView` — TODO
- `HUDView`：玩家 HP 条（Phase 0 就摆个 ProgressBar）
- `ResultView`：监听 LEVEL_CLEAR / FAIL，弹"胜/败 + 重开"按钮

---

## 5. 框架修改（借 MVP 顺手做）

### 5.1 Movement 输入抽象 — TODO（必须做）
**问题**：[Movement.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Movement.cs) 直接 `stage.GetInput<GamepadInput>(actor)`，AI 走不通。

**改法**：
- Movement 只读 MovementInfo（期望方向 + 速度 + 是否移动），不再读 Input
- 玩家侧：新增 `PlayerInputController` Behavior，把 GamepadInput 翻译成 MovementInfo
- AI 侧：ChargeAI 直接写 MovementInfo
- Movement 退化为纯执行器，"谁驱动它"取决于 Behavior 组合

**收益**：AI / 远程 / 人机切换都不用改 Movement。

### 5.2 HitLag 堆叠 bug — TODO（5 行）
**问题**：[HitEffect.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/HitEffect.cs) 的 AddHitLag 直接 `ticker.timescale -= xxx`，多次叠加会负。

**改法**：
- 维护 `int hitlagcount`
- AddHitLag → count++ 且只在 count==1 时记录原值并设新值
- RmvHitLag → count-- 且 count==0 时恢复原值

### 5.3 .editorconfig 强制缩进 — TODO（基建）
本会话内 Stage.cs 已被自动 spaces→tabs 改动两次。根目录 [.editorconfig](../.editorconfig) 强制 `.cs` 用 spaces，统一 Rider/VSCode。

---

## 6. 配置表（Luban）

| 表 | 是否新建 | 字段要点 |
|---|---|---|
| ModelInfo | **改** | 加 `type`(glb/primitive), `mesh`, `size`, `color` |
| HeroInfo | 沿用 | 剑士 maxhp / 攻速 / movespeed / 默认技能列表 |
| SkillInfo + Pipeline | 沿用，加数据 | 三段轻击 |
| ColliderInfo | 沿用，加数据 | 三段轻击的 box 尺寸/前向偏移 |
| EnemyInfo | **新建** | maxhp / movespeed / detectrange / attackrange |

---

## 7. 五日节奏

| Day | 目标 | 验收 | 状态 |
|---|---|---|---|
| D1 | 配置表骨架 + EnemyPrefab + StageRule(无 AI) | 玩家能走，怪能站桩 | DONE 2026-05-09 |
| D2 | §5.1 输入解耦 + ChargeAI | 怪能冲过来撞玩家 | TODO |
| D3 | PrimitiveMeshAgent + PrimitiveAnimAgent (IDLE/MOVE/HURT 三态) | 视觉能看出三种状态 | TODO |
| D4 | 3 段 combo + §5.2 HitLag 修复 + 伤害结算 | 能砍死一只怪 | TODO |
| D5 | HUDView + ResultView + 串通 + 实机跑通 | 完整一局 | TODO |

每天末尾留 30min 实跑 + 记问题。

### D1 完成清单（2026-05-09）

- `ACTOR_DEFINE.ENEMY = 6`
- `EnemyData` BuildData 结构 + `StageData.enemies` 字段
- `EnemyPrefab` + `EnemyPrefabInfo`（Behavior 组合与 HeroPrefab 一致，少 Gamepad/Buff）
- `Stage.Prefabs()` 注册 + `Stage.Building()` 加敌人刷出循环
- 配置：`EnemyData.xlsx`（与 HeroData 同字段）+ `__tables__.xlsx` 注册 EnemyInfos + Luban 重生成 EnemyInfo.cs/EnemyInfos.cs/Conf.EnemyInfo.bytes
- `LobbyView.LocalGameBtn` 改为 1 玩家居中 + 4 直冲怪四向 3m 站桩
- StageRule 这一刀**未做**（D1 没胜负判定，先专注怪能站桩）→ D5 集中做

### 已知 TODO 顺延

- StageRule + RIL_EVENT_LEVEL_CLEAR/FAIL → 延到 D5
- D1 拷过来的 5 张配置表只完成了 EnemyData，ColliderInfo / SkillInfo 数据条目复用了 hero 既有的，未改

---

## 8. 决策记录

| 日期 | 决策 | 原因 |
|---|---|---|
| 2026-05-09 | Demo 走 3D 而非 2D | 框架已为 3D 设计（FPVector3、Node3D、GRAVITY），改 2D 反而别扭 |
| 2026-05-09 | 用 PrimitiveMesh 不依赖美术 | 用户美术资源不易提供 |
| 2026-05-09 | 动画走方案 A（程序化映射） | Logic 抽象动画 ID 不变，Render 端映射，未来切真模型只换 Agent |
| 2026-05-09 | RIL_FACADE_MODEL 复用，不新增 PRIMITIVE RIL | Facade.SetModel 不动；ModelInfo.type 字段区分 |
| 2026-05-09 | 砍掉 Phase 1~5 长期路线图 | Demo 是框架试金石，不是完整游戏；MVP 跑通后再判断下一步 |
| 2026-05-09 | MVP 不做翻滚 / Boss / 词条 / 回放 / 多职业 | 跑通基础闭环就停 |
| 2026-05-09 | 接受 D1~D5 节奏 | 用户拍板 |
| 2026-05-09 | §5.1 输入解耦 / §5.2 HitLag / §5.3 editorconfig 三项 MVP 期内做 | 用户拍板 |
| 2026-05-09 | 动画语义采用方案 A（程序化映射） | 用户拍板 |

---

## 附录：现有框架积木速查

> 直接可用，不动。详见 [CLAUDE.md](../CLAUDE.md)。

### Logic 层
- [Stage.cs](../godot/Scripts/Goblin/Gameplay/Logic/Core/Stage.cs)
- [HeroPrefab.cs](../godot/Scripts/Goblin/Gameplay/Logic/Prefabs/HeroPrefab.cs)
- [SkillLauncher.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/SkillLauncher.cs) / [StateMachine.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/StateMachine.cs) / [Movement.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Movement.cs) / [Facade.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Facade.cs)
- [AttributeCalc.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/AttributeCalc.cs) / [Buff.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/Buff.cs) / [Bullet.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/Bullet.cs) / [Detection.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/Detection.cs) / [HitEffect.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/HitEffect.cs) / [Captain.cs](../godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/Captain.cs)

### Render 层
- [SpatialAgent.cs](../godot/Scripts/Goblin/Gameplay/Render/Agents/SpatialAgent.cs) / [SpatialBatch.cs](../godot/Scripts/Goblin/Gameplay/Render/Batches/SpatialBatch.cs)
- [EffectAgent.cs](../godot/Scripts/Goblin/Gameplay/Render/Agents/EffectAgent.cs)
- [DamageSalute.cs](../godot/Scripts/Goblin/Gameplay/Render/Resolvers/Salutes/DamageSalute.cs)
