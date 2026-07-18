# Goblin 待办清单

> 2026-07-19 | 基于 README + 源码分析汇总

---

## 一、Bug / 安全修复

| 优先级 | 项目 | 说明 | 文件 |
|--------|------|------|------|
| ~~**P0**~~ ✅ | ~~`InsideNotExeToExecute` 无递归上限~~ | 常量 `MAX_INSIDE_NOTEXE_DEPTH` 移入 `FLOW_DEFINE` | `Flow.cs`, `FLOW_DEFINE.cs` |
| ~~**P0**~~ ✅ | ~~释放技能后单位卡死在 CASTING~~ | `StageCache.Valid()` 只查了 `rmvactorset`（Recycle 后清空），加 `behaviordict.ContainsKey()` 做存在性验证。根因：Magic 自动清理为新增逻辑，暴露了 `Valid()` 的设计缺陷 | `Stage.cs` |

## 二、性能优化

| 项目 | 说明 | 文件 |
|------|------|------|
| ~~Spark 按 token 建索引~~ ✅ | `sparkindex` + `indexedpipelines` 已完成 | `Flow.cs` |
| ~~RunPipeline 记 `lastCompletedIndex`~~ ✅ | `FlowInfo.completedindex` 跳过已过期指令 | `FlowInfo.cs`, `Flow.cs` |

## 三、功能缺口

| 项目 | 说明 |
|------|------|
| 音效支持 | Sound 模块未实现 |
| 抗性计算 | 护甲 / 暴击 / 闪避 | `AttributeBucket.cs:123` |
| 受击动画独立状态机 | 当前耦合在 `ChangeStateExecutor` | `ChangeStateExecutor.cs:31` |
| Pipeline.Timeline 无 Model 也支持 TRS | 引入 Vector3/Quaternion/float |

## 四、架构重构

| 项目 | 说明 |
|------|------|
| ~~Skill 转为 Actor~~ ✅ | 已通过 `Magic` Actor 体系完成（`MagicPrefab` + `MagicInfo` + `Sa/Magic.cs`），子弹合并至此 |
| Info 转 RIL 自动化 | 减少手写同步代码 |
| Clone 自动化 | 所有 Behavior 自动深拷贝 |
| RIL 合并 | 同 RIL 用最新帧号 |
| 主观 RIL 传输 | 状态同步下缓存 + 主观推送 |
| InstructData 调整 + Timeline 自适应 | 数据结构与时序适配 |
| 帧同步渲染层兼容两套 | 帧同步 / 状态同步双模式 |
| GDScript 扩展 | Render 层脚本化（Logic 层只允许 int）|

## 五、UI

| 项目 | 说明 |
|------|------|
| MVVM 构造 | UI 系统基建 |
| UI 工作流 | 美术资源规范 |

## 六、构建

| 项目 | 说明 | 文件 |
|------|------|------|
| ~~Config.cs 构建环境判断~~ ✅ | 已移除条件编译，统一走同步加载 `LoadConfigSync` |
