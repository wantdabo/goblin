# 音效系统设计

> 2026-07-19 | 基于 Goblin 框架架构

---

## 一、背景与约束

### 1.1 核心挑战

Goblin 是格斗游戏框架，支持**游戏全局变速**（加速/减速）和 **HitLag 顿帧**。音效必须在变速期间保持与画面的同步，这是本设计的核心目标。

### 1.2 变速机制回顾

```
每个 Actor 的最终时间流速：
  tick = Stage.timescale × TickerInfo.timescale × LOGIC_TICK

  Stage.timescale     — 全局变速（慢镜头、加速）
  TickerInfo.timescale — Actor 独立变速（HitLag 顿帧、Buff 加减速）

HitLag 实现：
  HitEffect.AddHitLag() → ticker.timescale -= strength  （冻结，引用计数叠加）
  HitEffect.RmvHitLag()  → ticker.timescale = 原始值    （恢复）
```

### 1.3 已就绪的基础设施

时间缩放信息**已经在每帧传入 Render 层的 Agent**：

```
Logic: TickerTranslator → RIL_TICKER.timescale
  ↓
Render: World.OnTick() → Agent.Chase(tick, timescale)
```

音效系统接入后，**零额外管道成本**即可获得实时变速信息。

### 1.4 音效系统的两层需求

格斗游戏中，音效天然分两类，需要不同的架构承载：

| 分类 | 来源 | 空间 | 变速跟随 | 音量控制 |
|------|------|------|----------|----------|
| **玩法音效** (SFX) | Logic → RIL → SoundAgent | 3D 空间音频 | ✅ 跟随 Actor timescale | Master × SFX |
| **系统音频** (BGM/UI) | 系统层直接调用 | 2D 无衰减 | ❌ 不受游戏变速影响 | Master × BGM |

```
玩法音效：伤害音、受击音、技能音、脚步 → per-Actor，由 Logic 层驱动
系统音频：BGM、菜单音、提示音        → 全局，由系统层驱动
```

---

## 二、变速音效方案对比

| 方案 | 原理 | 效果 | 结论 |
|------|------|------|------|
| **pitch_scale** | Godot 内置，调播放速率 | 慢放→低沉，快放→尖细 | ✅ 采用 |
| AudioEffectPitchShift | 粒状合成，保音高 | 慢放音高不变，CPU 高 | ❌ 不需要 |
| AudioServer 全局变速 | 改动 `playback_speed` | 全局生效，无法区分角色 | ❌ 不适合 |

### 为什么 pitch_scale 是正确答案

格斗游戏中，HitLag 时音效变低沉是**期望的美学效果**——慢镜头配合低频轰鸣，玩家潜意识中已经将其与"命中的重量感"绑定。Godot 内置 `AudioStreamPlayer3D.pitch_scale` 完美支持，几乎零 CPU 开销。

如果未来有极端场景（如 10x 慢放需要保音高），可追加 AudioEffectPitchShift 作为备选方案，不影响当前架构。

---

## 三、架构设计

### 3.1 整体分层

```
Export.engine
│
├── .soundmanager ← [重构] SoundManager（替代现有 Sound）
│   ├── BGM 播放器    (AudioStreamPlayer, 2D, → BGM Bus)
│   ├── UI 音效池     (AudioStreamPlayer, 2D, → SFX Bus)
│   ├── 音量/静音控制 (AudioBus 路由)
│   └── SoundConfig   (soundid → 资源路径查表，供 SoundAgent 共用)
│
├── .sound  ← [移除] 旧 Sound 模块，逻辑并入 SoundManager
│
└── [Gameplay 流程]
    └── World
        ├── WorldRoot (Node3D)      ← 视觉（已有）
        ├── SoundRoot (Node3D)      ← 音频（新增）
        ├── ModelPool  (Node3D)     ← 模型池（已有）
        │
        └── SoundAgent : Agent  ← [新增] per-Actor
            ├── 空间音效池 (AudioStreamPlayer3D, → SFX Bus)
            ├── 节点挂 SoundRoot 下，靠 GlobalPosition 定位
            ├── 跟随 timescale 调 pitch_scale
            └── 由 RIL_EVENT_SOUND 驱动
```

### 3.2 场景树结构

