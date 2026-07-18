# Goblin
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/wantdabo/goblin)

基于 Godot 4 (C#) 的动作游戏框架，支持网络游戏开发。Logic 层纯 C#（确定性，可复用），Render 层依赖 Godot API。

### 大致全貌
- TCP / UDP / WebSocket 通信
- MessagePack 协议
- Luban 配置
- Pipeline.Timeline 技能编辑器
- Logic / Render 分层（Agent Chase 模式）
- 帧同步 / 状态同步双模式

### TODO
- Gameplay 战斗模块
  - 2026-01-01
    - 死亡流程 ⭐
    - 状态机重新梳理完善 ⭐
    - Flow 支持事件执行 ⭐
    - Flow 支持事件派发 ⭐
    - 顿帧 ⭐
    - 受击效果 ⭐
    - 音效支持
    - Pipeline.Timeline 处理无 Model 也需要支持 TRS, 引入 Vector3/Quaternion/float
    - AddBehaviorInfo/AddBehavior 存在差一帧时序问题 ✅ 已修复
      - 根因：RmvBehaviorInfo/RmvBehavior 是延迟回收（帧末 Recycle），但立即将 active 置为 false；SeekBehaviorInfo 因 active=false 返回 false，业务误判为不存在，AddBehaviorInfo 时 dict 里还有旧实例 → 抛出重复添加异常，卡死
      - 修法：AddBehaviorInfo/AddBehavior 中检查 rmvbehaviorinfos/rmvbehaviors 列表，若目标在其中则取消移除并重新初始化（Reset+Ready / active=true+AddBindingInfo），而非直接抛异常
    - 碰撞检测 (CollisionExecutor) -> 命中火花 BUG ✅ 已修复 (2026-07-19)
      - 修法：Spark() 同步调用返回后立即 flowcollision.targets.Clear()
      - Spark → ET_FLOW_HIT → Do() 链全程同步，Clear 时消费者已处理完毕
      - 第二条消费路径（直接读 targets）实际不存在——DamageExecutor 走参数，HitLagExecutor 在 Do() 循环内部

  - 2026-05-01
    - Flow.cs 深度分析 ✅ 逐路径追踪验证 📝 Docs/ANALYSIS_FLOW.md
      - 5条核心路径全部追踪：创建/Tick/EndPipeline/条件重试/Spark——基本正确
      - 真问题：P0 InsideNotExeToExecute 无递归上限 / P1 ET_FLOW_HIT doings 脏数据 / P2 Spark O(F×P×S)
      - P4(Read重复反序列化)不成立——PipelineDataReader 有字典缓存
    - Skill 转为 Actor（子弹合并至此）
    - Info 转 RIL 自动化
    - 所有 Clone 自动化
    - InstructData 数据调整，Timeline 自适应
    - RIL 合并（同 RIL 使用最新帧号，避免浪费性能在旧 RIL）
    - 引入主观 RIL 传输，因为兼容状态同步，有一些数据，状态同步需要缓存，但是主观推送
    - 帧同步的渲染层开发过程中，需要兼容两套
    - Scripting 扩展 GDScript（Render 层随意；Logic 层只允许 int，禁用 float）

- UI 模块
  - 2026-01-01
    - MVVM 构造
    - UI 工作流（美术限制）

---

### 快速开始

1. 安装 [.NET 8+](https://dotnet.microsoft.com/zh-cn/download)
2. 安装 [Godot 4.x (C# / Mono 版)](https://godotengine.org/)
3. 用 Godot 打开 `godot/` 目录即可

### 项目结构

```text
├─Config/          Luban 配置表（源数据 + 生成产物）
│  ├─Commands/     gen / godot_copy 脚本
│  ├─Datas/        Excel 源表
│  └─Cfg/          生成的 CS + Bytes
└─godot/           Godot 4 项目
   ├─GameRes/      动态加载资源（.tscn / .glb / .wav 等）
   ├─Plugins/      第三方库（LiteNetLib / MessagePack / Luban）
   └─Scripts/
      └─Goblin/
         ├─Core/       Comp / Engine / Export
         ├─Common/     Eventor / Ticker / ObjectPool / FSM / Network / Conf
         ├─Phases/     LoginPhase / GamingPhase
         ├─Sys/        UI 系统 / Proxy / Model
         └─Gameplay/
            ├─Logic/   纯 C# 确定性逻辑层（零 Godot 依赖）
            ├─Render/  Godot 渲染层（Agent / Batch / Enchant）
            └─Director/ 连接 Logic ↔ Render
```
