# Goblin 项目长期记忆

## 缩进铁律（极其重要）

- **Goblin 项目一律使用 4 空格缩进**，严禁 Tab 字符
- `replace_in_file` 操作必须确保 `old_str` 和 `new_str` 中全部是 4 空格缩进
- **每次编辑后自检**：被编辑文件中不应出现 `\t` 字符
- 历史上反复出现 Tab 污染工作树的问题（HEAD `a587e8cf` 专门做过 "Tab 缩进清理"），每次污染根因均为 `replace_in_file` 的 `new_str` 含 Tab

## 编码规范核心

- 属性全小写、常量 SCREAMING_SNAKE_CASE
- `if (null == x)` 非 `if (x == null)`
- 不用 `!`：`if (false == condition)`
- 注释独占一行，禁止行尾注释，中文注释
- 文件级命名空间（末尾 `;`）
- 卫语句单行：`if (null == comps) return;`
- 完整规范见 `Docs/CODING_STYLE.md`

## 架构分层

- Logic 层：零 Godot 依赖，定点数 `FPVector3/FPQuaternion/FP`
- **全部 Behavior 已提升至 Sa 级**（2026-07），每个 Behavior 类型全局仅一个实例，per-actor 数据由 BehaviorInfo 承载
- Behavior 公开方法首参为 `ulong actor`，跨 Behavior 调用使用 `stage.xxx.Method(actor, ...)` 快捷属性
- Stage 快捷属性：`stage.movement/statemachine/gamepad/tag/facade/hud/skilllauncher`
- Prefix/Prefab 使用 `AddBehaviorInfo<T>(actor)` 替代 `AddBehavior<T>(actor)`
- Behavior 文件放 `Behaviors/` 根目录（不再区分子目录）；BehaviorInfo 保持 `BehaviorInfos/` 和 `BehaviorInfos/Sa/` 分目录
- Projection 层：纯 C# 数据消费，Canvas + Shadow 体系
- Render 层：Godot 依赖 (VisualNode 等)
- SourceGenerators：`[Projector]` 扫描生成 IProjectable + Shadow + ApplyTo
