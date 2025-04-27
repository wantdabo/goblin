# Queen

基于 .NET 的多进程多线程跨平台游戏服务端。

### 大致全貌
- TCP 通信
- UDP 可靠通信
- WebSocket 通信（支持 SSL）
- RPC 远程调用
- 轻量协程
- MessagePack 协议
- MongoDB 数据库
- Luban 配置

#### [项目结构](#projectdire)
### <span id="catalog">目录</span>

- [1.快速开始](#qstart)
- [2.环境配置](#installenv)
    - [1.安装 .NET](#installenv.1)
    - [2.安装/配置 MongoDB](#installenv.2)
        - [1.下载安装 MongoDB](#installenv.2.1)
- [3.网络协议](#netproto)
    - [1.定义协议](#netproto.1)
    - [2.生成协议](#netproto.2)
    - [3.使用协议](#netproto.3)
- [4.配置表](#config)
    - [1.定义配置](#config.1)
    - [2.导出配置](#config.2)
- [5.服务器配置](#servsettings)
- [6.业务架构](#bizframework)
    - [1.大致架构](#bizframework.1)
    - [2.Role](#bizframework.2)
        - [1.初识 Role](#bizframework.2.1)
        - [2.Role 工作方式](#bizframework.2.2)
        - [3.RoleBehavior](#bizframework.2.3)
        - [4.IDBState](#bizframework.2.4)
    - [3.定位系统](#bizframework.3)
    - [4.事务系统](#bizframework.4)
---

### TODO
- Role 冷销毁/热内存常驻
- Eventor Tell 执行过程移除导致 BUG
- 时间轮，用于定时执行某些任务
- 增加崩溃容灾数据保存抢救
- Queen.Bot 设计
- Ticker 计时器 BUG
- 远程日志存盘系统，因为 Server 这些后面是要分布式的。所以，Logger 系统需要集中远程日志

---

#### <span id="qstart">1.快速开始</span>
- 1.开发环境中，需要安装 [**.NET8+**](#installenv.1)
- 2.安装，[MongoDB](#installenv.2.1)
- 3.此时，如果上述步骤，顺利完成。**支持 .NET8+ 的 IDE** 打开 **`./Queen.sln`** 解决方案，运行 **`Queen.Server.csproj`** 项目即可
#### <span id="installenv">2.环境配置</span>
- ##### <span id="installenv.1">1.安装 .NET</span>
    - 该项目，是基于 .NET8 来开发。因此，需要在开发环境中，安装好 [**.NET8+**](https://dotnet.microsoft.com/zh-cn/download)
    - 同时，MessagePack、Luban 配置工具也是基于 .NET 来开发的。因此，.NET 的环境在接下来的环节中，非常重要，请确保 .NET 开发环境成功配置
- ##### <span id="installenv.2">2.安装/配置 MongoDB</span>
    - <span id="installenv.2.1">1.下载安装 [MongoDB](https://www.mongodb.com/products/self-managed/community-edition)</span>

#### <span id="netproto">3.网络协议</span>
- ##### <span id="netproto.1">1.定义协议</span>
    - 协议的定义，需要去到 `./Queen.Protocols/` 定义，因为使用的是 **[MessagePack-CSharp](https://github.com/MessagePack-CSharp/MessagePack-CSharp)**，只需要定义 `Class/类型` 即可。解读，登录协议，协议文件 `./Queen.Protocols/LoginProto.cs`
    ```csharp
    /// <summary>
    /// 请求登录消息
    /// </summary>
    [MessagePackObject(true)]
    public class C2SLoginMsg : INetMessage
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string password { get; set; }
    }

    /// <summary>
    /// 响应登录消息
    /// </summay>
    [MessagePackObject(true)]
    public class S2CLoginMsg : INetMessage
    {
        /// <summary>
        /// 操作码/ 1 登录成功，2 用户不存在，3 密码错误
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 玩家 ID
        /// </summary>
        public string uuid { get; set; }
    }
    ```
    
    - 定义了 `C2SLoginMsg`、`S2CLoginMsg`，继承 `INetMessage` 接口，这样就定义好了两条协议了。C2S 表示，**ClientToServer**，S2C 表示 **ServerToClient**，使用这个前缀只是为了更好区分协议的流向
    - Class/类型中的结构，全部都是 `get`、`set` 属性。例如，`public string username { get; set; }` 同时，标记 MessagePackObject 特性,`[MessagePackObject(true)]`
    - 有时候，期望复用一些数据结构，又不是协议。那么，不继承 `INetMessage` 接口即可。下方给出示例代码
    - **\*注意**，结构中的`属性`不允许定义 Dictionary、List 。需要定义集合，请使用 Array，例如 `public uint[] items { get; set; }`

    ```csharp
    /// <summary>
    /// 数据结构
    /// </summary>
    [MessagePackObject(true)]
    public class Person
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string name{ get; set; }
        /// <summary>
        /// 年龄
        /// </summary>
        public uint age{ get; set; }
    }
    ```
    - ##### <span id="netproto.2">2.生成协议</span>
        - 因为协议在序列化后，传输，反序列化才能方便读取。协议的包头，需要定义协议号，根据不同的协议号来反序列化。框架底层自动分配协议号，运行 `./Commands/proto.bat` 同时，也会生成解析协议的静态查询表。`因为兼顾到前端的协议使用（不能通过反射去解析，IOS 不允许）`
        - **\*注意**，生成协议解析表，使用了 `MessagePack-CSharp` 的 `mpc` 指令，如果开发环境未支持，请参照下方指令进行装配
        ```text
        dotnet tool install --global MessagePack.Generator
        dotnet new tool-manifest
        dotnet tool install MessagePack.Generator
        dotnet mpc --help
        ```
        在控制台，执行上述指令，即可
    - ##### <span id="netproto.3">2.使用协议</span>
        - 消息监听
        ```csharp
        engine.slave.Recv<C2SLoginMsg>(OnC2SLogin);
        ```
        - 消息发送
        ```csharp
        // channel 是端对端的 Socket 连接
        channel.Send(new S2CLoginMsg { uuid = uuid, code = 1 });
        ```
        - 以上的例子，原生的消息监听及发送。在业务的开发过程中。例如，在未登录的阶段，无法确定玩家身份，只能通过这种方式来接收所有的消息及发送。针对确定玩家的消息监听/发送，也是业务最常用的消息监听及发送，详情请看 **[Role](#bizframework.2)** 的概念
- #### <span id="config">4.配置表</span>
    - ##### <span id="config.1">1.定义配置</span>
        - `./Config/Datas/` 存放所有 Excel 表
        - 配置，使用第三方 Luban 工具，使用方式，请查阅 [Luban](https://github.com/focus-creative-games/luban) 源项目
    - ##### <span id="config.2">2.导出配置</span>
        - `./Config/Commands/` 包含了配置表相关的 `BAT`
        - `./Config/Commands/gen.bat` 导出配置信息到 `./Config/Cfg/Bytes` 二进制数据，`./Config/Cfg/CS` 用于解析二进制数据的 CSharp 文件
        - `./Config/Commands/server_copy.bat` 导出在 `./Config/Cfg/` 中的配置信息，复制到业务项目中
        - `./Config/Commands/server_auto.bat` **\*推荐使用**，自动依次调用 `gen.bat`、`server_copy.bat`
- #### <span id="servsettings">5.服务器信息配置</span>
打开 `./Queen.Server/Res/settings.json` 文件，进行服务器信息配置
```text
{
  // 服务器名字
  "name": "queen.server",
  // 主机
  "host": "0.0.0.0",
  // 端口
  "port": 12801,
  // 最大连接数 (最大 4095)
  "maxconn": 4095,
  // 数据库主机
  "dbhost": "127.0.0.1",
  // 数据库端口
  "dbport": 27017,
  // 数据库名
  "dbname": "queen",
  // 数据库身份校验
  "dbauth": false,
  // 数据库用户名
  "dbuser": "root",
  // 数据库密码
  "dbpwd": "root",
  // 数据落地时间间隔（秒）
  "dbsave": 5
}
```
- #### <span id="bizframework">6.业务架构</span>
    - ##### <span id="bizframework.1">1.大致架构</span>
        - [项目结构](#projectdire) 最外层的 `./Queen/` 与 `./Queen.Server/`
        - `Queen` 是一个公共库，包含了，核心库、数据库、网络通信、配置表、RPC 等相关的基础支持
        - `Queen.Server` 基于 `Queen` 设计了一套以 **[Role](#bizframework.2)** 为核心的业务框架，也是主要的业务逻辑。他是一个多进程，多线程的框架（Role 业务逻辑是单线程处理）
        - 因此，在整体的设计中。可以选择直接使用 `Queen.Server` 来完成业务。也可以基于 `Queen` 设计一套符合自己预期的业务框架。不论多线程还是单线程
        - 包括，可能需要设计，事务系统、Gameplay、聊天系统，都可以基于 `Queen` 来设计一个进程
    - ##### <span id="bizframework.2">2.Role</span>
        - <span id="bizframework.2.1">1.初识 Role</span>
            - Role 就是玩家，在 `Queen.Server` 中实现的。在 `Queen.Server` Role 是最小并发单位。玩家通过了验证，就会产生一个 Role，与前端保持长连接通信，业务处理
            - `Queen.Server` 是多线程的设计。但是，Role 自身的业务逻辑处理是单线程的。Role 与 Role 之间是无法直接访问的，尽管他们在同一个 `Queen.Server`
            - 简单举个例子,`Queen.Server` 中，Role 作为最小单位多线程并发。 **Role [ Bag、Equip、Mail ...]** 自身的业务，均为单线程处理。
            - 因此，`Queen.Server` 是可以做到一直扩展（开进程），承载无数的 Role 进行服务
            - 如果需要 Role 与 Role 之间的交互，例如，Role-A 给 Role-B 转账。这种情况，需要依赖 `定位系统`、`事务系统`，因为 Role 在 `Queen.Server-A` 进行业务，也可以在 `Queen.Server-B` 进行业务（同时段，无法共存）。所以，需要定位到 Role 处于哪个 `Queen.Server` 才可以进行 RPC 通信。同时，因为跨了进程/线程进行通信处理业务，所以，Role-A 付款，Role-B 收款，需要借助 `事务系统`，双方业务完成，才是真正完成。任意一方失败/超时，均为失败。此时，双方的数据回滚到业务开始之前。
            - 请查阅，[定位系统]()、[事务系统]()
        - <span id="bizframework.2.2">2.Role 工作方式</span>
            - Role 由 N 个 RoleBehavior 构成的。每一个 RoleBehavior 就是 Role 的功能。RoleBehavior 之间可以放心的相互访问，Role 内部是单线程（绝对安全）。例如，`Bag : RoleBehavior、 Mail : RoleBehavior` Mail 接收物品道具，就可以直接调用 Bag 进行物品的新增
            - Role 每一条任务。例如，接收到玩家的请求消息，会在单线程中队列分发到每一个 RoleBehavior
            - 因此，Role 只是组合 RoleBehavior，分发任务的一个载体，具体的业务逻辑在 RoleBehavior
        - <span id="bizframework.2.3">3.RoleBehavior</span>
            - RoleBehavior 就是单个业务本身
            - 以 `Bag/背包` 举例。背包中的道具物品，需要持久化，写入到数据库中。逻辑的运行过程中，还需要频繁读写。因此，RoleBehavior 中有 Data 缓存在内存中的
            - 所以，功能的数据读写，就在 RoleBehavior 的 IDBState 中，Bag 有 BagData : IDBState、 Mail 有 MailData : IDBState。数据的颗粒度在 RoleBehavior 层
            - 得益于 Role 的单线程调度，RoleBehavior 的逻辑，可以不考虑多线程带来的安全问题。只要是 Role 的 RoleBehavior 业务处理，可以放心的随意调度
            - RoleBehavior 绑定一个 Data 缓存，逻辑与数据是分离的。开始任务前，数据会备份。如果出现了 `错误、事务超时...` 数据可以安全的回滚到任务前，任务过程中产生对前端的推送也会被取消
        - <span id="bizframework.2.4">4.IDBState</span>
            - RoleBehavior 有提到可以绑定一个 Data 来进行业务存储。就是当前所说的 IDBState
            - IDBState 的数据会根据 RoleBehavior 中的身份特征来写库，也是数据持久化的重要一环
            - 下方给出定义的例子
            ```csharp
            /// <summary>
            /// 背包数据
            /// </summary>
            [MessagePackObject(true)]
            public class BagData : IDBState
            {
                /// <summary>
                /// 自增 ID
                /// </summary>
                public int incrementId { get; set; } = 1000;
                /// <summary>
                /// 背包物品集合
                /// </summary>
                public List<BagItem> items { get; set; } = new();
            }

            /// <summary>
            /// 背包
            /// </summary>
            public class Bag : RoleBehavior<BagData, Adapter> {}
            ```
            用背包来举例，BagData 继承 IDBState 接口，同时标记特性 `[MessagePackObject(true)]`，结构使用属性来进行定义，例如 `public int incrementId { get; set; } = 1000;` 如此以来就实现一个专属 Bag 的专属 BagData
            - IDBState 跟定义协议大同小异。不过是继承 IDBState 而不是 INetMessage
    - ##### <span id="bizframework.3">3.定位系统</span>
        - 暂未实现
    - ##### <span id="bizframework.4">4.事务系统</span>
        - 暂未实现

---

#### <span id="projectdire">项目结构</span>
```text
├─Commands
├─Config
├─Queen
│  ├─3rd
│  ├─Common
│  ├─Core
│  └─Network
├─Queen.Protocols
│  └─Common
├─Queen.Protocols.Gen
├─Queen.Remote
│  └─Res
└─Queen.Server
    ├─Core
    ├─Logic
    ├─Res
    ├─Roles
    └─System
```

- **Commands/** BAT/SHELL，包含但不限，导表、协议生成/导出指令
- **Configs/** 配置表，使用方式，请参考 [Luban](https://github.com/focus-creative-games/luban)
- **Queen/** 核心库
- **Queen.Protocols/** 协议定义
- **Queen.Protocols.Gen/** 协议生成
- **Queen.Remote/** RPC 库
- **Queen.Server/** 业务
