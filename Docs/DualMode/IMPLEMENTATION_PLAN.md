# Property Sync 实施计划

> **统筹文档**：汇总三个设计文档的全部任务，拆解为可执行的阶段与子任务。
>
> 基线文档：
> - `CORE.md` — 哲学底座：Simulation → Projection → Presentation
> - `PROPERTY_SYNC_DESIGN.md` — 完整工程方案
> - `BEHAVIORINFO_LIFECYCLE_REPORT.md` — 生命周期自动化分析

---

## 1. 总体目标

**删 RIL，标 Project，Entity/Component 镜像。Logic 几乎不动。**

| 指标 | 当前 | 目标 |
|------|------|------|
| Logic → Render 中间类 | ~40（RIL + Translator + Cross） | 0 |
| Render 类 | ~32（Agent/Enchant/Invoker/Chase） | 3（Entity/Component/RenderWorld） |
| 新增同步字段改动文件数 | 3-5 | 1（加注解） |
| BehaviorInfo 手写生命周期方法 | 72 | 0（Source Generator 生成） |
| Diff 机制 | 手写 hash + 快照比较 | 脏标记（写入记账，零 Diff） |
| 裁剪 | 无接入点 | 规则链原生支持 |

详见 `PROPERTY_SYNC_DESIGN.md` §1/§9，`BEHAVIORINFO_LIFECYCLE_REPORT.md` §1。

---

## 2. 架构速查

```
Simulation（不动）
    Logic BehaviorInfo + [Project] 注解 + [Lifecycle] 注解
                │
                ▼
Sync Projection（新增）
    ProjectorSystem 脏标记 → Crop 规则链裁剪 → Transport 分叉
                │
                ▼
Presentation（重写）
    Entity + Component（删 Agent/Enchant/Invoker/Chase）
```

详见 `CORE.md`。

---

## 3. 阶段总览

| 阶段 | 天数 | 核心交付 | 删除 | 新增 |
|------|------|---------|------|------|
| Phase 1：基础管线 | ~7 | 脏标记 → ProjectorPacket → Component.Apply | ~72 类 | ~18 类 |
| Phase 2：表现层 | ~5 | 插值 + 模型加载 + 特效 + 动画 | — | — |
| Phase 3：裁剪 + 网络 | ~4 | 规则链 + NetworkTransport | — | — |
| Phase 4：回滚 | ~3 | 快照回滚 + Component 粒度 Flash | — | — |
| Phase 5：优化（可选） | ~2 | ProjectState 扁平化 | — | — |

**总计 ~21 天**。

---

## 4. Phase 1：基础管线（~7 天）

> 验收：Logic 改 `SpatialInfo.position`，下一帧 `SpatialComponent.position` 自动更新。`[Lifecycle]` 类的 Reset 零手写。

### T1.1 注解定义 + Source Generator 框架（1 天）

- [ ] 定义 `[Project(index, default)]` 字段级 Attribute
- [ ] 定义 `[Lifecycle]` 类级 Attribute
- [ ] 搭建 `Goblin.SourceGenerator` 项目
- [ ] 实现 Generator 入口：扫描 partial class + Attribute → 产出 `.g.cs`

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.1，`BEHAVIORINFO_LIFECYCLE_REPORT.md` §4
**产出**：`ProjectAttribute.cs` / `LifecycleAttribute.cs` / `GoblinSourceGenerator.cs`

---

### T1.2 BehaviorInfo 基类钩子（0.5 天）

- [ ] 新增 `Reset()` 方法：`ResetFields()` → `OnReset()` → `actor=0; active=false`
- [ ] 新增 `ResetFields()` — `internal virtual`，空实现，Source Generator 填
- [ ] 新增 `OnReset()` — `protected virtual`，空实现，用户覆写
- [ ] 新增 `projectDirtyMask`（`internal ulong`）
- [ ] 现有手写 `OnReset/OnReady/OnClone` 暂时保留，T1.11 才替换

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.4.1，`BEHAVIORINFO_LIFECYCLE_REPORT.md` §5
**产出**：`BehaviorInfo.cs`（修改）

---