参照现有 `WorldRoot` / `ModelPool` 模式，新增 `SoundRoot`（Node3D）：

```
SceneTree.Root
├── WorldRoot (Node3D)              ← 视觉渲染（已有）
│   ├── SunLight
│   ├── WorldEnv
│   ├── Floor
│   ├── [ModelAgent] 角色模型节点
│   ├── [EffectAgent] 特效节点
│   └── [PrimitiveMeshAgent] 基元网格
├── SoundRoot (Node3D)              ← 音频（新增）
│   └── [SoundAgent] AudioStreamPlayer3D × N
├── ModelPool (Node3D, Visible=false)
└── Camera3D
```

World.OnCreate 中创建：

```csharp
// 参照 worldroot / modelpool 的创建模式
soundroot = new Node3D { Name = "SoundRoot" };
sceneRoot?.AddChild(soundroot);
SoundAgent.SetRoot(soundroot);
```

SoundAgent 使用与 ModelAgent 相同的静态注入模式：

```csharp
// SoundAgent.cs
private static Node3D soundroot { get; set; }
public static void SetRoot(Node3D root) => soundroot = root;

// Play() 时挂载
var player = new AudioStreamPlayer3D();
soundroot?.AddChild(player);
```

### 3.3 Godot AudioBus 布局

AudioBus 在 Godot 编辑器中手动配置（`default_bus_layout.tres`），SoundManager 启动时只缓存索引：

```
[Master Bus]         ← 全局音量
├── [BGM Bus]        ← BGM 专用
│   └── AudioStreamPlayer (BGM)
└── [SFX Bus]        ← 音效专用
    ├── AudioStreamPlayer3D (玩法 SFX，来自 SoundAgent)
    └── AudioStreamPlayer (UI 音效，来自 SoundManager)
```

**操作接口（Godot 内置 API）**：

| 操作 | API |
|------|-----|
| 设置总线音量 | `AudioServer.SetBusVolumeDb(index, db)` |
| 静音总线 | `AudioServer.SetBusMute(index, true)` |
| 获取总线索引 | `AudioServer.GetBusIndex("BGM")` |

SoundManager 初始化时缓存 busIndex：

```csharp
void CacheBusIndices()
{
    masterBusIdx = AudioServer.GetBusIndex("Master");
    bgmBusIdx = AudioServer.GetBusIndex("BGM");
    sfxBusIdx = AudioServer.GetBusIndex("SFX");
}
```

### 3.4 数据流全景

```
┌─ Logic Layer ─────────────────────────────────────────────────────────┐
│                                                                        │
│  Flow Executor（DamageExecutor / ChangeStateExecutor）                  │
│    → 直接创建 RIL_EVENT_SOUND                                           │
│        var e = ObjectCache.Ensure<RIL_EVENT_SOUND>();                  │
│        e.actor    = target;                                            │
│        e.soundid  = SOUND_DEFINE.HIT_CONFIRM;                         │
│        e.mode     = SoundMode.OneShot;                                 │
│        stage.rilsync.Send(e);                                          │
│                                                                        │
└────────────────────┬───────────────────────────────────────────────────┘
                     │  RIL 传输
                     ▼
┌─ Render Layer ─────────────────────────────────────────────────────────┐
│                                                                        │
│  RILBucket → ProcessEventQueue()                                       │
│    → salutedict[RIL_DEFINE.EVENT_SOUND] → SoundSalute.OnSalute(e)     │
│    → world.EnsureAgent<SoundAgent>(e.actor).Play(e)                   │
│                                                                        │
│  SoundAgent : Agent                                                    │
│    ├── OnChase(tick, timescale)                                        │
│    │     → 每帧：pitch_scale = timescale                               │
│    │     → 每帧：GlobalPosition = SpatialAgent.position                │
│    │     → 自动回收已播完的非循环音效                                     │
│    ├── Play(RIL_EVENT_SOUND)  → 创建 AudioStreamPlayer3D               │
│    │     → soundroot.AddChild(player)                                   │
│    │     → player.Bus = "SFX"                                           │
│    │     → player.Play()                                                │
│    ├── Stop(uint soundid)     → mode=Stop 时触发                        │
│    └── OnReset()              → Actor 销毁时清理所有音效节点              │
│                                                                        │
└────────────────────────────────────────────────────────────────────────┘
```

