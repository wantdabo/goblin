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
