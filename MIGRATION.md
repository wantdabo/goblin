# Goblin: Unity → Godot 4 C# 迁移计划

## Context

项目是一个动作游戏框架，当前基于 Unity + URP。架构设计良好，Logic/Render 层有明确分离。
目标：迁移到 Godot 4.x (C#)，范围为运行时（不含 Timeline 编辑器工具），单人执行，在同一 repo 内新建 Godot 项目目录。

核心优势：Logic 层（~182 脚本）是纯 C#，可直接复用。主要工作量集中在 Render 层和 UI 层。

---

## 阶段划分

### Phase 1 — 项目骨架搭建（1-2 周）

**目标**：建立 Godot 项目，跑通引擎初始化。

**任务：**
1. 在 repo 根目录新建 `godot/` 子目录，创建 Godot 4 C# 项目，配置 .NET 8
2. 将以下目录整体复制到 Godot 项目（零改动或极少改动）：
   - `Assets/Scripts/Goblin/Core/` → `godot/Scripts/Goblin/Core/`
   - `Assets/Scripts/Goblin/Common/` → `godot/Scripts/Goblin/Common/`（除 U3DKit.cs）
   - `Assets/Scripts/Goblin/Gameplay/Logic/` → `godot/Scripts/Goblin/Gameplay/Logic/`
   - `Assets/Scripts/Goblin/Gameplay/RIL/` → `godot/Scripts/Goblin/Gameplay/RIL/`
   - `Assets/Scripts/Goblin/Phases/` → `godot/Scripts/Goblin/Phases/`（去掉 YooAsset 依赖）
   - `Config/` → `godot/Config/`（Luban 生成的 C# 代码直接用）
3. 替换 Unity 专属引用（Common 层共 9 个文件有 UnityEngine 依赖，逐一处理）：
   - `Ticker.cs`：`Mathf.Max` → `Math.Max`（1 处）
   - `Random.cs`：`Mathf.Cos/Sin/Deg2Rad` → `Math` 等价方法，`Vector2` → `FPVector2`
   - `Engine.cs`、`Config.cs`：`#if UNITY_WEBGL` → `#if GODOT_WEB`
   - `FSM/Machine.cs`：移除 `using UnityEngine`（仅 import，无实际使用）
   - `GameRes.cs`、`Location.cs`：Phase 2 重写，此处跳过
   - `Sounds/Sound.cs`、`Sounds/SoundInfo.cs`：Phase 6 重写，此处跳过
   - `Gamepad.cs`（InputSystem 自动生成）：Phase 2 重写，此处跳过
   - `Gameplay/Logic` 中 3 个文件（SpatialPositionData、CollisionData、BeHitData）：移除 `using UnityEngine.Serialization`
4. 修改 `Entrance.cs`，继承 Godot `Node` 替换 `MonoBehaviour`：
   ```csharp
   public override void _Ready() => Export.Init(...);
   public override void _Process(double delta) => Export.Tick((float)delta);
   public override void _PhysicsProcess(double delta) => Export.FixedTick((float)delta);
   ```
5. 验证：项目能编译，`Export.Init()` 能调用

**关键文件：**
- `Assets/Scripts/Entrance.cs` → 修改继承和生命周期
- `Assets/Scripts/Goblin/Core/Export.cs` → 直接复用
- `Assets/Scripts/Goblin/Core/Engine.cs` → 移除 `using UnityEngine`，改条件编译
- `Assets/Scripts/Goblin/Gameplay/Director/LocalDirector.cs` → 确认线程退出方式（Thread.Abort 未发现，但需检查 Thread.Interrupt/Join）

---

### Phase 2 — 资产系统 & 输入适配（1-2 周）

**目标**：替换 YooAsset，适配输入系统，为 Render 层搭建做准备。

**任务：**

**2a. 资产系统**
1. 重写 `GameRes.cs` 适配 Godot 资源系统：
   - 模型加载：`ResourceLoader.Load<PackedScene>(path).Instantiate()`
   - 音效加载：`ResourceLoader.Load<AudioStream>(path)`
   - 配置加载：直接读取 `Config/Cfg/Bytes/` 二进制文件（Luban 不变）
2. 重写 `Location.cs` 路径映射（`res://` 路径体系）
3. 将 3D 资产导入 Godot：
   - `Assets/UERes/Model/` → `godot/Assets/Model/`（.fbx 直接支持）
   - `Assets/UERes/Texture/` → `godot/Assets/Texture/`
   - `Assets/UERes/Sound/` → `godot/Assets/Sound/`
4. Phases 系统去掉 YooAsset（ResPhase、HotfixPhase 的资源下载逻辑替换为 Godot 本地加载）
5. 删除 YooAsset 相关脚本（`Assets/Scripts/Yoo/` 7 个文件不迁移）

**2b. 输入系统**（提前到此阶段，Phase 3 Render 测试需要）
- `Gamepad.inputactions` → 在 Godot InputMap 中重新配置（触屏 / 手柄 / 键鼠三套映射）
- `Assets/Scripts/Goblin/Gameplay/Logic/BehaviorInfos/Gamepad.cs` → 适配 Godot `Input` API
- `Render/Common/InputSystem.cs` → 重写为 Godot 输入读取

**关键文件：**
- `Assets/Scripts/Goblin/Common/GameRes/GameRes.cs` → 重写
- `Assets/Scripts/Goblin/Common/GameRes/Location.cs` → 重写路径映射
- `Assets/Scripts/Goblin/Gameplay/Logic/BehaviorInfos/Gamepad.cs` → 改写输入读取
- `Assets/Scripts/Goblin/Gameplay/Render/Common/InputSystem.cs` → 重写

---

### Phase 3 — Render 层重建（4-6 周）

**目标**：用 Godot Node3D 体系重建 Render 层，保留 Agent/Batch/Resolver 架构。

**注意**：Spine 动画暂不支持，AnimationAgent 先用 Godot `AnimationPlayer` 实现基础动画。

**任务：**

**3a. Core（1 周）**
- `Render/Core/Agent.cs` → 保留抽象类，去掉 Unity 引用
- `Render/Core/Batch.cs` → 保留抽象类，移除 `Unity.Jobs`
- `Render/Core/World.cs` → 保留逻辑，替换 ObjectPool 的 GameObject 为 Node3D

**3b. Agents（2-3 周）**
- `NodeAgent.cs`：`GameObject` → `Node3D`，`transform` → `node.Transform`
- `ModelAgent.cs`：`ObjectPool<GameObject>` → `ObjectPool<Node3D>`，加载改用 `PackedScene`
- `AnimationAgent.cs`：Animancer → `AnimationPlayer`（Spine 后续再补）
- `EffectAgent.cs`：粒子系统 → Godot `GpuParticles3D`

**3c. Cameras（3-5 天）**
- `Cameras/Eyes.cs`：Cinemachine FreeLook → 自定义 Godot `Camera3D` 控制器
  - 保留原有的跟随/旋转逻辑，去掉 Cinemachine 依赖

**3d. Batches（1 周）**
- `SpatialBatch.cs`：移除 `IJobParallelForTransform`，改为普通 C# 循环
  - 性能优化后期可用 `WorkerThreadPool` 替代

**3e. Resolvers（1 周）**
- `Resolvers/Cross/`、`Resolvers/Enchants/`、`Resolvers/Salutes/` → 保留逻辑，替换 Unity API

**3f. Debug 渲染（1-2 天）**
- `RendererFeatures/DrawPhysRendererFeature.cs`：URP ScriptableRenderPass → Godot `DebugDraw3D` 或自定义 `ImmediateMesh`

**关键文件（全部需要改写）：**
- `Assets/Scripts/Goblin/Gameplay/Render/Agents/NodeAgent.cs`
- `Assets/Scripts/Goblin/Gameplay/Render/Agents/ModelAgent.cs`
- `Assets/Scripts/Goblin/Gameplay/Render/Agents/AnimationAgent.cs`
- `Assets/Scripts/Goblin/Gameplay/Render/Batches/SpatialBatch.cs`
- `Assets/Scripts/Goblin/Gameplay/Render/Cameras/Eyes.cs`
- `Assets/Scripts/Goblin/RendererFeatures/DrawPhysRendererFeature.cs`

---

### Phase 4 — UI 系统重建（待定）

**目标**：重构为 MVVM 架构，用 Godot Control/CanvasLayer 替换 UGUI，支持触屏 / 手柄 / PC 三端输入。

#### 现状

当前是 MVP + 事件总线，View 直接调用 Proxy 方法，耦合较重。
MVVM 具体设计方案待补充（用户将提供更完整的框架设计理念）。

#### 已确定的约束

- 支持三端输入：触屏、手柄、PC（键鼠）
- 输入方式切换时 UI 焦点/导航逻辑需要适配
- Godot 4 的 `CanvasLayer` 替换 Unity Canvas，`Control` 替换 RectTransform

#### 待补充

- MVVM 分层设计（等用户提供框架理念后补充）
- 数据绑定方案
- 三端输入的 UI 导航设计
- 具体任务拆分和时间估算

**关键文件：**
- `Assets/Scripts/Goblin/Sys/` 全部 24 个脚本
- `Assets/Scripts/Goblin/Common/U3DKit.cs` → 完全重写适配 Godot，改名为 `GDKit.cs`

---

### Phase 5 — 网络层验证（3-5 天）

**目标**：验证网络系统正常工作。

**任务：**
1. 网络层（LiteNetLib + MessagePack）：纯 C# 库，直接复用，无需改动
2. WebSocket（UnityWebSocket）→ 替换为 Godot 内置 `WebSocketPeer` 或 LiteNetLib WebSocket

**关键文件：**
- `Assets/Scripts/Goblin/Common/Network/` → 直接复用（LiteNetLib）
- `Assets/Scripts/Goblin/Common/Network/NetWebSocket.cs` → 替换 UnityWebSocket

---

### Phase 6 — 集成测试 & 性能调优（2-3 周）

**目标**：跑通完整战斗循环，达到可玩状态。

**任务：**
1. 搭建主场景（替换 `Main.unity`）
2. 验证战斗循环：角色加载 → 技能释放 → 碰撞检测 → 伤害计算
3. Shader 迁移：
   - `Assets/UERes/Shader/` 中的 HLSL → 改写为 Godot GLSL（`.gdshader`）
   - 后处理效果 → Godot `WorldEnvironment` + `Environment` 资源
4. Sound 系统：`AudioSource` → `AudioStreamPlayer3D`，对象池逻辑保留
5. 性能分析，必要时用 `WorkerThreadPool` 优化 SpatialBatch

---

## 工作量汇总

| 阶段 | 内容 | 预估时间 |
|------|------|---------|
| Phase 1 | 项目骨架 + 纯逻辑迁移 | 1-2 周 |
| Phase 2 | 资产系统 + 输入适配 | 1-2 周 |
| Phase 3 | Render 层重建 | 4-6 周 |
| Phase 4 | UI 系统重建（MVVM，三端输入） | 待定 |
| Phase 5 | 网络层验证 | 3-5 天 |
| Phase 6 | 集成测试 & 调优 | 2-3 周 |
| **总计** | | **12-18 周（单人，不含 Phase 4）** |

---

## 可直接复用（零改动或 <5 行修改）

- `Scripts/Goblin/Core/Export.cs`
- `Scripts/Goblin/Core/Comp.cs`
- `Scripts/Goblin/Gameplay/Logic/` 全部（182 脚本，其中 3 个仅需移除 UnityEngine.Serialization import）
- `Scripts/Goblin/Gameplay/Logic/Common/Math/` 全部（FP 定点数学库，纯 C#）
- `Scripts/Goblin/Common/Parallel/` 全部（自定义协程）
- `Scripts/Goblin/Common/FSM/`（Machine.cs 移除空 UnityEngine import）
- `Scripts/Goblin/Common/Eventor.cs`
- `Scripts/Goblin/Common/ObjectPool.cs`
- `Scripts/Goblin/Common/Network/`（LiteNetLib + MessagePack）
- `Config/` 全部（Luban 配置 + 生成代码）
- 所有 3D 模型、贴图、音频资产

## 需要完全重写

- `Scripts/Goblin/Gameplay/Render/Agents/` 全部
- `Scripts/Goblin/Gameplay/Render/Batches/SpatialBatch.cs`
- `Scripts/Goblin/Gameplay/Render/Cameras/Eyes.cs`
- `Scripts/Goblin/Gameplay/Render/Common/InputSystem.cs`
- `Scripts/Goblin/RendererFeatures/DrawPhysRendererFeature.cs`
- `Scripts/Goblin/Sys/` UI 绑定部分
- `Scripts/Goblin/Common/GDKit.cs`（原 U3DKit.cs，改名重写）
- `Scripts/Goblin/Common/GameRes/GameRes.cs`
- `Scripts/Goblin/Common/GameRes/Location.cs`
- `Entrance.cs`（改继承，不改文件名）
- 所有 UGUI Prefab（→ Godot .tscn 场景）
- 所有 HLSL Shader（→ Godot .gdshader）

## 不迁移 / 丢弃

- `Assets/Scripts/Yoo/`（YooAsset，7 个文件）
- HybridCLR 热更新（不再需要）
- Spine 动画（暂不支持，后续再评估）
- `Assets/Scripts/Misc/`（Odin Inspector 相关，编辑器工具）
- `Pipeline.Timeline/`（编辑器工具，不在运行时范围内）

---

## 验证方式

每个阶段结束的验收标准：
- Phase 1：`dotnet build` 无错误，引擎 Init/Tick 可调用
- Phase 2：能在 Godot 场景中加载一个 3D 模型，输入事件能触发
- Phase 3：角色能在场景中显示、播放动画、响应输入移动
- Phase 4：Login/Lobby UI 能正常显示和交互（三端输入均可操作）
- Phase 5：本地对战能建立连接
- Phase 6：完整战斗循环可运行，帧率稳定
