# 渲染层设计方案

> 状态：`Design`
>
> 2026-07-28
>
> 本文保留 Render 层设计草案，描述的是历史的 `Mirror/VisualNode` pull 模型。当前实现已采用 `Canvas/Shadow` 数据推送链路，详见 [ARCHITECTURE.md](ARCHITECTURE.md) §3.6；本文不作为当前 API 说明。

---

## 一、核心模型：状态同步

渲染层是一个**纯消费的状态同步终端**。它不知道也不关心 Logic 在哪里——本地进程、帧同步服务器、状态同步服务器。

```
                        可替换的 Transport 层
                       ┌───────────────────┐
  Logic (任意位置)  ──→ │ Local / TCP / UDP │ ──→ Mirror ──→ 表现层
                       └───────────────────┘    纯数据快照    消费数据
```

只要 Mirror 的数据模型一致，表现层不需要任何改动。单机开发和线上部署是同一套代码。

---

## 二、Mirror：纯数据快照

Mirror 是 Render 侧的数据中心，不包含任何表现逻辑。

```csharp
public class Mirror
{
    // ---- 核心数据 ----
    // ActorID → (ComponentType → Component 实例)
    internal Dictionary<ulong, Dictionary<Type, Component>> datas { get; }

    // ---- 生命周期标记 ----
    // 本帧收到过数据的 Actor（= 通过 AOI 裁剪，在观察者视野内）
    public HashSet<ulong> touchedactors { get; }

    // ---- 注册表 ----
    // BehaviorInfo 类型 → Component 类型 映射
    private Dictionary<Type, Type> infotocomp;

    // Component 类型 → ApplyTo 静态委托（零反射）
    private Dictionary<Type, Action<object, ulong, object[]>> applymap;

    // Component 类型 → 工厂委托（零反射创建）
    private Dictionary<Type, Func<Component>> factorymap;

    // ---- 数据消费 ----
    public void ApplyPackets(ObserverPacket[] packets)
    {
        foreach (var p in packets)
        {
            Apply(p.actor, p.behaviorinfotype, p.fieldmask, p.values);
            touchedactors.Add(p.actor);
        }
    }

    // ---- 查询接口 ----
    public T GetComp<T>(ulong actor) where T : Component;
    public bool HasActor(ulong actor);
}

// Component 纯数据基类
public abstract class Component : IGBL { }

// StageInfo 映射 — 提供存活 Actor 全集
[ProjectorTarget(typeof(StageInfo))]
public class StageComponent : Component
{
    public HashSet<ulong> actors { get; } = new();
}

// SpatialInfo 映射 — 位置/旋转/缩放
[ProjectorTarget(typeof(SpatialInfo))]
public class SpatialComponent : Component
{
    public FPVector3 position;
    public FPVector3 euler;
    public FP scale;
}

// FacadeInfo 映射 — 模型/动画/特效
[ProjectorTarget(typeof(FacadeInfo))]
public class FacadeComponent : Component
{
    public int model;
    public byte animticktype;
    public byte animstate;
    public uint animhash;
    public FP animelapsed;
    public uint effectincrement;
    public List<uint> rmveffects;
    public Dictionary<uint, EffectInfo> effectdict;
    public List<AnimationSlot> animslots;
}

// HUDInfo 映射 — UI 数据（不经过 SceneTree）
[ProjectorTarget(typeof(HUDInfo))]
public class HUDComponent : Component
{
    public int hp;
    public int maxhp;
    public int movespeed;
    public int attack;
}
```

---

## 三、RenderWorld：生命周期管理器

RenderWorld 的唯一职责：比对 Mirror 数据和自身注册表，决策 Godot 节点的创建/更新/隐藏/销毁。它不关心节点内部的渲染逻辑。

