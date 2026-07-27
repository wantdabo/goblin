# GBL 容器 Reset 能力全面审计

> 2026-07-27 | Logic 层手动 `Clear()` + `ObjectCache.Set()` 模式搜集与优化

## 背景

GBL 容器（`GBLList`、`GBLDict`、`GBLHashSet`）均实现了 `IGBL`，`Reset()` 已内置：
- 遍历元素 → `IGBL.Reset()` + `ObjectCache.Set(igbl)`（若元素是 IGBL）
- 清空内部容器 → `ObjectCache.Set(内部容器)` → 重新获取空容器
- 等价于 `Clear() + ObjectCache.Set(this)` 的完整语义

`Gameplay/Logic` 中存在大量冗余的 `Clear() + ObjectCache.Set()` 两步操作，以及因不了解 `GBLDict.Clear()` 内部行为导致的重复回收。

## A 类：`Clear() + ObjectCache.Set(容器)` → `容器.Reset()`

不可变的一步替代两步，更安全（避免忘记 Set）。

| # | 文件 | 行号 | 当前 | 优化 |
|---|------|------|------|------|
| 1 | `Behaviors/Sa/Herald.cs` | 60-61 | `soliderdict.Clear(); ObjectCache.Set(soliderdict);` | `soliderdict.Reset();` |
| 2 | `Behaviors/Sa/Flow.cs` | 188-189 | `indexes.Clear(); ObjectCache.Set(indexes);` | `indexes.Reset();` |
| 3 | `Behaviors/SkillLauncher.cs` | 59-60 | `pipelines.Clear(); ObjectCache.Set(pipelines);` | `pipelines.Reset();` |
| 4 | `Behaviors/Sa/AttributeBucket.cs` | 195-196 | `done.Clear(); ObjectCache.Set(done);` | `done.Reset();` |

## B 类：GBLDict.Clear() 已处理 IGBL 子元素，foreach 多余

`GBLDict<K,V>.Clear()` 当 V 是 IGBL 时：

```
Clear() → foreach value → igbl.Reset() + ObjectCache.Set(igbl)
       → data.Clear() + order.Clear()
```

外部手动 foreach 会导致同一对象进池两次。

### B-1: SilentMercy.OnEndTick（双重池化）

**文件**: `Behaviors/Sa/SilentMercy.cs` 第 92-97 行

```csharp
// 当前
foreach (var kv in info.killrelations)
{
    kv.Value.Clear();           // GBLList<ulong>.Clear() → 清空内部 List
    ObjectCache.Set(kv.Value);  // 第一次进池
}
info.killrelations.Clear();     // GBLDict.Clear() → 遍历值 → Reset()+Set → 第二次进池
```

```csharp
// 优化后
info.killrelations.Clear();     // GBLDict.Clear() 已处理所有子 GBLList 的 Reset+Set
```

### B-2: Eventor.OnDisassemble（双重池化）

**文件**: `Behaviors/Sa/Eventor.cs` 第 32-38 行

```csharp
// 当前
foreach (var kv in eventdict)
{
    kv.Value.Clear();
    ObjectCache.Set(kv.Value);      // 值被池化
}
eventdict.Clear();
ObjectCache.Set(eventdict);         // Clear() 再次池化值
```

```csharp
// 优化后（合并 A+1 和 B-2）
eventdict.Reset();                  // 一行替代整段，无双重池化
```

## C 类：Herald 双向池化 Bug

**文件**: `Behaviors/Sa/Herald.cs` 第 55-61 行

```csharp
// 当前（Bug）
foreach (var solider in soliderdict.Values)
{
    solider.Unload();
    ObjectCache.Set(solider);       // ← Solider 第一次进池
}
soliderdict.Clear();                // Clear() 遍历值 → Reset()+Set → 第二次进池
ObjectCache.Set(soliderdict);       // soliderdict 进池
```

`soliderdict.Clear()` 内部对每个 Solider（IGBL）做 `Reset() + Set()`，导致 Solider 进池两次。

```csharp
// 优化后
foreach (var solider in soliderdict.Values) solider.Unload();
soliderdict.Reset();                // Reset() 内部处理 Solider + soliderdict 双回收，一次搞定
```

> `Reset()` 内部会 Reset+Set 所有值（含已 Unload 的 Solider），**每个 Solider 仅进池一次**。

## D 类：Raw `List<T>` 手动 Clear+Set（低优先级）

这些是原生 `List<T>`（非 GBL），经由 `ObjectCache` capacity pool 路径回收。可选择性统一为 `GBLList<T>.Reset()` 风格。

| # | 文件 | 行号 | 当前类型 |
|---|------|------|----------|
| 5 | `Behaviors/Sa/Buff.cs` | 64-65 | `List<uint>` — `pipelines` |
| 6 | `Behaviors/Sa/Buff.cs` | 176-177 | `List<ulong>` — `buffs` |
| 7 | `Behaviors/Facade.cs` | 193-194 | `List<uint>` — `effectKeys` |

## D+: 架构正确无需改动的场景

以下模式虽含 `Clear()` 但语义正确，不需优化：

| 场景 | 原因 |
|------|------|
| `Detection.OnTick` 每帧 `raycasts.Clear()` | 持久字段，跨帧复用，不归还池 |
| `Stage.Recycle` 中 `rmvactorset.Clear()` | 持久字段，Reset 会重新 Ensure 内部集合（浪费） |
| `Gamepad.OnEndTick` 手动 `Reset()+Set` 元素 + `keys.Clear()` | 元素类型非 IGBL，手动处理正确 |

## 汇总

| 优先级 | 数量 | 类型 |
|--------|------|------|
| **P0 Bug** | 3 处 | 双重池化（Herald、Eventor、SilentMercy） |
| **P1 简化** | 4 处 | Clear+Set → Reset |
| **P2 风格** | 3 处 | Raw List → GBLList |
| **合计消除行数** | ~18 行 | |