```
┌─ System Layer（不经过 RIL 管道）───────────────────────────────────────┐
│                                                                        │
│  Engine.soundmanager                                                   │
│    ├── PlayBGM("bgm_battle.ogg")   → AudioStreamPlayer (→ BGM Bus)    │
│    ├── StopBGM()                                                        │
│    ├── PlayUI("click.wav")         → AudioStreamPlayer (→ SFX Bus)    │
│    ├── SetMasterVolume(0.8f)       → AudioServer.SetBusVolumeDb(...)  │
│    ├── SetBGMVolume(0.6f)                                                 │
│    ├── SetSFXVolume(1.0f)                                                 │
│    ├── MuteBGM(true)               → AudioServer.SetBusMute(...)      │
│    └── MuteSFX(false)                                                    │
│                                                                        │
└────────────────────────────────────────────────────────────────────────┘
```

### 3.5 与现有模式的对照

#### RIL 事件流（玩法 SFX）

| 概念 | Damage 事件 | Sound 事件 |
|------|------------|-----------|
| Logic 创建 | `ObjectCache.Ensure<RIL_EVENT_DAMAGE>()` | `ObjectCache.Ensure<RIL_EVENT_SOUND>()` |
| Logic 发送 | `stage.rilsync.Send(eventdamage)` | `stage.rilsync.Send(eventsound)` |
| RIL_DEFINE | `EVENT_DAMAGE = 1` | `EVENT_SOUND = 4` |
| RIL 类 | `RIL_EVENT_DAMAGE : IRIL_EVENT` | `RIL_EVENT_SOUND : IRIL_EVENT` |
| Render 分发 | `salutedict[1] → DamageSalute` | `salutedict[4] → SoundSalute` |
| Render 消费 | UI 飘字事件 | `SoundAgent.Play()` |

#### 场景树节点注入（与 ModelAgent 一致）

| Agent | 根节点 | 注入方式 |
|-------|--------|----------|
| ModelAgent | `WorldRoot` | `ModelAgent.SetRoot(worldroot)` |
| EffectAgent | `WorldRoot` | `EffectAgent.SetRoot(worldroot)` |
| PrimitiveMeshAgent | `WorldRoot` | `PrimitiveMeshAgent.SetRoot(worldroot)` |
| **SoundAgent** | **SoundRoot** | `SoundAgent.SetRoot(soundroot)` |

#### 系统音频（不经过 RIL）

系统音频直接通过 `Engine.soundmanager` 调用，**不进入 RIL 管道**：

```
用法举例：
  engine.soundmanager.PlayBGM("bgm_stage1.ogg");  // 进入对局时
  engine.soundmanager.PlayUI("click.wav");          // UI 按钮点击
  engine.soundmanager.MuteBGM(true);                // 玩家关闭 BGM
```

---

## 四、核心类设计

### 4.1 SoundManager（全局，挂 Engine）

职责：BGM 播放、UI 音效、音量/静音控制、soundid 查表。**不参与 RIL 管道**。

```
位置：godot/Scripts/Goblin/Common/Sounds/SoundManager.cs
继承：Comp
挂载：engine.soundmanager = AddComp<SoundManager>(); soundmanager.Create();
```

核心接口：

```
// ── BGM ──
PlayBGM(string res)                  // 播放 BGM（v1：直接切换；v2：交叉淡入）

StopBGM()                            // 停止 BGM

// ── UI 音效 ──
PlayUI(string res)                   // 播放 UI 音效

// ── 音量控制 ──
SetMasterVolume(float volume)        // 0.0 - 1.0
SetBGMVolume(float volume)           // 0.0 - 1.0
SetSFXVolume(float volume)           // 0.0 - 1.0

// ── 静音控制 ──
MasterMuted { get; set; }
BGMMuted { get; set; }
SFXMuted { get; set; }

// ── 初始化 ──
CacheBusIndices()                    // 缓存 AudioBus 索引
```

内部状态：

```
int masterBusIdx { get; set; }
int bgmBusIdx { get; set; }
int sfxBusIdx { get; set; }

SoundSettings settings { get; set; }              // 音量/静音数据

AudioStreamPlayer bgmPlayer { get; set; }         // BGM 专用（→ BGM Bus）
List<AudioStreamPlayer> uiPool { get; set; }       // UI 音效对象池

// soundid → 资源路径查表（与 SoundAgent 共用）
// 来源：Luban 配置表
Dictionary<uint, SoundConfig> soundConfigs { get; set; }
```

