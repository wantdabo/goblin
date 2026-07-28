# Goblin 文档索引

> 更新日期：2026-07-28

---

## 架构（当前实现）

| 文档 | 内容 |
|------|------|
| [ARCHITECTURE.md](ARCHITECTURE.md) | 完整架构文档（基于源码） |
| [CODING_STYLE.md](CODING_STYLE.md) | 编码规范 |

## 设计文档

| 文档 | 内容 |
|------|------|
| [RENDER_LAYER_DESIGN.md](RENDER_LAYER_DESIGN.md) | 渲染层设计方案（proposal，非当前实现） |
| [ANIMATION_PROPOSAL.md](ANIMATION_PROPOSAL.md) | 动画槽位优先级系统（Phase 1-3 已落地） |
| [Projection/CORE.md](Projection/CORE.md) | Projection 设计哲学：Simulation → Projection → Presentation |
| [Projection/PROPERTY_SYNC_DESIGN.md](Projection/PROPERTY_SYNC_DESIGN.md) | 属性同步完整设计（部分实现，实际使用 Mirror 而非 Entity/RenderWorld） |
| [Projection/SNAPSHOT_RESTORE_DESIGN.md](Projection/SNAPSHOT_RESTORE_DESIGN.md) | 快照/恢复机制与投影脏标记设计 |

## 实施与审计

| 文档 | 内容 |
|------|------|
| [Projection/IMPLEMENTATION_PLAN.md](Projection/IMPLEMENTATION_PLAN.md) | Property Sync 实施计划与进度追踪 |
| [Projection/GAMEPLAY_AUDIT.md](Projection/GAMEPLAY_AUDIT.md) | Gameplay 模块全面审计（2026-07-27） |
| [Projection/PROJECTION_AUDIT.md](Projection/PROJECTION_AUDIT.md) | 投影系统审计（2026-07-27） |
| [Projection/BEHAVIORINFO_LIFECYCLE_REPORT.md](Projection/BEHAVIORINFO_LIFECYCLE_REPORT.md) | BehaviorInfo 生命周期自动化分析 |

## 其他

| 文档 | 内容 |
|------|------|
| [../README.md](../README.md) | 项目入口 |
| [../TODO.md](../TODO.md) | Bug 修复与功能规划 |
| [../Skills/goblin-debug.md](../Skills/goblin-debug.md) | Debug HTTP API 参考 |
