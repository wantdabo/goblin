# Goblin 编码规范

> 2026-07-19 | 基于实际源码反推

---

## 1. 属性全小写

所有属性 — 公开、私有、protected — **一律小写**。不 `_` 前缀，不 PascalCase。

```csharp
// ✅
public bool destroyed { get; private set; }
public Engine engine { get; set; }
private List<Comp> comps { get; set; }
protected Stage stage { get; private set; }

// ❌
public bool Destroyed { get; set; }
private Engine _engine;
```

常量使用 SCREAMING_SNAKE_CASE，定义类以 `_DEFINE` 后缀：

```csharp
// ✅
STATE_DEFINE.NONE
ATTRIBUTE_DEFINE.MOVESPEED
FLOW_DEFINE.ET_CASTER
ACTOR_DEFINE.HERO

// ❌
StateDefine.None
AttributeDefine.moveSpeed
```

## 2. 条件判断：常量在前

`null` 和 `false` 永远放左边，不用 `!` 取反。

```csharp
// ✅
if (null == comps) return;
if (false == stage.SeekBehavior(actor, out StateMachine machine)) return;

// ❌
if (comps == null) return;
if (!dict.TryGetValue(...))
```

## 3. 动词缩写命名

短名优先：

| 缩写 | 全称 |
|------|------|
| `Rmv` | Remove |
| `Gen` | Generate |
| `Seek` | Find/Lookup |
| `Tell` | Dispatch/Send |

## 4. 方法命名

| 类别 | 命名 | 示例 |
|------|------|------|
| 公共方法 | PascalCase | `Create()`, `Destroy()`, `Tick()` |
| 虚方法钩子 | `On` 前缀 | `OnCreate()`, `OnTick()`, `OnEnter()` |
| 查询方法 | 动词短名 | `GetComp<T>()`, `SeekBehavior()` |

## 5. SeekXxx 模式

一律返回 `bool`，数据走 `out` 参数：

```csharp
if (false == stage.SeekBehaviorInfo(actor, out SpatialInfo spatial))
if (false == stage.SeekBehavior(actor, out StateMachine machine))
```

## 6. Behavior / BehaviorInfo 模式

```csharp
// Info: 纯数据，OnReady/OnReset/OnClone 三生命周期
public class MovementInfo : BehaviorInfo
{
    public bool turnmotion { get; set; }
}

// Behavior: 逻辑，绑定泛型 Info，OnTick/OnEndTick
public class Movement : Behavior<MovementInfo>
{
    // info 访问绑定数据
    // stage.SeekXxx() 访问其他 Behavior/Info
}
```

## 7. Executor 签名

```csharp
protected override void OnEnter((uint pipelineid, uint index) identity, 
    DamageData data, FlowInfo flowinfo, ulong target)
```

## 8. 生命周期

```
Comp:     OnCreate → OnDestroy
Behavior: OnAssemble → OnTick × N → OnEndTick → OnDisassemble
Info:     OnReady → OnReset → OnClone
Executor: OnEnter → OnExecute → OnExit
```

## 9. 简短卫语句单行

`if + return/break/continue` 体量小时合并一行：

```csharp
// ✅
if (null == comps) return;
if (false == condition) break;
if (false == stage.SeekBehaviorInfo(actor, out MagicInfo magic)) return 0;

// ❌
if (null == comps)
    return;
```

## 10. 缩进与编码

- 4 空格缩进
- CRLF 行尾
- UTF-8 BOM
- 文件级命名空间（`;` 结尾）

## 11. 注释

- `/// <summary>` XML 文档，中文
- 行内注释 `//` 中文
- 不写英文注释
- 注释独占一行，不跟在代码后面（禁止行尾注释）

## 12. 严禁事项

| ❌ |
|---|
| 属性用 PascalCase 或 `_` 前缀 |
| `if (x == null)` — 必须 `if (null == x)` |
| `if (!condition)` — 必须 `if (false == condition)` |
| 用裸 field 代替 `{ get; set; }` 属性 |
| 类名加前缀/后缀 |
| 英文注释 |
| emoji |
| 行尾注释 — 注释独占一行 |

## 13. 架构速查

- Logic 层零 Godot 依赖，用自研定点数（FPVector3/FPQuaternion/FP），Render 层依赖 Godot API
- Projection 投影同步：Logic 层 `[Projector]` 字段变更 → `ProjectorSystem` 脏标记出包 → `Crop` 规则链裁剪 → `Transport` → `Mirror.ApplyPackets` → `Component` 更新
- Render 层：`Mirror`（数据镜像）+ `Component` 子类（`SpatialComponent`/`FacadeComponent`/`HUDComponent`），零反射 `ApplyTo` 消费
- SA（System Actor，actor=ulong.MaxValue）挂全局 Behavior
- Source Generator：`partial class + IGBL` 自动生成 `Reset()`/`Clone()`；`[Projector]` 类级注解生成 backing field + `IProjectable` 实现
- 详细架构见 [ARCHITECTURE.md](ARCHITECTURE.md)