### T1.3 属性 + 脏标记生成（1 天）

- [ ] 为 `[Project(index)]` 字段生成 backing field + 属性 getter/setter
- [ ] setter 注入脏标记：值变 → `projectDirtyMask |= (1ul << index)` → `Stage.RegisterDirty(this)`
- [ ] 生成 `TakeProjectValues(mask)` — 只取 mask 标记的字段到 `object[]`
- [ ] 生成 `ClearProjectDirty()` — `projectDirtyMask = 0`
- [ ] 处理 `default` 值（`[Project(index: 2, default: 1)]`）
- [ ] 值类型序列化：FPVector3 → 3×long，FP → long，bool → byte，enum → int

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.4.2
**产出**：Source Generator 属性生成逻辑

---

### T1.4 生命周期生成（1.5 天）

- [ ] `[Lifecycle]` 类生成 `internal override void ResetFields()`：
  - 值类型 → default 值（尊重 `[Project(default: x)]`）
  - ProjectorDict/ProjectorList → 调 `container.Reset()`（清数据不还池）
  - 裸容器 → 遍历元素 Reset → Clear（不还池）
  - 引用类型 → `null`
  - `projectDirtyMask = 0`
- [ ] 生成 `internal void CloneFields(T src)`：
  - 值类型 → 直接赋值；容器 → `Clone()`
  - 直接写 backing field，不触发脏标记
- [ ] 生成 `public override BehaviorInfo Clone()`：`Ensure<T>()` → `CloneFields` → `Ready(actor)`
- [ ] 非 `[Lifecycle]` 类：不生成，`ResetFields()` 保持空实现

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.4.2-2.4.3，`BEHAVIORINFO_LIFECYCLE_REPORT.md` §5-6
**产出**：Source Generator ResetFields/CloneFields/Clone 生成逻辑

---

### T1.5 ProjectorDict / ProjectorList（1 天）

- [ ] `ProjectorDict<K,V>`：自追踪（addedKeys/removedKeys/changedKeys）、`CollectDiff()`、`Reset()`、`Clone()`
- [ ] `ProjectorList<T>`：同理，跟踪 addedIndices/removedIndices
- [ ] 写入即记账：新增/修改/删除自动记录，增删同一 key 自动抵消

**输入**：`PROPERTY_SYNC_DESIGN.md` §2.3
**产出**：`ProjectorDict.cs` / `ProjectorList.cs`

---

### T1.6 ProjectorSystem（1 天）

- [ ] 全局脏集 `HashSet<BehaviorInfo> dirtyInfos`
- [ ] `Tick()`：遍历脏集 → 读 `projectDirtyMask` → `TakeProjectValues` → 产出 `ProjectorPacket[]` → 清脏
- [ ] 集合 Diff 收集：对有 ProjectorDict/List 字段且 mask 位为 1 的，调 `CollectDiff()`
- [ ] 快照管理（预留 Phase 4）：`TakeSnapshot` / `CloneSnapshot`
- [ ] Actor 移除：`RmvActor(actor)` 清理快照
- [ ] `Stage.RegisterDirty(BehaviorInfo)` — 属性 setter 自动调用

**输入**：`PROPERTY_SYNC_DESIGN.md` §3
**产出**：`ProjectorSystem.cs` / `ProjectorPacket.cs`

---

### T1.7 Crop 接口 + GodRule（0.5 天）

- [ ] `IProjectionRule`：`ulong Filter(ProjectorPacket, Observer, ulong currentMask)`
- [ ] `Crop`：规则链串联，mask == 0 丢弃
- [ ] `GodRule`：全通过（零裁剪，Phase 1 所有 Observer 挂此）
- [ ] `Observer` + `ObserverType` 枚举（Player/Spectator/GM/Replay/AI/Editor）

**输入**：`PROPERTY_SYNC_DESIGN.md` §4
**产出**：`IProjectionRule.cs` / `Crop.cs` / `Observer.cs` / `ObserverPacket.cs`

---

### T1.8 Transport 接口 + LocalTransport（0.5 天）