### 4.2 SoundAgent（per-Actor，挂 World）

职责：空间音效播放、跟随 timescale 调 pitch、位置同步。**由 RIL_EVENT_SOUND 驱动**。

```
位置：godot/Scripts/Goblin/Gameplay/Render/Agents/SoundAgent.cs
继承：Agent
挂载：world.EnsureAgent<SoundAgent>(actor)
```

核心逻辑：

```
// ── 静态注入 ──
private static Node3D soundroot;
public static void SetRoot(Node3D root) => soundroot = root;

// ── 每帧 ──
OnChase(tick, timescale):
    foreach active player:
        player.PitchScale = timescale                  // 实时跟随变速
        player.GlobalPosition = spatial.position        // 跟随角色位置
    检查已播完的非循环音效 → soundroot.RemoveChild + 回收

// ── 播放 ──
Play(RIL_EVENT_SOUND e):
    switch e.mode:
        SoundMode.OneShot / Loop:
            从 SoundManager.soundConfigs 查表获取资源路径
            engine.gameres.LoadAssetSync<AudioStream>(path)
            创建 AudioStreamPlayer3D
            player.Bus = "SFX"
            soundroot?.AddChild(player)
            player.Play()
        SoundMode.Stop:
            Stop(e.soundid)

// ── 停止 ──
Stop(uint soundid):
    查找 soundid 对应的循环音效
    player.Stop()
    soundroot.RemoveChild(player)
    回收

// ── 销毁 ──
OnReset():
    停止并清理所有活跃音效节点
```

每帧执行流程：

```
World.OnTick(tick)
  → agent.Chase(tick, timescale)          // timescale 从 RIL_TICKER 提取
    → SoundAgent.OnChase(tick, timescale)
        → foreach active player:
            player.PitchScale = timescale        // 实时跟随变速
            player.GlobalPosition = spatialpos    // 跟随角色位置
            if OneShot && !playing → 回收
```

**关键设计**：pitch 不是播放时设一次——而是在 `OnChase` 中**每帧更新**。这意味着：

- HitLag 开始 → 正在播放的受击音 pitch 实时降低
- HitLag 结束 → 同一个音效的 pitch 实时恢复
- 玩家感受到的是无缝的"慢镜头音效"，不是断层

### 4.3 SoundSettings（数据容器）

```
位置：godot/Scripts/Goblin/Common/Sounds/SoundSettings.cs
```

音量/静音偏好的纯数据容器，由 SoundManager 读写。后续可接入 Luban 配置或本地持久化。

```
public class SoundSettings
{
    public float mastervolume { get; set; } = 1.0f;   // 0.0 - 1.0
    public float bgmvolume { get; set; } = 1.0f;      // 0.0 - 1.0
    public float sfxvolume { get; set; } = 1.0f;      // 0.0 - 1.0
    public bool mastermuted { get; set; } = false;
    public bool bgmmuted { get; set; } = false;
    public bool sfxmuted { get; set; } = false;
}
```

### 4.4 SoundConfig（配置数据）

```
位置：godot/Scripts/Goblin/Common/Sounds/SoundConfig.cs
```

soundid 到资源的映射，由 Luban 配置表生成，SoundManager 和 SoundAgent 共用。

soundid 按分段管理，代码中不硬编码判断，纯粹是配置管理约定：

| 范围 | 分类 | 说明 |
|------|------|------|
| 0000000 - 9999999 | SFX | 玩法音效（3D 空间，1000 万） |
| 10000000 - 19999999 | BGM | 背景音乐（2D，1000 万） |
| 20000000 - 29999999 | UI | UI 音效（2D，1000 万） |

```
public class SoundConfig
{
    public uint soundid { get; set; }       // 唯一 ID（按分段分配）
    public string res { get; set; }         // 资源路径（相对于 soundpath）
    public SoundCategory category { get; set; }  // SFX / BGM / UI
    public float defaultvolume { get; set; }     // 默认音量（备用，当前由 AudioBus 控制）
}

public enum SoundCategory : byte
{
    SFX = 0,   // 玩法音效（3D 空间）
    BGM = 1,   // 背景音乐（2D）
    UI = 2,    // UI 音效（2D）
}
```

