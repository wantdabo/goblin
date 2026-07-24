# _Archive

Property Sync 体系重构前的旧代码归档。不要修改此目录的文件。

## Render/

旧 RIL 体系的渲染层（~31 个 .cs 文件）。
迁移路径详见 `Docs/DualMode/IMPLEMENTATION_PLAN.md` Phase 4-5。

| 分类 | 数量 | 说明 |
|------|------|------|
| KEEP | 3 | AnimationConfig, EffectController, VectorExtension |
| EXTRACT | 12 | 有可复用代码，需剥 RIL 外壳 |
| DELETE | 16 | 纯 RIL 胶水 |

## Director/

旧 RIL 桥接层（GameplayDirector + LocalDirector）。
迁移路径详见 `Docs/DualMode/IMPLEMENTATION_PLAN.md` Phase 3-4。
重构时会重新设计 ProjectorSystem → RenderWorld 的数据流。

---

所有文件通过 `git mv` 移入，历史可追溯。
