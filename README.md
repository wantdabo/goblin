# Goblin
[![Ask DeepWiki](https://deepwiki.com/badge.svg)](https://deepwiki.com/wantdabo/goblin)

这是基于 Unity 开发的一套框架，支持网络游戏开发
### 大致全貌
- TCP 通信
- UDP 通信
- WebSocket 通信
- MessagePack 协议
- Yooasset 资源管理
- Luban 配置
- Timeline 编辑器
- Odin 编辑器

#### [项目结构](#projectdire)
### <span id="catalog">目录</span>
- [1.快速开始](#qstart)
- [2.环境配置](#installenv)
  - [1.安装 .NET](#installenv.1)
  - [2.安装 Unity](#installenv.2)
  - [3.安装 Odin](#installenv.3)

---

### TODO
- Gameplay 战斗模块
  - 2025-10-01
    - 新增输入/帧指令
    - 技能管线
    - 碰撞检测
    - Goblin 技能编辑器 2.0
---

#### <span id="qstart">1.快速开始</span>
- 1.开发环境中，需要安装 [**.NET8+**](#installenv.1)
- 2.安装，Unity，推荐使用 Unity 2021+ 以上版本
- 3.安装，Odin，推荐使用 3.3.1.13+ 以上版本
- 4.此时，如果上述步骤，顺利完成。使用 Unity 打开此项目即可
#### <span id="installenv">2.环境配置</span>
- ##### <span id="installenv.1">1.安装 .NET</span>
  - 该项目，Luban 配置工具依赖，需要安装 [**.NET8+**](https://dotnet.microsoft.com/zh-cn/download)
  - 同时，Luban 配置工具，以及配套的第三方插件，也是基于 .NET 来开发或调用。因此，.NET 的环境在接下来的环节中，非常重要，请确保 .NET 开发环境成功配置
- ##### <span id="installenv.2">2.安装 Unity</span>
  - 该项目，是基于 Unity 来开发。因此，需要在开发环境中，安装好 [**Unity**](https://unity.com) 推荐 Unity 2021+ 版本
- ##### <span id="installenv.3">2.安装 Odin</span>
  - 该项目，使用 [**Odin**](https://odininspector.com/) 来进行扩展编辑器。因此需要在开发环境中部署该插件。这是一款优秀的编辑器扩展插件，需要自行购买导入

---

#### <span id="projectdire">项目结构</span>
```text
├─Assets
│  ├─GameRes
│  ├─Plugins
│  ├─Resources
│  ├─Scripts
│  │  ├─Editor
│  │  └─Goblin
│  └─UERes
│      ├─Art
│      └─Gameplay
├─Config
├─Packages
└─ProjectSettings
```

- **Asset/GameRes/**  动态加载资源的目录，包含但不限，图集、贴图、预制体、场景
- **Asset/Plugins/**  第三方插件库
- **Asset/Scripts/**  业务逻辑代码
- **Asset/UERes/** Unity 的资源，例如，Texture、Mesh、Material、编辑器的中间配置
- **Configs/** 配置表，使用方式，请参考 [Luban](https://github.com/focus-creative-games/luban)
- **Packages/** Unity 的包引用
- **ProjectSettings/** Unity 项目设置
