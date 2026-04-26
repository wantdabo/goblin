# Goblin Framework — CLAUDE.md

Goblin 是一个基于 Godot 4 (C#) 的动作游戏框架，从 Unity 迁移而来。
Logic 层是纯 C#（确定性逻辑，可复用），Render 层依赖 Godot API。

---

## 目录结构

```
godot/
  Scripts/
    Entrance.cs                        # Godot Node 入口，驱动 Export.Tick/FixedTick
    Goblin/
      Core/          Comp, Engine, Export
      Common/        GDKit, Eventor, Ticker, ObjectPool, GameRes, FSM, Parallel, Network, Conf
      Phases/        Phase, LoginPhase, GamingPhase (FSM 阶段管理)
      Sys/           UI 系统、Proxy/Model 业务层
      Gameplay/
        Logic/       纯 C# 确定性逻辑层（零 Godot 依赖，不要修改）
        Render/      Godot 渲染层
        Director/    导演层（连接 Logic ↔ Render）
  GameRes/           资源文件（.tscn, .glb, .json, .wav 等）
```

---

## 核心层 (Core)

### Comp — 组件基类
所有框架对象都继承 `Comp`。提供组件树管理。

```csharp
comp.engine    // 全局 Engine 引用（所有 Comp 都能访问）
comp.parent    // 父组件
comp.Create()  // 触发 OnCreate()
comp.Destroy() // 触发 OnDestroy()，递归销毁子组件
comp.AddComp<T>()   // 创建并挂载子组件
comp.GetComp<T>()   // 获取子组件（取最新一个）
comp.RmvComp(comp)  // 移除子组件
```

### Engine — 引擎根组件
单例，通过 `Export.engine` 访问。持有所有顶层服务：

| 属性 | 类型 | 说明 |
|------|------|------|
| `eventor` | `Eventor` | 全局事件总线 |
| `ticker` | `Ticker` | 帧驱动器 |
| `gdkit` | `GDKit` | Godot API 封装 |
| `gameres` | `GameRes` | 资源加载 |
| `cfg` | `Config` | Luban 配置表 |
| `net` | `NetNode` | 网络（TCP/WebSocket） |
| `sound` | `Sound` | 音效 |
| `proxy` | `GameProxy` | 业务 Proxy 集合 |
| `gameui` | `GameUI` | UI 管理器 |
| `phase` | `Phase` | 游戏阶段 FSM |

### Export — 对外入口
```csharp
Export.Init()          // 创建 Engine（在 Entrance._Ready 中调用）
Export.Tick(delta)     // 驱动 Ticker.Tick（在 _Process 中调用）
Export.FixedTick(delta)// 驱动 Ticker.FixedTick（在 _PhysicsProcess 中调用）
```

---

## 帧驱动流程

```
Entrance._Process(delta)
  └─ Export.Tick(delta)
       └─ engine.ticker.Tick(delta)
            ├─ 驱动所有 Timing 计时器
            ├─ eventor.Tell(TickEvent)      → 所有监听者（UI、Director 等）
            └─ eventor.Tell(LateTickEvent)  → 后处理监听者

Entrance._PhysicsProcess(delta)
  └─ Export.FixedTick(delta)
       └─ engine.ticker.FixedTick(delta)
            ├─ eventor.Tell(FixedTickEvent)
            └─ eventor.Tell(FixedLateTickEvent)

GameplayDirector（监听 engine.ticker.eventor）
  OnTick(TickEvent)
    └─ world.ticker.Tick(e.tick)   → 驱动 World 内所有 Agent.Chase()
  OnFixedTick(FixedTickEvent)      → 单线程模式下驱动 OnStep()
  [多线程模式] Thread.OnStep()     → 独立线程以固定 16ms 步长驱动逻辑
```

---

## Common 层

### Eventor — 事件总线
```csharp
eventor.Listen<T>(Action<T> func)    // 订阅
eventor.UnListen<T>(Action<T> func)  // 取消订阅
eventor.Tell<T>(T e)                 // 派发
```
事件必须实现 `IEvent` 接口（struct）。

### Ticker — 帧驱动器
```csharp
engine.ticker.eventor.Listen<TickEvent>(OnTick)
engine.ticker.Timing(action, duration, loop)  // 计时器，loop=-1 无限循环
engine.ticker.StopTimer(id)
engine.ticker.timeScale  // 全局时间缩放
```

### ObjectPool — 对象池
```csharp
ObjectPool.Get<T>(key)       // 取出（可能为 null）
ObjectPool.Ensure<T>(key)    // 取出或 new()
ObjectPool.Set(obj, key)     // 归还
// 容器专用（带容量）：
ObjectPool.Ensure<List<T>>(capacity)
ObjectPool.Ensure<Dictionary<K,V>>(capacity)
```
**规则**：所有频繁创建的对象（Agent、RIL、Command、集合）都必须走 ObjectPool。

### GDKit — Godot API 封装
```csharp
engine.gdkit.GetNode<T>(node)           // 获取节点自身或其脚本组件
engine.gdkit.GetNode<T>(node, path)     // 精准路径查找（"Parent/Child"）
engine.gdkit.SeekNode<T>(node, name)    // 模糊递归查找（按名称）
engine.gdkit.GetLookInput()             // 视角输入（手柄右摇杆 + 鼠标）
engine.gdkit.GetScrollInput()           // 滚轮输入
```

---

## Sys 层（UI + 业务）

### UI 架构
```
GameUI（管理器）
  ├─ CanvasLayer UIMain   (Layer=0)
  ├─ CanvasLayer UIAlert  (Layer=1)
  └─ CanvasLayer UITop    (Layer=2)

UIBaseView（View 基类，继承 UIBase<T>）
  └─ UIBaseCell（Cell 基类，子组件）
```

**View 生命周期**：
```
engine.gameui.Open<MyView>()
  → Load()：加载 .tscn，OnLoad() → OnBuildUI() → OnBindEvent()
  → Open()：设置 Layer/Sorting，OnOpen()，node.Visible=true
engine.gameui.Close(view)
  → Close()：OnClose()，node.Visible=false，Unload()，QueueFree()
```

**View 开发规范**：
```csharp
public class MyView : UIBaseView
{
    public override UILayer layer => UILayer.UIMain;
    protected override string res => "Folder/MyView";  // GameRes/UIPrefab/ 下的相对路径

    private Label myLabel { get; set; }  // 所有成员用 { get; set; }

    protected override void OnBuildUI()
    {
        base.OnBuildUI();
        myLabel = engine.gdkit.SeekNode<Label>(node, "MyLabel");  // 查找节点
    }

    protected override void OnBindEvent()
    {
        base.OnBindEvent();
        AddUIEventListener("MyBtn", () => { /* 点击回调 */ });
    }
}
```

### Proxy/Model 模式
```
GameProxy
  ├─ engine.proxy.initialize  → InitializeProxy
  ├─ engine.proxy.login       → LoginProxy
  ├─ engine.proxy.lobby       → LobbyProxy
  └─ engine.proxy.gameplay    → GameplayProxy
```
每个 `Proxy<T>` 持有对应的 `Model`（数据）。业务逻辑写在 Proxy，数据写在 Model。

---

## Gameplay/Director 层

### GameplayDirector（抽象基类）
连接 Logic 层（Stage）和 Render 层（World）。

```csharp
director.CreateGame(data, multithread)  // 创建游戏（multithread=true 开独立线程）
director.StartGame() / StopGame()
director.PauseGame() / ResumeGame()
director.Snapshot() / Restore()         // 快照/回滚
director.world                          // 渲染层 World
```

**多线程模式**：Logic 在独立线程以 16ms 固定步长运行，RIL 通过线程安全队列传递给主线程。

### LocalDirector（单机实现）
- `OnStep()`：读取输入 → `stage.Step()` → 逻辑帧推进
- `OnTick()`：消费 RIL 队列 → `world.rilbucket.SetRIL()`
- RIL 队列用 `lock` 保护线程安全

---

## Gameplay/Render 层

### World — 渲染世界
```csharp
world.sa          // Stage Actor ID（ulong.MaxValue）
world.self        // 当前玩家的 Actor ID
world.selfseat    // 当前玩家座位号
world.rilbucket   // RIL 数据桶
world.eyes        // 摄像机
world.input       // 输入系统
world.ticker      // 渲染层独立 Ticker（由 Director.OnTick 驱动）
world.EnsureAgent<T>(actor)  // 获取或创建 Agent
world.GetAgent<T>(actor)     // 获取 Agent
world.RmvAgent(actor)        // 移除 Actor 的所有 Agent
```

### Agent — 渲染代理（Chase 模式）
每个 Actor 可挂多个 Agent，每种 Agent 负责一个渲染维度。

```csharp
// 实现一个 Agent：
public class MyAgent : Agent
{
    protected override void OnReady()
    {
        WatchRIL<RIL_XXX>(OnRILXxx);  // 订阅关心的 RIL
    }
    protected override void OnReset() { /* 归还资源 */ }
    protected override bool OnArrived() { /* 返回 true 表示已追上目标状态 */ }
    protected override void OnFlash() { /* 快照恢复时瞬间同步 */ }
    private void OnRILXxx(RIL_XXX ril) => ChangeStatus(ChaseStatus.Chasing);
}
```

**Chase 机制**：Agent 收到 RIL 后进入 `Chasing` 状态，每帧调用 `OnArrived()` 检查是否追上目标，追上后进入 `Arrived` 停止更新。

### RILBucket — 渲染状态桶
接收来自 Logic 层的渲染指令（RIL），分发给 Agent 和 Enchant。

```csharp
rilbucket.SetRIL(ril)       // 设置状态（去重，hashcode 相同则丢弃）
rilbucket.SetDiff(diff)     // 合并差异状态（通过 RILCross）
rilbucket.SetEvent(e)       // 处理一次性事件（通过 RILSalute）
rilbucket.GetRIL<T>(actor)  // 读取状态
rilbucket.SeekRIL<T>(actor, out ril)  // 读取状态（带 bool 返回）
rilbucket.LossAllRIL()      // 清空所有状态
```

### AgentEnchant — 代理赋能
根据 RIL 的到来/消失，自动为 Actor 添加/移除对应 Agent。
例如：`NodeEnchant` 监听 `RIL_SPATIAL`，有则 `EnsureAgent<NodeAgent>`，无则 `RmvAgent`。

### Batch — 批处理
监听 `world.ticker` 的 Tick/LateTick，做批量计算（如 `SpatialBatch` 处理空间变换）。

---

## Gameplay/Logic 层（只读，不修改）

纯 C# 确定性逻辑，零 Godot 依赖。核心概念：

- `Stage`：逻辑世界，持有所有 Actor/Behavior/BehaviorInfo
- `RIL`（Render Instruction Layer）：Logic 向 Render 发送的渲染指令，通过 `stage.onril` 事件传出
- `Command`：玩家输入指令，通过 `stage.SetCommand()` 注入
- `Input`：摇杆/按键状态，通过 `stage.SetInput()` 注入

---

## 编码规范

1. **所有自定义类成员一律用 `{ get; set; }` 属性**，不用裸 public/private field
2. **命名全小写**：`private string mlayername { get; set; }` 而非 `mLayerName`
3. **节点查找**：用 `engine.gdkit.SeekNode<T>(node, name)`，不直接调 `node.FindChild(...)`
4. **对象池**：频繁创建的对象必须走 `ObjectPool.Ensure/Get/Set`
5. **Godot 对象有效性**：从 ObjectPool 取出的 Godot 节点必须用 `GodotObject.IsInstanceValid(node)` 检查
6. **资源路径**：模型 `res://GameRes/Model/`，特效 `res://GameRes/Effect/`，UI `res://GameRes/UIPrefab/`，动画配置 `res://GameRes/AnimCfg/`
7. **渲染器**：使用 `gl_compatibility`（OpenGL），不用 Vulkan。特效用 `CpuParticles3D`，不用 `GpuParticles3D`

---

## 关键路径速查

| 需求 | 入口 |
|------|------|
| 打开一个 UI | `engine.gameui.Open<MyView>()` |
| 关闭一个 UI | `engine.gameui.Close(view)` |
| 发送全局事件 | `engine.eventor.Tell(new MyEvent{...})` |
| 订阅帧事件 | `engine.ticker.eventor.Listen<TickEvent>(OnTick)` |
| 读取配置表 | `engine.cfg.location.HeroInfos.Get(id)` |
| 加载资源 | `engine.gameres.LoadAssetSync<PackedScene>(path)` |
| 查找 UI 节点 | `engine.gdkit.SeekNode<Label>(node, "NodeName")` |
| 读取渲染状态 | `world.rilbucket.GetRIL<RIL_SPATIAL>(actor)` |
| 获取/创建 Agent | `world.EnsureAgent<NodeAgent>(actor)` |
| 发送玩家输入 | `world.input.EnqueueCommand(cmd)` |