### 4.5 旧模块处理

现有 `Common/Sounds/Sound.cs` 和 `SoundInfo.cs`：

- **SoundManager 替代 Sound.cs**：Play/Stop/Load/Unload 逻辑移入 SoundManager
- **SoundInfo.cs 移除**：`AudioStreamPlayer3D` 的创建逻辑移入 SoundAgent，SoundInfo 不再需要
- **`engine.sound` 属性保持兼容**：短期内让 `engine.sound` 转发到 `engine.soundmanager`，后续可移除

---

## 五、RIL 管道详细设计

### 5.1 RIL_DEFINE 新增常量

```csharp
// RIL_DEFINE.cs，追加在下一条

/// <summary>
/// RIL_EVENT 音效
/// </summary>
public const ushort EVENT_SOUND = 4;
```

### 5.2 RIL_EVENT_SOUND 定义

```csharp
// godot/Scripts/Goblin/Gameplay/Logic/RIL/EVENT/RIL_EVENT_SOUND.cs

using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.Common;

namespace Goblin.Gameplay.Logic.RIL.EVENT;

/// <summary>
/// RIL 事件 - 音效事件
/// </summary>
public class RIL_EVENT_SOUND : IRIL_EVENT
{
    public override ushort id => RIL_DEFINE.EVENT_SOUND;

    /// <summary>
    /// 发出音效的 Actor
    /// </summary>
    public ulong actor { get; set; }
    /// <summary>
    /// 音效配置 ID
    /// </summary>
    public uint soundid { get; set; }
    /// <summary>
    /// 模式, 参考 SoundMode
    /// OneShot=0 / Loop=1 / Stop=2
    /// </summary>
    public byte mode { get; set; }

    protected override void OnReset()
    {
        actor = 0;
        soundid = 0;
        mode = 0;
    }

    protected override void OnClone(IRIL_EVENT clone)
    {
        if (clone is not RIL_EVENT_SOUND e) return;

        e.actor = actor;
        e.soundid = soundid;
        e.mode = mode;
    }
}
```

> 注意：不带 volume 字段。SFX 音量由 AudioBus 统一控制（全局偏好），不是 per-event 数据。

### 5.3 SoundMode 枚举

```csharp
// godot/Scripts/Goblin/Gameplay/Logic/Common/Defines/SoundMode.cs

namespace Goblin.Gameplay.Logic.Common.Defines;

/// <summary>
/// 音效播放模式
/// </summary>
public enum SoundMode : byte
{
    /// <summary>
    /// 一次性播放，播完自动回收
    /// </summary>
    OneShot = 0,
    /// <summary>
    /// 循环播放，需要显式 Stop
    /// </summary>
    Loop = 1,
    /// <summary>
    /// 停止当前 Actor 上指定 soundid 的循环音效（per-instance，非全局同名）
    /// </summary>
    Stop = 2,
}
```

### 5.4 SoundSalute

```csharp
// godot/Scripts/Goblin/Gameplay/Render/Resolvers/Salutes/SoundSalute.cs

using Goblin.Gameplay.Logic.Common.Defines;
using Goblin.Gameplay.Logic.RIL.EVENT;
using Goblin.Gameplay.Render.Agents;
using Goblin.Gameplay.Render.Resolvers.Common;

namespace Goblin.Gameplay.Render.Resolvers.Salutes;

public class SoundSalute : RILSalute<RIL_EVENT_SOUND>
{
    protected override void OnSalute(RIL_EVENT_SOUND e)
    {
        var agent = rilbucket.world.EnsureAgent<SoundAgent>(e.actor);
        agent.Play(e);
    }
}
```

### 5.5 注册到 RILBucket

```csharp
// RILBucket.cs → Salutes()
Salute<SoundSalute>(RIL_DEFINE.EVENT_SOUND);
```

### 5.6 Logic 端调用示例

