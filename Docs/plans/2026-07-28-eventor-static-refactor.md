# Eventor 静态重构 Implementation Plan

> **For Claude:** REQUIRED SUB-SKILL: Use superpowers:executing-plans to implement this plan task-by-task.

**Goal:** 将 Eventor 从 Behavior 实例回调模式重构为纯静态注册派发类，消除 EventorInfo 序列化状态，简化快照/恢复。

**Architecture:** Eventor 退化为 `static class`，Behavior 在静态 ctor 中通过 `Eventor.Listen<T>(handler)` 注册静态处理器。派发时 `Eventor.Tell(stage, e)` 按 handler 所在类型全名（`StringComparer.Ordinal`）升序遍历，纯编译期常量确定，跨平台一致。

**Tech Stack:** C# 静态类、`Dictionary<Type, List<...>>` 非池化永久存储

---

## 设计决策

### 排序：`declaringTypeFullName` 升序

- handler 所在类型的全名（如 `"Goblin.Gameplay.Logic.Behaviors.Sa.AttributeBucket"`）是编译期嵌入 IL 的字符串常量
- `StringComparer.Ordinal`（Unicode 码位逐字节）比较，跨平台/OS/Locale 100% 一致
- 注册零参数，时序完全由类型名决定，无需人工介入
- 若未来需要调序，可随后加可选 `order` 参数，不影响现有 API

### 生命周期

- 注册：静态 ctor 中 `Eventor.Listen<T>(handler)`
- 派发：`Eventor.Tell(stage, e)` — 显式传入 Stage
- **去掉 behavior.active 检查**：订阅者是全局 SA 行为，常驻

### 事件结构体不变

- 保持纯数据，不携带 stage
- 接口 `IEvent` 保留在 Eventor.cs（标记接口）

---

## Task 1: 重写 Eventor.cs 为静态类

**Files:**
- Modify: `godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/Eventor.cs`

**Step 1: 替换 Eventor.cs 全部内容**

```csharp
using System;
using System.Collections.Generic;
using Goblin.Gameplay.Logic.Core;

namespace Goblin.Gameplay.Logic.Behaviors.Sa;

/// <summary>
/// 事件标记接口
/// </summary>
public interface IEvent { }

/// <summary>
/// 事件订阅派发者（静态单例）
/// 订阅方在静态 ctor 中调用 Listen 注册
/// 派发方调用 Tell 通知所有订阅方
/// 按 handler 所在类型全名（Ordinal）确定时序，跨平台一致
/// </summary>
public static class Eventor
{
    /// <summary>
    /// 按类型名排序用的比较器
    /// </summary>
    private static readonly EntryComparer comparer = new();

    /// <summary>
    /// 事件字典 [事件类型 → 处理器列表]
    /// </summary>
    private static readonly Dictionary<Type, List<(string key, Delegate action)>> eventdict = new();

    /// <summary>
    /// 注册事件监听
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="handler">静态处理函数</param>
    public static void Listen<T>(Action<Stage, T> handler) where T : IEvent
    {
        var type = typeof(T);
        if (false == eventdict.TryGetValue(type, out var list))
        {
            list = new List<(string, Delegate)>();
            eventdict.Add(type, list);
        }

        string key = handler.Method.DeclaringType.FullName;
        list.Add((key, handler));
        list.Sort(comparer);
    }

    /// <summary>
    /// 派发事件
    /// </summary>
    /// <typeparam name="T">事件类型</typeparam>
    /// <param name="stage">逻辑阶段</param>
    /// <param name="e">事件参数</param>
    public static void Tell<T>(Stage stage, T e) where T : IEvent
    {
        if (null == stage) return;
        if (false == eventdict.TryGetValue(typeof(T), out var list)) return;
        for (int i = 0; i < list.Count; i++)
        {
            var entry = list[i];
            (entry.action as Action<Stage, T>).Invoke(stage, e);
        }
    }

    /// <summary>
    /// 条目排序比较器：按类型全名字母序
    /// </summary>
    private sealed class EntryComparer : IComparer<(string key, Delegate action)>
    {
        public int Compare((string key, Delegate action) x, (string key, Delegate action) y)
        {
            return string.CompareOrdinal(x.key, y.key);
        }
    }
}
```

---

## Task 2: 删除 EventorInfo.cs

**Files:**
- Delete: `godot/Scripts/Goblin/Gameplay/Logic/BehaviorInfos/Sa/EventorInfo.cs`

**理由:** 静态类无实例状态，increment/indexes 全部废弃。

---

## Task 3: 修改 Stage.cs — 移除 Eventor 的 Behavior 属性