- [ ] `IPropertyTransport`：`void Send(List<ObserverPacket>)`
- [ ] `LocalTransport`：直接调 `RenderWorld.Apply()`
- [ ] 计算 `latency`（帧同步恒 0~1）

**输入**：`PROPERTY_SYNC_DESIGN.md` §6
**产出**：`IPropertyTransport.cs` / `LocalTransport.cs`

---

### T1.9 Entity + Component + RenderWorld（1 天）

- [ ] `Entity`：actor/comps 字典/GetComp/AddComp/RmvComp/Destroy
- [ ] `Component` 基类：entity/actor 属性；`abstract Apply(mask, values)`；`OnCreate/OnDestroy`；`PushHistory`（ring buffer 2 帧）
- [ ] `RenderWorld`：entities 字典 + behaviorToComp 映射表；`Apply()` — Ensure Entity → Ensure Component → PushHistory；`RmvEntity()`；事件钩子
- [ ] 用户手写首批 Component：`SpatialComponent`、`TickerComponent`
- [ ] Source Generator 生成 `Apply` 方法：按 mask 位将 values 写入 Component 字段

**输入**：`PROPERTY_SYNC_DESIGN.md` §7
**产出**：`Entity.cs` / `Component.cs` / `RenderWorld.cs` / `SpatialComponent.cs` / `TickerComponent.cs`

---

### T1.10 删除 RIL 体系 + Agent/Enchant/Invoker/Chase（0.5 天）

> ⚠️ **确认 T1.9 链路跑通后再删。**

- [ ] 删 RIL 体系（~40 类）：IRIL 及子类、Translator 及子类、RILSync/RILDispatch/RILCache/RILCross/IRIL_DIFF/RIL_DEFINE/RILSalute/Salute
- [ ] 删 Render 层（~32 类）：Agent 体系 / Enchant 体系 / Invoker 体系 / Chase 体系 / Batch/Bucket 体系
- [ ] 清理 Behavior/Render 层残留引用

**输入**：`PROPERTY_SYNC_DESIGN.md` §7.2/§9
**产出**：删除 ~72 文件/类

---

### T1.11 `[Lifecycle]` 类迁移（0.5 天）

按复杂度分 4 批迁移 24 个 BehaviorInfo 子类：

| 批次 | 类 | 特征 | 风险 |
|------|-----|------|------|
| 1 | TickerInfo, MovementInfo, MagicInfo | 纯值类型，1-2 字段 | 零 |
| 2 | SpatialInfo, StateMachineInfo, SkillLauncherInfo 等 | 值类型 + struct | 低 |
| 3 | TagInfo, GamepadInfo | 1-2 层容器 | 中 |
| 4 | FacadeInfo, StageInfo, FlowCollisionInfo 系列 | 深层嵌套容器 | 高 |

每批操作：类加 `[Lifecycle]` + `partial` → 删手写 OnReady/OnReset/OnClone → 验证 Reset/Clone 正确。

随批次 4 自然修复 3 个已知 Bug：
- FlowCollisionInfo.OnClone 硬编码子类类型 → SG 用 `Ensure<实际类型>()`
- FlowCollisionHurtInfo 子类字段未 Reset → `[Lifecycle]` 接管全部字段
- OnReady 调 OnReset 反模式 → 容器不还池

**输入**：`BEHAVIORINFO_LIFECYCLE_REPORT.md` §2/§8
**产出**：24 个 BehaviorInfo 子类迁移完成，72 个手写方法归零

---

### Phase 1 任务依赖

```
T1.1（注解 + SG 框架）
 │
 ├── T1.2（基类钩子）
 │     │
 │     ├── T1.3（属性 + 脏标记生成）
 │     │     │
 │     │     ├── T1.6（ProjectorSystem）
 │     │     │     │
 │     │     │     ├── T1.7（Crop + GodRule）
 │     │     │     │     │
 │     │     │     │     └── T1.8（Transport）
 │     │     │     │           │
 │     │     │     │           └── T1.9（Entity/Component/RenderWorld）
 │     │     │     │                 │
 │     │     │     │                 └── T1.10（删除 RIL + Agent）
 │     │     │     │
 │     │     │     └── T1.5（ProjectorDict/List）
 │     │     │
 │     │     └── T1.4（生命周期生成）
 │     │
 │     └── T1.11（[Lifecycle] 类迁移）
```

