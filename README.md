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

详见 [Docs/TODO.md](Docs/TODO.md)。

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
