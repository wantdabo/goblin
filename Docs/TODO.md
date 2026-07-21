# Goblin 待办清单

> 2026-07-22 | 更新于动画槽位优先级系统 Phase 1+2 落地

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
| 逐层动画 elapsed + AnimationTree | Phase 2.5。当前 `info.animelapsed` 单值共享所有层，多层独立进度无法追踪。需 RIL `LayerAnimEntry` 加 elapsed + AnimationAgent 接入 AnimationTree 骨骼遮罩混合 | `RIL_FACADE_ANIMATION.cs`, `AnimationAgent.cs` |
| 槽位 key 复合化（多命名动画共存）| Phase 3。`SLOT_NAMED` 单 key，同帧不同 layer 命名动画互斥。需复合键或新增 per-layer slot key | `Facade.cs`, `ANIM_DEFINE.cs` |
| 动画事件帧系统 | Phase 4。动作游戏出伤帧 / 取消窗与动画进度解耦（当前靠 Pipeline 时间线硬编码）。需 AnimationConfig 加 event frames + Logic 层按 elapsed 回调 | `AnimationConfig.cs` |
| 确定性混合参数 | Phase 5。帧同步下 BlendSpace 参数（速度→走/跑混合）只在 Render 层，两端可能分歧。需 RIL 扩展 blend weight | `RIL_FACADE_ANIMATION.cs` |
| Godot Pipeline 可视化编辑器 | Scripting / Timeline / GraphNode 三种编辑方式并存，统一底层 Pipeline 数据结构 |

## 四、架构重构

| 项目 | 说明 |
|------|------|
| ~~Skill 转为 Actor~~ ✅ | 已通过 `Magic` Actor 体系完成（`MagicPrefab` + `MagicInfo` + `Sa/Magic.cs`） |
| 穿透式目标查找规则 | **部分推进：** `ET_MAGIC_OWNER` 已移除，统一用 `ET_CASTER`。**待完成：** 替代所有硬编码 ET 常量，用规则链动态解析目标，指令复用时不论上下文都能找到正确目标。参考 `Docs/PENETRATING_TARGET_RESOLUTION.md` |
| ~~CheckCondition 移入 ExecuteInstruct~~ ✅ | 已统一到 `ExecuteInstruct` 内部 |
| Info 转 RIL 自动化 | 减少手写同步代码 |
| Clone 自动化 | 所有 Behavior 自动深拷贝 |
| RIL 合并 | 同 RIL 用最新帧号 |
| 主观 RIL 传输 | 状态同步下缓存 + 主观推送 |
| 帧同步渲染层兼容两套 | 帧同步 / 状态同步双模式 |
| GDScript 扩展 | Render 层脚本化（Logic 层只允许 int）。当前 `.gd` 文件 0 个，尚未起步 |

## 五、UI

| 项目 | 说明 |
|------|------|
| MVVM 构造 | UI 系统基建 |
| UI 工作流 | 美术资源规范 |

## 六、构建

| 项目 | 说明 | 文件 |
|------|------|------|
| ~~Config.cs 构建环境判断~~ ✅ | DEV 模式 JSON 加载，非 DEV 走 ByteBuf 二进制。条件编译为设计意图，非残留 | `Config.cs` |
