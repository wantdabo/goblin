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
- Projection 层：纯 C# 数据消费，Canvas + Shadow 体系
- Render 层：Godot 依赖 (VisualNode 等)
- SourceGenerators：`[Projector]` 扫描生成 IProjectable + Shadow + ApplyTo