**关键路径**：T1.1 → T1.2 → T1.3 → T1.6 → T1.7 → T1.8 → T1.9 → T1.10（8 步，~6 天）
**可并行**：T1.4 与 T1.5 在 T1.3 之后并行推进

---

## 5. Phase 2：表现层（~5 天）

> 验收：角色移动平滑插值，模型加载正常，特效跟随集合变更。

### T2.1 ProjectionStrategy：插值与预测（1.5 天）

- [ ] Component 基类新增 `OnExpress(float dt)` 虚方法
- [ ] PushHistory ring buffer 扩充至 4 帧
- [ ] 自动判断时间方向：`frame < renderFrame` → 插值，`frame > renderFrame` → 预测，`frame == renderFrame` → 直接 Apply
- [ ] Jitter Buffer 自适应窗（latency 稳定 → 小窗，抖动 → 大窗）
- [ ] 平滑修正：`correctionDelta = (serverValue - current) * smoothFactor`
- [ ] 阈值 Snap：误差过大直接跳正

**输入**：`PROPERTY_SYNC_DESIGN.md` §5
**产出**：`Component.cs`（修改）/ `ProjectionStrategy.cs`

---

### T2.2 SpatialComponent 插值（0.5 天）

- [ ] `OnExpress` 中 position lerp + rotation slerp
- [ ] t = (renderTime - lastFrameTime) / (nextFrameTime - lastFrameTime)

**产出**：`SpatialComponent.cs`（修改）

---

### T2.3 FacadeComponent：模型加载（1 天）

- [ ] 监听 modelid 变更 → 异步加载 .tscn/.glb
- [ ] 加载完成实例化挂到 Entity Node3D
- [ ] modelid 变更 → 销毁旧模型、加载新模型

**产出**：`FacadeComponent.cs`

---

### T2.4 EffectComponent：特效创建/回收（1 天）

- [ ] 监听 effectdict 的 addedKeys/removedKeys/changedKeys
- [ ] added → 创建特效实例，removed → 回池，changed → 更新参数
- [ ] 特效对象池管理

**产出**：`EffectComponent.cs` / `EffectPool.cs`

---

### T2.5 AnimationComponent：动画推进（0.5 天）

- [ ] 监听 animstate/animhash/animticktype
- [ ] 状态变化 → 播动画，手动 ticktype → 手动设置 progress
- [ ] Crossfade 带 blend time

**产出**：`AnimationComponent.cs`

---

### T2.6 分层 Express（0.5 天）

- [ ] 定义 Phase A(Spatial) → B(Animation) → C(Facade) → D(Effect) 执行顺序
- [ ] Component 间依赖声明

**产出**：`RenderWorld.cs`（修改：分层 Tick）

---

## 6. Phase 3：裁剪 + 网络（~4 天）

> 验收：敌方只看位置不看 HP，网络序列化正确，状态同步链路跑通。

### T3.1 裁剪规则实现（1.5 天）

- [ ] `AOIRule`：距离过滤，超半径返回 0
- [ ] `PermissionRule`：(关系, behaviorType) → 允许的 fieldmask。敌方 mask 掉 hp
- [ ] `VisibilityRule`：草丛/隐身标记，不可见返回 0
- [ ] `FrequencyRule`：每个字段独立推送间隔
- [ ] Observer 工厂：按 ObserverType 组装规则链

**输入**：`PROPERTY_SYNC_DESIGN.md` §4.4
**产出**：AOIRule/PermissionRule/VisibilityRule/FrequencyRule/ObserverFactory

---

### T3.2 NetworkTransport + 序列化（1.5 天）

- [ ] `NetworkTransport`：按 ObserverPacket 序列化（actor+frame+behaviorType+fieldmask+values）
- [ ] SG 生成 Serialize/Deserialize
- [ ] 接收端 `RemoteTransport` → 反序列化 → `RenderWorld.Apply()`