```csharp
// 一次音效（受击）
var e = ObjectCache.Ensure<RIL_EVENT_SOUND>();
e.actor    = target;
e.soundid  = SOUND_DEFINE.HIT_CONFIRM;
e.mode     = (byte)SoundMode.OneShot;
stage.rilsync.Send(e);

// 循环音效（蓄力开始）
var e = ObjectCache.Ensure<RIL_EVENT_SOUND>();
e.actor    = self;
e.soundid  = SOUND_DEFINE.CHARGE_LOOP;
e.mode     = (byte)SoundMode.Loop;
stage.rilsync.Send(e);

// 循环音效（蓄力结束）
var e = ObjectCache.Ensure<RIL_EVENT_SOUND>();
e.actor    = self;
e.soundid  = SOUND_DEFINE.CHARGE_LOOP;
e.mode     = (byte)SoundMode.Stop;
stage.rilsync.Send(e);
```

---

## 六、音效分类总览

| 类型 | 调用入口 | 播放器 | AudioBus | 变速跟随 | 空间 | 示例 |
|------|----------|--------|----------|----------|------|------|
| **OneShot SFX** | RIL_EVENT_SOUND (Logic) | AudioStreamPlayer3D (SoundAgent) | SFX Bus | ✅ pitch_scale | 3D | 刀光、受击、跳跃 |
| **Loop SFX** | RIL_EVENT_SOUND (mode=Loop) | AudioStreamPlayer3D (SoundAgent) | SFX Bus | ✅ pitch_scale | 3D | 蓄力、持续 Buff |
| **Stop SFX** | RIL_EVENT_SOUND (mode=Stop) | — | — | — | — | 停止上述循环 |
| **BGM** | SoundManager.PlayBGM() | AudioStreamPlayer | BGM Bus | ❌ | 2D | 战斗 BGM、菜单 BGM |
| **UI 音效** | SoundManager.PlayUI() | AudioStreamPlayer | SFX Bus | ❌ | 2D | 按钮、提示音 |

---

## 七、实现清单

### 第一阶段：系统层重构

| # | 内容 | 文件 | 说明 |
|---|------|------|------|
| 1 | 新建 `SoundSettings` | `Common/Sounds/SoundSettings.cs` | 音量/静音数据容器 |
| 2 | 新建 `SoundConfig` | `Common/Sounds/SoundConfig.cs` | soundid 映射、SoundCategory 枚举 |
| 3 | 在 Godot 编辑器配置 AudioBus | `default_bus_layout.tres` | Master / BGM / SFX 三条总线 |
| 4 | 新建 `SoundManager : Comp` | `Common/Sounds/SoundManager.cs` | BGM、UI 音效、音量控制、CacheBusIndices |
| 5 | 重构 `engine.sound` → `engine.soundmanager` | `Engine.cs` | 挂载 SoundManager，旧属性转发兼容 |
| 6 | 移除或标记废弃 | `Common/Sounds/Sound.cs`、`SoundInfo.cs` | 逻辑已移入 SoundManager + SoundAgent |

### 第二阶段：RIL 管道（Logic 侧）

| # | 内容 | 文件 | 说明 |
|---|------|------|------|
| 7 | 定义 `SoundMode` 枚举 | `Logic/Common/Defines/SoundMode.cs` | OneShot / Loop / Stop |
| 8 | 新增 `EVENT_SOUND` 常量 | `RIL_DEFINE.cs` | `= 4` |
| 9 | 定义 `RIL_EVENT_SOUND` | `RIL/EVENT/RIL_EVENT_SOUND.cs` | 实现 `OnReset`/`OnClone` |

### 第三阶段：Render 端

| # | 内容 | 文件 | 说明 |
|---|------|------|------|
| 10 | World 创建 `SoundRoot` 节点 | `Render/Core/World.cs` | `soundroot = new Node3D { Name = "SoundRoot" }` |
| 11 | `SoundAgent.SetRoot(soundroot)` | `World.cs` OnCreate | 与 ModelAgent.SetRoot 相同模式 |
| 12 | 实现 `SoundAgent : Agent` | `Render/Agents/SoundAgent.cs` | AudioStreamPlayer3D 池，pitch 跟随，位置同步 |
| 13 | 实现 `SoundSalute` | `Render/Resolvers/Salutes/SoundSalute.cs` | 分发到 SoundAgent |
| 14 | 注册 Salute | `RILBucket.Salutes()` | `Salute<SoundSalute>(RIL_DEFINE.EVENT_SOUND)` |

### 第四阶段：Logic 端触发 + 配置

