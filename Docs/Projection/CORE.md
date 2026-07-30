# Goblin Runtime - World Projection Vision

> 状态：`Design`
>
> **One Simulation, Multiple Projections.**
>
> 本文件描述 Projection 体系的设计哲学；当前实现详见 [ARCHITECTURE.md](../ARCHITECTURE.md) §3.6。

---

# 核心思想

Goblin 不应该围绕：

- ECS
- 网络同步
- 渲染

去设计。

Goblin 应该围绕：

> **Simulation（世界）如何投影给不同 Observer（观察者）**。

整个 Runtime 可以抽象成三层：

```
                Simulation
            (World Truth)

                   │
                   ▼

           World Projector

                   │
                   ▼

            Presentation
```

整个 Runtime 只有一个真实世界（Simulation）。

所有客户端、AI、Replay、GM、Render，都只是这个世界的不同投影（Projection）。

---

# Runtime Architecture

```
                 Simulation
               (Logic ECS)

                     │
                     │
             World ChangeSet
                     │
                     ▼

            World Projector
         (ProjectorSystem 脏标记)

                     │
      ┌──────────────┼──────────────┐
      │              │              │
      ▼              ▼              ▼

 Player Crop     Replay Crop      AI Crop
(Player 规则链)  (Replay 规则链)  (AI 规则链)

      │              │              │
      ▼              ▼              ▼
  Transport      Transport       Transport

      │              │              │
      └──────────────┼──────────────┘
                     │
                     ▼

              Presentation
            (Entity/Component)

                     │

                     ▼

                 Renderer
```

---

# 第一层：Simulation

Simulation 是整个 Runtime 唯一真实世界。

Simulation 只负责：

- 世界规则
- ECS
- Gameplay
- 状态演化

Simulation 永远不知道：

- 网络
- AOI
- 战争迷雾
- 客户端
- Render
- Animation
- Effect

Simulation 每 Tick：

输入：

```
Input
Command
Event
```

输出：

```
World State
```

---

# Simulation 对外输出

Simulation 不直接产生：

- Animation
- Effect
- Network Packet

Simulation 每 Tick：

只产生：

```
World ChangeSet
```

例如：

```
Created Entity

Removed Entity

Added Component

Removed Component

Modified Component
```

Simulation 到这里结束。

---

# 第二层：World Projector

Goblin 最重要的一层。

World Projector 的职责：

> **将同一个 Simulation World 投影给不同的 Observer。**

例如：

同一个世界：

```
10000 Entity
```

Player：

```
100 Entity
```

GM：

```
10000 Entity
```

Replay：

```
录像世界
```

AI：

```
AI 感知世界
```

Simulation 永远保持一致。

不同的是：

```
Projection
```

---

# Projection 的职责

Projection 不负责：

- Gameplay
- Combat
- Skill
- AI

Projection 只负责：

> **世界如何被观察。**

包括**空间**（看到什么）和**时间**（怎么平滑）：

```
AOI (距离 / 分区)

Permission (同队 / 敌方 / 中立)

Visibility (战争迷雾 / 草丛 / 隐身)

Frequency (位置 20Hz, 背包 1Hz)

Interpolation           ← 帧间平滑

Prediction              ← 客户端推测
```

---

# Projection Pipeline

```
World ChangeSet

↓

AOI

↓

Permission

↓

Visibility

↓

Frequency

↓

Presentation Mapping

↓

Projection Result
(fieldmask + values + frame)
```

规则链串联执行。Crop 负责组织规则顺序，各规则独立裁决。最后 mask == 0 则丢弃。

---

# AOI

负责：

```
谁在观察范围内。
```

例如：

- 距离
- 分区（MMO 地图格）

---

# Visibility

负责：

```
是否可见。
```

例如：

- 战争迷雾
- 草丛
- 隐身
- 视野

Simulation：

永远知道。

Projection：

决定是否告诉 Observer。

---

# Permission

负责：

```
能看到什么数据。
```

例如：

```
同队 → HP / Buff 全部可见
敌方 → 只能看位置，HP 不可见
```

不同 Observer 挂不同权限配置。

---

# Frequency

负责：

```
多久同步一次。
```

例如：

```
Transform

20Hz

Inventory

1Hz
```

---

# Presentation Mapping

负责：

Simulation

↓

Presentation State

例如：

```
Buff

↓

Aura Effect
```

```
Skill State

↓

Animation
```

这里不是 Render。

这里只负责：

**Presentation State。**

---

# Property Sync

Logic 层 BehaviorInfo 通过 `[Project]` 注解声明哪些字段参与同步。

ProjectorSystem 按脏标记直接出包。Crop 按 Observer 规则链（AOI/权限/视野/频率）裁剪。Transport 将修剪后的属性值推送到 Render 层。

不是消息，不是 RPC，不是 API。是**属性的声明式同步**——Logic 改了，Render 收到。

详细的工程方案见 `PROPERTY_SYNC_DESIGN.md`。任务拆解见 `IMPLEMENTATION_PLAN.md`。

---

# 第三层：Presentation

Presentation：

永远不知道：

当前运行的是：

- 单机
- 帧同步
- 状态同步
- Replay
- AI

Presentation：

只消费：

```
fieldmask + values + frame + latency
```

即可。

以 Entity + Component 组织。每个 Logic Actor 对应一个 Render Entity，Entity 上挂 Component。Component 只做两件事：**收属性** + **表达**。

---

# Frame Sync

```
Input

↓

Simulation

↓

ChangeSet

↓

Projection

↓

Property Sync

↓

Presentation
```

同步的是：

```
Input
```

---

# State Sync

```
Server

↓

Simulation

↓

ChangeSet

↓

Projection

↓

Network

↓

Client

↓

Property Sync

↓

Presentation
```

同步的是：

```
State
```

Presentation 完全一致。

---

# Replay

```
Replay Stream

↓

Simulation

↓

Projection

↓

Property Sync

↓

Presentation
```

无需特殊逻辑。

---

# AI

```
Simulation

↓

AI Projection

↓

AI
```

AI 不一定拥有完整世界。

例如：

```
普通 AI

↓

视野 Projection
```

作弊 AI：

```
God Projection
```

即可。

---

# Observer

Projection 的核心不是 Client。

而是：

```
Observer
```

任何观察者：

```
Player

Replay

GM

AI

Spectator
```

都只是：

```
Observer
```

Projection：

本质就是：

```
Simulation World

↓

Observer

↓

Projection World
```

---

# World Projector Interface

```csharp
public interface IProjectionRule
{
    /// <summary>
    /// 裁剪一个 (actor, fieldmask, values)
    /// 返回修剪后的 fieldmask。0 表示整条丢弃
    /// </summary>
    ulong Filter(ProjectorPacket packet, Observer observer, ulong currentMask);
}
```

不同 Observer 挂不同规则链（AOI/权限/视野/频率），链上每节独立裁决，最后 mask == 0 就丢弃。

---

# Design Philosophy

Simulation：

> 世界真实是什么。

Projection：

> 不同 Observer 应该知道什么。

Presentation：

> 如何表现这个世界。

---

# Goblin Runtime

```
Goblin Runtime

├── Simulation
│      世界真实状态
│      (BehaviorInfo + [Project] 注解)
│
├── World Projector
│      世界投影
│      (ProjectorSystem 脏标记 + Crop 规则链裁剪)
│
└── Presentation
       世界表现
       (Entity + Component)
```

---

# Vision

**One Simulation.**

**Multiple Projections.**

**One Presentation.**

**Business Logic Exists Only Once.**