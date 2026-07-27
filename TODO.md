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