| # | 内容 | 文件 | 说明 |
|---|------|------|------|
| 15 | Luban 音效配置表 | Config Excel | soundid → 资源路径、SoundCategory |
| 16 | Executor 内发射音效 | `DamageExecutor.cs` 等 | 伤害、受击、状态切换时调用 |

---

## 八、关键设计决策

| 决策 | 理由 |
|------|------|
| 音效分两层：RIL 管道（玩法 SFX）+ 直接调用（系统音频） | BGM 和 UI 音效不受 Logic 层控制、不跟变速、不需要 per-actor |
| RIL_EVENT_SOUND 不带 volume 字段 | SFX 音量由 AudioBus 统一控制，是全局偏好，不是 per-event 数据 |
| SoundMode 含 Stop（非独立 RIL 事件） | 同一条 RIL 类型，mode 区分——保持 RIL 类型数量少，SoundSalute 代码统一 |
| 使用 Godot AudioBus 而非手写音量乘法 | 引擎内置、零 CPU 开销、支持静音/独奏、代码量更少 |
| AudioBus 在编辑器中手动配置，运行时只缓存索引 | 这是 Godot 标准做法；AudioBus 是项目资源文件，不应代码创建 |
| SoundManager 挂 Engine 上 | 全局可访问，BGM 生命周期与游戏进程一致 |
| SoundAgent 挂 World 中 | per-Actor 生命周期，随 Stage 创建/销毁；正常流程 Logic 应先发 Stop，OnReset 是兜底 |
| 新增 SoundRoot（Node3D），与 WorldRoot 平级 | 视觉和音频节点分离管理；参照 WorldRoot / ModelPool 的既有模式 |
| SoundAgent 通过 SetRoot 静态注入根节点 | 与 ModelAgent、EffectAgent 完全一致的注入模式 |
| BGM 交叉淡入为 v2 | v1 直接 Stop→Play，降低首版复杂度；交叉淡入依赖 Tween 管理，可后续追加 |
| pitch_scale 而非 AudioEffect | 格斗游戏美学：慢镜低沉是 feature 不是 bug |
| 每 Actor 独立 SoundAgent | timescale 是 per-actor 的，HitLag 只影响被击中的角色 |
| AudioStreamPlayer3D 而非 2D（玩法 SFX） | 格斗游戏需要空间感（左右声道区分远近） |
| AudioStreamPlayer 2D（BGM/UI） | 不需要空间衰减，全局一致 |

---

## 九、不做的部分

| 项目 | 原因 |
|------|------|
| AudioEffect 滤镜链 | 当前阶段不需要 EQ/混响等后期处理 |
| 多普勒效应 | 格斗游戏角色移动速度不产生可感知的多普勒效应 |
| 运行时音频裁剪/编辑 | 超出框架范围 |
| 回滚音效处理 | 声音不能撤销；格斗游戏通过压短回滚窗口规避；网络模块定型后再议 |
| 音效优先级/驱逐 | 当前规模不需要，AudioStreamPlayer 池上限即可 |

---

## 十、回滚与音效

当前不做回滚音效处理，原因：

- **声音不能「撤销」**：预测帧播放了受击音效，回滚后无法收回
- **格斗游戏通用做法**：压短回滚窗口 → 丢失帧不可感知
- **Goblin 当前阶段**：帧同步/状态同步最终方案未定，提前处理是过度设计

待网络模块定型后，可选方案：延迟播放已确认帧的音效、或确保所有音效长度远小于回滚窗口。

---

## 十一、已确认决策

| # | 决策 | 结论 |
|---|------|------|
| 1 | Loop 音效在 Stage 销毁时的行为 | Stage 销毁 → World 销毁 → SoundAgent.OnReset() 清理所有音效节点。Loop 音效应在**正常流程结束前**由 Logic 层发 SoundMode.Stop 主动关闭；Stage 销毁时的清理是兜底，不做渐弱处理 |
| 2 | SoundMode.Stop 的粒度 | **per-instance**（Actor + soundid 组合），不是全局同名。停止 Actor A 上的蓄力音效不影响 Actor B 上同名的蓄力音效 |
| 3 | soundid 分段 | SFX: 0000000-9999999 / BGM: 10000000-19999999 / UI: 20000000-29999999（每类 1000 万，8 位 uint）。代码不校验范围，纯配置管理约定 |
