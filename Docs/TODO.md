# Goblin 待办清单

> 2026-07-25 | 更新于 Property Sync 体系设计落地 + Lifecycle 自动化方案

---

## 一、Bug / 安全修复

| 优先级 | 项目 | 说明 | 文件 |
|--------|------|------|------|
| ~~**P0**~~ ✅ | ~~`InsideNotExeToExecute` 无递归上限~~ | 常量 `MAX_INSIDE_NOTEXE_DEPTH` 移入 `FLOW_DEFINE` | `Flow.cs`, `FLOW_DEFINE.cs` |
| ~~**P0**~~ ✅ | ~~释放技能后单位卡死在 CASTING~~ | `StageCache.Valid()` 只查了 `rmvactorset`（Recycle 后清空），加 `behaviordict.ContainsKey()` 做存在性验证。根因：Magic 自动清理为新增逻辑，暴露了 `Valid()` 的设计缺陷 | `Stage.cs` |
| ~~**P0**~~ ✅ | ~~INSTR_DEFINE 编号偏移致序列化反序列化错乱~~ | 删除 MAGIC_MOTION（4）后编号整体左移，序列化 bytes 仍存旧编号（5-16），反序列化映射错误。还原原始编号，4 号位空缺 | `INSTR_DEFINE.cs` |
| ~~**P0**~~ ✅ | ~~BeHit/HitLag/Damage 永远无效（ET 搜索拿到空 targets）~~ | 三者 `et=ET_FLOW_HIT/ET_HIT_VICTIM` 依赖 `flowcollision.targets`，作为帧 instruct 在 Collision.OnExecute spark→Clear 后才执行。改为 spark instruct（TOKEN_ON_HIT），spark 内同步执行 | `S10020.cs`, `SPARK_INSTR_DEFINE.cs` |

## 二、性能优化

| 项目 | 说明 | 文件 |
|------|------|------|
| ~~Spark 按 token 建索引~~ ✅ | `sparkindex` + `indexedpipelines` 已完成 | `Flow.cs` |
| ~~RunPipeline 跳过已过期指令~~ ✅ | `FlowInfo.completedindex`（`Dictionary<uint,uint>`，pipelineid→最后完成 index） | `FlowInfo.cs`, `Flow.cs` |

## 三、功能缺口

| 项目 | 说明 |
|------|------|
| ~~音效支持~~ ✅ | Sound 模块已实现（handle-based SFX API + SoundAgent + RIL 事件管线） |
| ~~抗性计算~~ ✅ | 暴击 / 闪避 / 护甲 / 魔抗 + 伤害类型分支 | `AttributeBucket.cs` |
| ~~受击动画独立状态机~~ ✅ | Phase 1+2 已落地：AnimationSlot 优先级 + 多层 RIL + StateMachine 限时状态 + BeHitExecutor HITSTUN。`ChangeStateExecutor` hack 已删。参考 `Docs/ANIMATION_PROPOSAL.md` | `Docs/ANIMATION_PROPOSAL.md` |
| 逐层动画 elapsed + AnimationTree | ✅ Phase 2.5 已落地。`AnimationSlot.elapsed` 逐槽位递进、`LayerAnimEntry.elapsed` RIL 逐层传输、AnimationAgent 接入 AnimationTree 多层播放（回退兼容 AnimationPlayer） | `Facade.cs`, `RIL_FACADE_ANIMATION.cs`, `AnimationAgent.cs` |
| ~~槽位 key 复合化（多命名动画共存）~~ ✅ | Phase 3 已落地。slot key byte→ushort 复合键 `(slotType<<8 \| layer)` + `RmvSlotsByType` 批量移除 + ANIM_DEFINE 常量改名 `SLOT_TYPE_*` | `Facade.cs`, `ANIM_DEFINE.cs` |
| ~~动画事件帧系统~~ 🗑️ | Phase 4 已砍。Pipeline 时序天然等价于动画帧驱动，取消窗/命中帧由 Pipeline 管理 | |
| ~~确定性混合参数~~ 🗑️ | Phase 5 已砍。Blend weight 分歧不影响游戏态，animstate 覆盖"播哪个状态"的确定性 | |
| Godot Pipeline 可视化编辑器 | Scripting / Timeline / GraphNode 三种编辑方式并存，统一底层 Pipeline 数据结构 |

## 四、架构重构

| 项目 | 说明 |
|------|------|
| ~~Skill 转为 Actor~~ ✅ | 已通过 `Magic` Actor 体系完成 |
| ~~穿透式目标查找规则~~ ✅ | 统一用 `ET_CASTER` + 规则链动态解析 |
| ~~CheckCondition 移入 ExecuteInstruct~~ ✅ | 已统一到 `ExecuteInstruct` 内部 |
| ~~Info 转 RIL 自动化~~ | 被 Property Sync 体系替代（删 RIL，标 Project） |
| ~~Clone 自动化~~ | 被 `[Lifecycle]` + Source Generator 替代 |
| ~~RIL 合并~~ | 被 Property Sync 体系替代（删 RIL） |
| ~~主观 RIL 传输~~ | 被 ProjectorSystem + Crop 规则链替代 |
| ~~帧同步渲染层兼容两套~~ | 被统一 Property Sync 管线替代 |
| **Property Sync 体系实施** | `Docs/DualMode/IMPLEMENTATION_PLAN.md`（5 阶段，~21 天）。详见 `CORE.md` / `PROPERTY_SYNC_DESIGN.md` / `BEHAVIORINFO_LIFECYCLE_REPORT.md` |
| GDScript 扩展 | Render 层脚本化（Logic 层只允许 int）。尚未起步 |

## 五、UI

| 项目 | 说明 |
|------|------|
| MVVM 构造 | UI 系统基建 |
| UI 工作流 | 美术资源规范 |

## 六、构建

| 项目 | 说明 | 文件 |
|------|------|------|
| ~~Config.cs 构建环境判断~~ ✅ | DEV 模式 JSON 加载，非 DEV 走 ByteBuf 二进制。条件编译为设计意图，非残留 | `Config.cs` |
