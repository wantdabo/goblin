# TODO

## Bug：重复进入游戏时 GenActor 空引用

- **堆栈**：`Stage.cs:652 @ GenActor()` → `Spawn()` → `Building()` → `Initialize()`
- **复现**：开始游戏 → 退出 → 开始，第二次开始时崩溃
- **错误**：`System.NullReferenceException: Object reference not set to an instance of an object`
- **猜测**：Stage 退出后某个缓存/Behavior 字典未正确 Reset，第二次 Initialize 时状态残留导致空引用