```csharp
public class RenderWorld
{
    // ActorID → 对应视觉节点
    private Dictionary<ulong, Node3D> active;      // 场景树上的活跃节点
    private Dictionary<ulong, Node3D> pooled;       // 离开 AOI 暂存的节点（隐藏状态）
    private HashSet<ulong> destroying;              // 等待死亡动画结束

    // Mirror 引用（由外部注入）
    private Mirror mirror;

    // 工厂：根据 FacadeComponent.model 决定实例化哪个场景
    private Func<int, PackedScene> resourceLoader;

    // ---- 每帧入口（OnLateTick 中 ApplyProjection 后调用）----
    public void OnPostUpdate()
    {
        var alive   = mirror.GetComp<StageComponent>(Stage.SA)?.actors
                      ?? new HashSet<ulong>();
        var visible = mirror.touchedactors;

        // 四分支决策

        // CREATE: alive && visible && 本地没有
        foreach (var actor in visible)
        {
            if (alive.Contains(actor) && false == active.ContainsKey(actor))
            {
                Spawn(actor);
            }
        }

        // HIDE: alive && !visible && 本地有
        foreach (var actor in active.Keys)
        {
            if (alive.Contains(actor) && false == visible.Contains(actor))
            {
                Pool(actor);
            }
        }

        // DESTROY: !alive && 本地有
        foreach (var actor in active.Keys)
        {
            if (false == alive.Contains(actor))
            {
                Destroy(actor);
            }
        }

        // UPDATE 不在这里做——VisualNode 自己在 _Process 里 pull Mirror
    }

    // ---- 四条分支的具体操作 ----

    private void Spawn(ulong actor)
    {
        Node3D node;

        // 优先从池取
        if (pooled.TryGetValue(actor, out var cached))
        {
            node = cached;
            pooled.Remove(actor);
            node.Visible = true;
        }
        else
        {
            // 工厂创建：读 FacadeComponent.model 决定用哪个场景
            var facade = mirror.GetComp<FacadeComponent>(actor);
            var scene = facade != null
                ? resourceLoader(facade.model)
                : resourceLoader(defaultModel);
            node = scene.Instantiate<Node3D>();
        }

        // 注入 Mirror 引用 + actor ID，节点自己用
        if (node is IVisualNode vn)
        {
            vn.mirror = mirror;
            vn.actor = actor;
        }

        sceneRoot.AddChild(node);
        active[actor] = node;
    }

    private void Pool(ulong actor)
    {
        if (active.TryGetValue(actor, out var node))
        {
            node.Visible = false;
            pooled[actor] = node;
            active.Remove(actor);
        }
    }

    private void Destroy(ulong actor)
    {
        if (active.TryGetValue(actor, out var node))
        {
            if (node is IVisualNode vn)
                vn.OnDestroyed(() => FinalizeDestroy(actor, node));
            else
                FinalizeDestroy(actor, node);
        }
    }

    private void FinalizeDestroy(ulong actor, Node3D node)
    {
        node.QueueFree();
        pooled.Remove(actor);
        destroying.Remove(actor);
    }
}
```

---

## 四、VisualNode：自驱型视觉节点

每个 VisualNode 持有 Mirror 引用，在 Godot 帧循环里自己从 Mirror pull 数据。RenderWorld 不调它们的更新方法。

```csharp
// 标记接口：RenderWorld 通过这个注入 Mirror + actor
public interface IVisualNode
{
    Mirror mirror { get; set; }
    ulong actor { get; set; }
    void OnDestroyed(Action onComplete);
}

// 角色视觉节点示例
public partial class HeroVisual : Node3D, IVisualNode
{
    public Mirror mirror { get; set; }
    public ulong actor { get; set; }

    private AnimationPlayer animator;
    private Dictionary<uint, Node3D> effects;

    public override void _Process(double delta)
    {
        if (null == mirror) return;

        // Pull: 拉取 SpatialComponent → 驱动 Transform
        var spatial = mirror.GetComp<SpatialComponent>(actor);
        if (spatial != null)
        {
            Position = spatial.position.ToGodot();
            Rotation = spatial.euler.ToGodot();
            Scale    = spatial.scale.ToGodot();
        }

        // Pull: 拉取 FacadeComponent → 驱动动画和特效
        var facade = mirror.GetComp<FacadeComponent>(actor);
        if (facade != null)
        {
            // 模型变更
            if (facade.model != lastModel) ApplyModel(facade.model);

            // 动画变更
            if (facade.animhash != lastAnimHash)
            {
                animator?.Play(facade.animhash);
                lastAnimHash = facade.animhash;
            }

            // 特效增量
            ApplyEffectDeltas(facade);
        }
    }

    public void OnDestroyed(Action onComplete)
    {
        animator?.Play("death");
        // 动画结束后回调
        onComplete();
    }
}

// 魔法体/子弹视觉节点
public partial class MagicVisual : Node3D, IVisualNode
{
    public Mirror mirror { get; set; }
    public ulong actor { get; set; }

    public override void _Process(double delta)
    {
        var spatial = mirror?.GetComp<SpatialComponent>(actor);
        if (spatial != null)
        {
            Position = spatial.position.ToGodot();
            Rotation = spatial.euler.ToGodot();
        }
    }

    public void OnDestroyed(Action onComplete)
    {
        // 魔法体直接消失，不播动画
        onComplete();
    }
}
```

---

## 五、Component 到 VisualNode 消费关系

| Component | 消费方 | 更新方式 |
|-----------|--------|---------|
| `StageComponent` | RenderWorld | 不做视觉更新，只用于四分支比对 |
| `SpatialComponent` | 所有 VisualNode | 每帧 pull → Godot Position/Rotation/Scale |
| `FacadeComponent` | HeroVisual / EnemyVisual / BuffVisual | 每帧 pull → 模型/动画/特效 |
| `HUDComponent` | HUDView（独立于 SceneTree） | 每帧 pull → Canvas 控件更新 |

HUDComponent 不经过 RenderWorld。HUDView 直接 `mirror.GetComp<HUDComponent>(hero)` 读取，和 3D 场景树完全解耦。

---

## 六、VisualNode 类型映射

