# Goblin 文档索引

> 更新日期：2026-07-31
>
> 文档状态：`Current` 表示源码已验证的当前实现；`Design` 表示设计方案；`Plan` 表示待执行或迁移中的计划；`Audit` 表示阶段性审查记录；`Historical` 表示历史资料。设计、计划和审计文档不得替代当前架构说明。

---

## 推荐阅读顺序

1. [../README.md](../README.md)：安装、构建入口和项目结构
2. [ARCHITECTURE.md](ARCHITECTURE.md)：当前实现的唯一架构权威
3. [CODING_STYLE.md](CODING_STYLE.md)：代码规范和层间边界
4. [Projection/CORE.md](Projection/CORE.md)：Projection 的设计目标
5. [../TODO.md](../TODO.md)：未完成事项和已知风险

## Current：当前实现

| 文档 | 内容 |
|------|------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | Logic、Projection、Render 当前架构和数据流 |
| [CODING_STYLE.md](CODING_STYLE.md) | 命名、对象池、生命周期和格式规范 |
| [ANIMATION_PROPOSAL.md](ANIMATION_PROPOSAL.md) | 动画槽位优先级系统；已落地部分以源码为准 |

## Design：设计方案

| 文档 | 内容 |
|------|------|
| [RENDER_LAYER_DESIGN.md](RENDER_LAYER_DESIGN.md) | Render 层设计草案，部分术语已过时 |
| [Projection/CORE.md](Projection/CORE.md) | Simulation → Projection → Presentation 设计哲学 |
| [Projection/PROPERTY_SYNC_DESIGN.md](Projection/PROPERTY_SYNC_DESIGN.md) | 属性同步设计；Entity/Component 是历史方案名 |
| [Projection/SNAPSHOT_RESTORE_DESIGN.md](Projection/SNAPSHOT_RESTORE_DESIGN.md) | 快照恢复与投影脏标记设计 |

## Plan：实施计划

| 文档 | 内容 |
|------|------|
| [Projection/IMPLEMENTATION_PLAN.md](Projection/IMPLEMENTATION_PLAN.md) | Property Sync 迁移计划；完成度以当前源码为准 |

## Audit：审计与分析

| 文档 | 内容 |
|------|------|
| [Projection/GAMEPLAY_AUDIT.md](Projection/GAMEPLAY_AUDIT.md) | 2026-07-27 Gameplay 阶段性审计 |
| [Projection/PROJECTION_AUDIT.md](Projection/PROJECTION_AUDIT.md) | 2026-07-27 Projection 阶段性审计 |
| [Projection/BEHAVIORINFO_LIFECYCLE_REPORT.md](Projection/BEHAVIORINFO_LIFECYCLE_REPORT.md) | 生命周期自动化分析；数字为历史基线 |

## 其他入口

| 文档 | 内容 |
|------|------|
| [../TODO.md](../TODO.md) | Bug 修复记录和功能规划 |
| [../Skills/goblin-debug.md](../Skills/goblin-debug.md) | Debug HTTP API 参考 |