**Files:**
- Modify: `godot/Scripts/Goblin/Gameplay/Logic/Core/Stage.cs`

**Step 1: 移除 `using SaEventor = ...` 别名（line 8），改为直接引用静态类**

```csharp
// 删除
using SaEventor = Goblin.Gameplay.Logic.Behaviors.Sa.Eventor;
```

改为普通 using：

```csharp
using Goblin.Gameplay.Logic.Behaviors.Sa;
```

（如果还没有 BehaviorInfos.Sa 的 using 则需要加，但实际上已有 line 5 `using Goblin.Gameplay.Logic.BehaviorInfos.Sa;` 和 line 7 `using Goblin.Gameplay.Logic.Behaviors.Sa;`）

**Step 2: 删除 `eventor` 属性（line 109）**

```csharp
// 删除
public SaEventor eventor => GetBehavior<SaEventor>(sa, true);
```

**Step 3: 删除 `AddBehavior<SaEventor>(sa)`（line 217）**

从 `Behaviors()` 方法中删除该行。

**Step 4: 修改 Tell 调用 — 改为静态调用**

`eventor.Tell(new ActorRmvEvent { actor = actor })` → `Eventor.Tell(this, new ActorRmvEvent { actor = actor })`

`eventor.Tell(new ActorBornEvent { actor = actor })` → `Eventor.Tell(this, new ActorBornEvent { actor = actor })`

---

## Task 4: 修改 AttributeBucket.cs — 静态注册

**Files:**
- Modify: `godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/AttributeBucket.cs`

**Step 1: 添加静态构造函数注册事件**

```csharp
static AttributeBucket()
{
    Eventor.Listen<ActorRmvEvent>(OnActorRmv);
}
```

**Step 2: 删除 OnAssemble/OnDisassemble 中的 Listen/UnListen**

删除 line 28-32 和 line 34-38。

**Step 3: OnActorRmv 改为静态方法，接收 Stage**

```csharp
private static void OnActorRmv(Stage stage, ActorRmvEvent e)
{
    // 需要通过 stage 获取 AttributeBucket 实例
    var bucket = stage.attrb;
    if (false == bucket.info.attributes.ContainsKey(e.actor)) return;
    if (bucket.info.pendings.Contains(e.actor)) return;
    bucket.info.pendings.Add(e.actor);
}
```

**Step 4: 确认 `stage.attrb` 属性存在**

需确认 Stage 上有 `attrb` 属性。如果没有，需要添加：
```csharp
public AttributeBucket attrb => GetBehavior<AttributeBucket>(sa, true);
```

---

## Task 5: 修改 Seat.cs — 静态注册

**Files:**
- Modify: `godot/Scripts/Goblin/Gameplay/Logic/Behaviors/Sa/Seat.cs`

**Step 1: 添加静态构造函数**

```csharp
static Seat()
{
    Eventor.Listen<ActorRmvEvent>(OnActorRmv);
}
```

**Step 2: 删除 OnAssemble/OnDisassemble 中的 Listen/UnListen**

删除 line 12-16 和 line 18-22。

**Step 3: OnActorRmv 改为静态，接收 Stage**

```csharp
private static void OnActorRmv(Stage stage, ActorRmvEvent e)
{
    var seat = stage.seat;
    if (false == seat.info.asdict.TryGetValue(e.actor, out var seatid)) return;
    seat.info.asdict.Remove(e.actor);
    seat.info.sadict.Remove(seatid);
}
```

---

## Task 6: 编译验证

**Step 1: 编译项目**

```powershell
dotnet build godot/Scripts/Goblin/Goblin.csproj
```

**Step 2: 修复编译错误**

预期可能的问题：
- `stage.attrb` 属性不存在（Task 4 中需要添加）
- using 调整后的命名空间冲突
- 其他 Behavior 中对 `eventor` 属性的引用

**Step 3: 全量搜索残留引用**

```powershell
rg "eventor\." godot/Scripts/ --type cs
rg "SaEventor" godot/Scripts/ --type cs
rg "EventorInfo" godot/Scripts/ --type cs
```

确认全部清理完毕。

---

## 变更清单

| 文件 | 操作 |
|---|---|
| `Eventor.cs` | 重写为 static class |
| `EventorInfo.cs` | **删除** |
| `Stage.cs` | 移除 eventor 属性、AddBehavior、改为静态 Tell |
| `AttributeBucket.cs` | 静态 ctor 注册、OnActorRmv 改为 static |
| `Seat.cs` | 静态 ctor 注册、OnActorRmv 改为 static |