```csharp
// RenderWorld 用 FacadeComponent.model 查找创建哪种视觉节点
// 和 Logic 层的 Prefab 体系对称

// Logic: HeroPrefab → 组装 Behavior + BehaviorInfo
// Render: model ID → 实例化对应的 Godot 场景

int model = facade.model;
PackedScene scene = model switch
{
    10001 => heroScene,      // 英雄 A
    10002 => heroSceneB,     // 英雄 B
    20001 => magicSceneA,    // 魔法体 A
    30001 => buffSceneA,     // Buff 视觉效果
    _     => defaultScene,
};
```

---

## 七、完整帧循环

```
┌─────────────────────────────────────────────────────────┐
│                    逻辑帧 (OnStep)                        │
│                                                         │
│  Stage.Step()                                           │
│    → Behavior.OnTick × N （修改 BehaviorInfo）           │
│    → Behavior.EndTick × N                                │
│      → ProjectorSystem.OnEndTick 自检脏 Info             │
│        → ProjectorPacket[]                               │
│                                                         │
│  Pipeline.Process(packets)                               │
│    → Crop.Process (AOI / Freq / Perm / Vis)              │
│    → ObserverPacket[]                                    │
│                                                         │
│  Transport.Send(observerPackets)                         │
│    → LocalTransport: 直接写 Mirror                       │
│    → NetworkTransport: 序列化 → 网络                     │
└─────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                    主线程 (OnLateTick)                    │
│                                                         │
│  Mirror.ApplyPackets()         ← 若多线程，从队列取包     │
│    → 更新所有 Component 实例                              │
│    → touchedactors.Add(actor)                            │
│                                                         │
│  RenderWorld.OnPostUpdate()                              │
│    → PULL mirror.stage.actors   (谁活着)                  │
│    → PULL mirror.touchedactors (本帧谁可见)               │
│    → 四分支比对: create / hide / destroy                  │
│    → 不调 VisualNode 更新                                │
│                                                         │
│  HUDView.OnLateTick()                                    │
│    → mirror.GetComp<HUDComponent>(hero) → 更新 UI         │
└─────────────────────────────────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────────┐
│                  Godot 渲染帧 (_Process)                  │
│                                                         │
│  每个 VisualNode._Process()                              │
│    → mirror.GetComp<SpatialComponent>(actor)            │
│    → mirror.GetComp<FacadeComponent>(actor)             │
│    → 自己拉到数据后更新自己                                │
│                                                         │
│  GPU 渲染                                                │
└─────────────────────────────────────────────────────────┘
```

---

## 八、Transport 替换：单机 → 服务器

同一套 Mirror + RenderWorld + VisualNode，切换 Transport 即可：

```csharp
// 单机
pipeline.transport = new LocalTransport { mirror = mirror };

// 帧同步服务器
pipeline.transport = new NetworkTransport { socket = serverSocket };

// 状态同步服务器（客户端本地无 Stage）
// 直接从网络读 ObserverPacket → Mirror.ApplyPackets
networkClient.OnReceive += (packets) => mirror.ApplyPackets(packets);
```

表现层代码不动。这是整个设计的核心价值。

---

## 九、设计原则总结

| 原则 | 说明 |
|------|------|
| **Mirror = 纯数据** | 不包含逻辑，不直接驱动渲染 |
| **RenderWorld = 生命周期管理** | 只管节点有没有、该不该在场景树上 |
| **VisualNode = 自驱更新** | 自己从 Mirror pull 数据，自己更新自己 |
| **状态同步模型** | 表现层是无状态消费者，权威在 Logic |
| **Transport 可替换** | Local / TCP / UDP / WebSocket 同一接口 |
| **ActorID = EntityID** | 不引入额外映射层 |
| **四分支决策** | create / hide / destroy 由 RenderWorld 做，update 由 VisualNode 自己做 |

---

## 十、实际实现对照

本文描述的是 Mirror/VisualNode **pull 模型**（VisualNode 在 `_Process` 中主动从 Mirror pull 数据）。实际代码实现采用了更简洁的 **push 模型**：

| 维度 | 本文设计 | 实际实现 |
|------|---------|---------|
| 数据中心 | `Mirror` | `Mirror`（一致） |
| 可见性追踪 | `Mirror.touchedactors`（本帧收到数据的 Actor 集合） | 无（Mirror 不追踪可见性） |
| 数据消费 | `VisualNode._Process()` pull | `IComponentApply<T>.ApplyTo()` push |
| 生命周期管理 | `RenderWorld` 四分支决策 | `Mirror.ApplyPackets` 直接写入 Component |
| Component 角色 | 纯数据（一致） | 纯数据 + `IComponentApply` 静态 ApplyTo |
| 映射注册 | `Mirror.Register<TInfo, TComp>()` | 一致 |

实际实现中，Render 层更薄：没有 `RenderWorld`、没有 `VisualNode` 自驱更新、没有对象池缓存（pooled/destroying 状态机）。Component 是被动数据容器，由 Mirror 直接写入。

详见 [ARCHITECTURE.md](ARCHITECTURE.md) §3.6。