**输入**：`PROPERTY_SYNC_DESIGN.md` §6.3
**产出**：`NetworkTransport.cs` / `RemoteTransport.cs` / SG 序列化生成

---

### T3.3 状态同步预测（1 天）

- [ ] Player Observer 输入预测：客户端基于快照 + 本地输入死推算
- [ ] 服务端确认 → 误差平滑修正/snap

**输入**：`PROPERTY_SYNC_DESIGN.md` §5.3/§5.7
**产出**：`ProjectionStrategy.cs`（修改）/ `PredictionState.cs`

---

## 7. Phase 4：回滚（~3 天）

> 验收：帧同步 rollback 时 Component 粒度 Flash，不回滚的 Component 不受影响。

### T4.1 ProjectorSystem 快照回滚（1 天）

- [ ] `TakeSnapshot` / `CloneSnapshot` — 仅 `[Project]` 字段
- [ ] 回滚时取出目标帧快照 → 恢复 BehaviorInfo → 重新 Tick
- [ ] 清理超出回滚窗口的快照

**产出**：`ProjectorSystem.cs`（修改）/ `BehaviorInfoSnapshot.cs`

---

### T4.2 RenderWorld 回滚（1 天）

- [ ] 记录回滚窗口内每个帧的 `(actor, behaviorType, fieldmask, values)`
- [ ] Rollback 时标记受影响 Entity → Component
- [ ] 只 Flash 标记为 dirty 的 Component

**产出**：`RenderWorld.cs`（修改）

---

### T4.3 事件幂等（1 天）

- [ ] 事件关联 frame：`lastProcessedFrame` 去重
- [ ] 回滚时回放事件（非重复不重播）

**产出**：事件系统修改

---

## 8. Phase 5：优化（可选，~2 天）

### T5.1 ProjectState 扁平 struct（1 天）

- [ ] 将 `[Project]` 字段打包为 struct
- [ ] 快照/序列化 memcpy 量级，消除 object[] 装箱

**产出**：`ProjectState` struct + SG 生成逻辑修改

---

### T5.2 嵌套对象支持（0.5 天）

- [ ] `[ProjectNested]` 注解
- [ ] SG 生成嵌套对象递归 Reset/Clone/Serialize

**产出**：`ProjectNestedAttribute` + SG 扩展

---

### T5.3 性能验证（0.5 天）

- [ ] 1000 Entity 场景压力测试
- [ ] 脏集遍历开销测量
- [ ] 序列化带宽测量
- [ ] 边缘 case 覆盖（空脏集、全脏、Actor 快速创建销毁）

---

## 9. 风险与注意事项

| 风险 | 等级 | 缓解 |
|------|------|------|
| Source Generator 调试困难 | 中 | T1.1 产空文件验证框架，T1.3 增量加逻辑 |
| 删除 RIL 后行为回归 | 高 | T1.10 放在链路跑通后，逐类删逐次编译 |
| 容器迁移遗漏嵌套 Reset | 中 | 批次 3/4 用 FacadeInfo 作金丝雀测试 |
| 性能不达预期（object[] 装箱） | 低 | Phase 5 flat struct 解决，Phase 1 不追求 |

### 铁律

1. **T1.10 删除必须放最后**：ProjectorSystem + Entity/Component 全链路跑通前，不动旧代码
2. **每步编译通过再进下一步**：Source Generator 阶段尤其
3. **批次 4 迁移前先修 Bug**：FlowCollisionInfo 的 Clone 硬编码 Bug 在迁移时自然修复，但应先有测试覆盖
4. **FPVector3 `==` 确认有重载**：属性 setter 中值比较依赖 `==` 操作符，如无则需用 Equals

---

## 10. 文档索引

| 文档 | 定位 |
|------|------|
| `CORE.md` | 哲学底座 |
| `PROPERTY_SYNC_DESIGN.md` | Property Sync 体系完整设计 |
| `BEHAVIORINFO_LIFECYCLE_REPORT.md` | BehaviorInfo 生命周期自动化分析 |
| `IMPLEMENTATION_PLAN.md`（本文） | 实施任务拆解与依赖 |
