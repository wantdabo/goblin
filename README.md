# Queen

这是基于 .NET 的多进程单线程执行的跨平台服务端。
在这个高并发，高性能，分布式的时代。
有时候，不需要那么高的性能，只需要一个简单，易用的服务器罢了。

基础运行环境 **.NET8+**

### 大致全貌

- ENet 套字节
- MessagePack 协议
- MongoDB 数据库
- Luban 配置

### *快速部署

- **step 1** 开发环境中，需要安装 **.NET8+**
- **step 2** 安装，MongoDB，根据文件 **'./Queen.Server/Res/queen_mongo'** 进行数据库创建。
- **step 3** 服务器的配置文件，位于 **'./Queen.Server/Res/settings.json'** 可以根据需要进行修改。
- **step 4** 此时，如果上述步骤，顺利完成。**支持 .NET8+ 的 IDE** 打开 **'./Queen.sln'**解决方案，运行 **'Queen.Server'** 项目即可。

### [DOCUMENT](#catalog)

### <span id="catalog">目录</span>

- 1.占坑测试
- 2.占坑测试
- 3.占坑测试
- 4.占坑测试

### TODOLIST

- Ticker 计时器 BUG

- 时间轮，用于定时执行某些任务

- RPC 改为短链接（如果同时出现多个一致目标，不创建新连接。直到所有任务完成关闭短链接）

- RPC 发送，返回对应（因为之前是单线程，一来一回。现在 Role 独立为多线程，存在两个 Role 同时 RPC，需要确定返回的点对点）

- 远程日志存盘系统，因为 Server 这些后面是要分布式的。所以，Logger 系统需要集中远程日志

- Server 与 Server 之间的交互事务机制

- ENet 修改最大连接数，4095 -> 65535

- H5 小游戏 WebSocket 支持

- TCP 支持
