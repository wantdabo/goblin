# TODO

## Bug：重复进入游戏时 GenActor 空引用

- **状态**：已修复
- **根因**：`Stage.Recycle()` 中内层字典被重复回池（`Clear + Set + Remove` 三步中 `Remove` 自动 `Reset + Set`，导致同一对象入池两次）
- **修复**：
  - `Stage.Recycle` 删除多余的 `Clear + Set`，仅靠 `GBLDict.Remove` 自动回池
  - BehaviorInfo 移除处用 `GBLList.RemoveSilent` 避免三重回池
  - `GBLList` 新增 `RemoveSilent` 方法（移除但不回池，用于跨容器场景）

## Bug：FacadeInfo 集合字段 NRE

- **状态**：已修复
- **根因**：SG 生成的 `GBLList`/`GBLDict` backing field 未初始化，`Reset` 设 `= default`（null）
- **修复**：SG 对 `GBLList<>`/`GBLDict<>` 类型生成 `= new()` 初始化器和 Reset 中的 `new` 重置

## 功能：Godot Pipeline 可视化编辑器

- **状态**：规划中
- **方向**：Scripting / Timeline / GraphNode 三种编辑方式并存，统一底层 Pipeline 数据结构

## 重构：Projection 层命名与职责重整

- **状态**：规划中
- **包含三件事**：

### 一、Component → Shadow / Mirror → Canvas 重命名

- **动机**："Component" 在 ECS / Unity / Godot 中各有语义，沟通成本高；"Canvas"+"Shadow" 形成完整的投影隐喻体系
- **命名链**：

| 当前 | → | 新 |
|------|---|-----|
| `Component` | → | `Shadow` |
| `Mirror` | → | `Canvas` |
| `SpatialComponent` / `FacadeComponent` / `HUDComponent` | → | `SpatialShadow` / `FacadeShadow` / `HUDShadow` |
| `IComponentApply<T>` | → | `IShadowApply<T>` |
| `[ProjectorTarget(typeof(X))]` | → | 删除（SG 生成） |
| `Mirror.Register<TInfo, TComp>()` | → | `Canvas.Register<TInfo, TShadow>()` |
| `Mirror.GetComp<T>()` | → | `Canvas.GetShadow<T>()` |

### 二、Mirror/Shadow 从 Render 移到 Projection

- **动机**：Mirror 是纯 C# 数据消费端，不依赖 Godot，本质是 ProjectorSystem 的另一端；Shadow 存的是 FP 定点数，放在 Render 目录里矛盾
- **目标分层**：

```
Gameplay/
├─ Logic/           纯 C# 确定性逻辑（零 Godot 依赖）
├─ Projection/       同步层 — 纯 C#，同步模式无关
│   ├─ Core/         ProjectorSystem, IProjectable
│   ├─ Rules/        Crop, AOI, Permission, Visibility, Frequency
│   ├─ Transport/    LocalTransport, NetworkTransport
│   ├─ Canvas.cs     数据画布 — 接收侧数据中心
│   └─ Shadows/      SpatialShadow, FacadeShadow, HUDShadow
└─ Render/           客户端表现层 — Godot 专属
    ├─ VisualNode    读 Shadow → 驱动 Godot Node3D
    ├─ ModelLoader   .tscn/.glb 异步加载
    ├─ AnimationDriver
    └─ InputSystem
```

### 三、SG 自动生成 Shadow 类 + 注册映射

- **动机**：`[Projector]` 已完整描述投影字段的 name/type/index，手写 Shadow 是纯信息冗余；`[ProjectorTarget]` 同样是手写映射
- **SG 从 `[Projector]` 一站式生成**：
  1. BehaviorInfo 侧（已有）：backing field + dirty setter + IProjectable
  2. Shadow 类（新增）：字段 + `IShadowApply<T>` 的 `ApplyTo` 按 mask 位写字段
  3. 注册映射（新增）：`Canvas.Register<XxxInfo, XxxShadow>()` 放入启动代码
- **删除**：
  - `Render/Components/Component.cs` → 移为 `Projection/Shadows/Shadow.cs`
  - `Render/Components/SpatialComponent.cs` 等 → SG 生成
  - `Render/Components/IComponentApply.cs` → SG 消费
  - `Render/Components/ProjectorTargetAttribute.cs` → 不再需要

## 清理：删除 OnInitContainers 死代码

- **状态**：待处理
- **问题**：
  - SG `EmitOnReady` 生成 `protected override void OnInitContainers()`，但 BehaviorInfo 基类未定义该 virtual 方法
  - `Reset` 中 GBL 容器只 `Clear()` 不置 null，`OnInitContainers` 的 `if (null == field)` 永不会触发
  - 无任何调方 —— `Ready()` → `OnReady()`，SG 未生成 `OnReady` override 去调用它
- **清理内容**：
  - SG 删除 `EmitOnReady` 方法及 `EmitLifecycleCode` 中的调用
  - BehaviorInfo 基类如有偷偷定义的 `protected virtual void OnInitContainers()` 一并删除
  - 容器初始化由 `Reset` 的 `Clear()` 自行保证
