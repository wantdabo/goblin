using Queen.Core;
using Queen.Network.Common;
using Queen.Protocols.Common;
using System.Collections.Concurrent;

namespace Queen.Network.Cross;

/// <summary>
/// RPC
/// </summary>
public class RPC : Comp
{
    /// <summary>
    /// RPC 连接 KEY
    /// </summary>
    private readonly string KEY = "QUEEN_RPC";
    /// <summary>
    /// 地址
    /// </summary>
    public string ip { get; private set; }
    /// <summary>
    /// 端口
    /// </summary>
    public ushort port { get; private set; }
    /// <summary>
    /// 空闲等待的 Client 数量
    /// </summary>
    public ushort idlecc { get; private set; }
    /// <summary>
    /// 超时设定
    /// </summary>
    public uint timeout { get; private set; }
    /// <summary>
    /// 死等上限
    /// </summary>
    public uint deadtime { get; private set; }
    /// <summary>
    /// 服务器节点
    /// </summary>
    private TCPServer server { get; set; }
    /// <summary>
    /// 路由方法列表
    /// </summary>
    private Dictionary<string, Action<CrossContext>> ractions = new();
    /// <summary>
    /// RPC 请求消息的队列
    /// </summary>
    private ConcurrentQueue<(NetChannel, ReqCrossMessage)> reqlist = new();
    /// <summary>
    /// 生成客户端节点的队列
    /// </summary>
    private ConcurrentQueue<TCPClient> bornclients = new();
    /// <summary>
    /// 客户端节点
    /// </summary>
    private ConcurrentDictionary<string, TCPClient> clients = new();

    protected override void OnCreate()
    {
        base.OnCreate();
        server = AddComp<TCPServer>();
        server.Initialize(ip, port, true, int.MaxValue, 5, int.MaxValue);
        server.Create();
        server.Recv<ReqCrossMessage>(OnReqCross);
        engine.eventor.Listen<ExecuteEvent>(OnExecute);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        server.UnRecv<ReqCrossMessage>(OnReqCross);
        server.Destroy();
        engine.eventor.UnListen<ExecuteEvent>(OnExecute);
    }

    /// <summary>
    /// 初始化 RPC
    /// </summary>
    /// <param name="ip">IP 地址</param>
    /// <param name="port">端口</param>
    /// <param name="idlecc">空闲等待的 Client 数量</param>
    /// <param name="timeout">超时设定</param>
    /// <param name="deadtime">死等上限</param>
    public void Initialize(string ip, ushort port, ushort idlecc, uint timeout, uint deadtime = 60000)
    {
        this.ip = ip;
        this.port = port;
        this.idlecc = idlecc;
        this.timeout = timeout;
        this.deadtime = deadtime;
    }

    /// <summary>
    /// 监听路由
    /// </summary>
    /// <param name="route">路径</param>
    /// <param name="action">路由动作</param>
    /// <exception cref="Exception">不能添加重复的路径</exception>
    public void Routing(string route, Action<CrossContext> action)
    {
        if (false == ractions.TryAdd(route, action)) throw new Exception("route can't be repeat.");
    }

    /// <summary>
    /// 注销路由
    /// </summary>
    /// <param name="route">路径</param>
    /// <param name="action">路由动作</param>
    public void UnRouting(string route, Action<CrossContext> action)
    {
        if (false == ractions.ContainsKey(route)) return;
        ractions.Remove(route);
    }

    /// <summary>
    /// RPC 通信
    /// </summary>
    /// <param name="ip">目标主机 IP</param>
    /// <param name="port">目标主机端口</param>
    /// <param name="route">路径</param>
    /// <param name="content">传输内容</param>
    /// <returns>RPC 结果</returns>
    public CrossResult CrossAsync(string ip, ushort port, string route, string content)
    {
        var result = new CrossResult();
        CrossAsync(ip, port, route, content, (r) =>
        {
            result.state = r.state;
            result.content = r.content;
        });

        return result;
    }

    /// <summary>
    /// RPC 通信
    /// </summary>
    /// <param name="ip">目标主机 IP</param>
    /// <param name="port">目标主机端口</param>
    /// <param name="route">路径</param>
    /// <param name="content">传输内容</param>
    /// <typeparam name="T">NewtonJson 的转化类型</typeparam>
    /// <returns>RPC 结果</returns>
    public CrossResult CrossAsync<T>(string ip, ushort port, string route, T content) where T : class
    {
        return CrossAsync(ip, port, route, Newtonsoft.Json.JsonConvert.SerializeObject(content));
    }

    /// <summary>
    /// RPC 通信
    /// </summary>
    /// <param name="ip">目标主机 IP</param>
    /// <param name="port">目标主机端口</param>
    /// <param name="route">路径</param>
    /// <param name="content">传输内容</param>
    /// <param name="callback">回调</param>
    public void CrossAsync(string ip, ushort port, string route, string content, Action<CrossResult> callback)
    {
        if (false == Online(ip, port))
        {
            callback?.Invoke(new CrossResult { state = CROSS_STATE.ERROR, content = "target host is offline." });
            return;
        }

        var id = Guid.NewGuid().ToString();
        var client = GetClient(ip, port);
        Action<NetChannel, ACKCrossMessage> ackation = null;
        Action<NetChannel, ResCrossMessage> action = null;

        bool ack = false;
        ackation = (channel, msg) =>
        {
            if (id.Equals(msg.id))
            {
                ack = true;
                client.UnRecv(ackation);
            }
        };

        action = (channel, msg) =>
        {
            if (null == action) return;
            if (msg.id != id) return;
            callback?.Invoke(new CrossResult { state = msg.state, content = msg.content });
            client.UnRecv(action);
            action = null;
        };

        client.Recv(ackation);
        client.Recv(action);
        // 发送 RPC 的数据
        client.Send(new ReqCrossMessage { id = id, route = route, content = content });
        Task.Run(async () =>
        {
            await Task.Delay((int)timeout);
            if (ack) await Task.Delay((int)deadtime);
            if (null == action) return;
            callback?.Invoke(new CrossResult { state = CROSS_STATE.TIMEOUT });
            client.UnRecv(action);
            action = null;
        });
    }

    /// <summary>
    /// RPC 通信
    /// </summary>
    /// <param name="ip">目标主机 IP</param>
    /// <param name="port">目标主机端口</param>
    /// <param name="route">路径</param>
    /// <param name="content">传输内容</param>
    /// <param name="callback">回调</param>
    /// <typeparam name="T">NewtonJson 的转化类型</typeparam>
    public void CrossAsync<T>(string ip, ushort port, string route, T content, Action<CrossResult> callback) where T : class
    {
        CrossAsync(ip, port, route, Newtonsoft.Json.JsonConvert.SerializeObject(content), callback);
    }

    /// <summary>
    /// RPC 通信
    /// </summary>
    /// <param name="ip">目标主机 IP</param>
    /// <param name="port">目标主机端口</param>
    /// <param name="route">路径</param>
    /// <param name="content">传输内容</param>
    /// <returns>RPC 结果</returns>
    public CrossResult CrossSync(string ip, ushort port, string route, string content)
    {
        var result = CrossAsync(ip, port, route, content);
        while (CROSS_STATE.WAIT == result.state) Thread.Sleep(1);

        return result;
    }

    /// <summary>
    /// RPC 通信
    /// </summary>
    /// <param name="ip">目标主机 IP</param>
    /// <param name="port">目标主机端口</param>
    /// <param name="route">路径</param>
    /// <param name="content">传输内容</param>
    /// <typeparam name="T">NewtonJson 的转化类型</typeparam>
    /// <returns>RPC 结果</returns>
    public CrossResult CrossSync<T>(string ip, ushort port, string route, T content) where T : class
    {
        return CrossSync(ip, port, route, Newtonsoft.Json.JsonConvert.SerializeObject(content));
    }

    /// <summary>
    /// RPC 目标主机是否在线
    /// </summary>
    /// <param name="ip">目标主机 IP</param>
    /// <param name="port">目标主机端口</param>
    /// <returns>YES/NO</returns>
    public bool Online(string ip, ushort port)
    {
         var client = GetClient(ip, port);

         return client.connected;
    }

    /// <summary>
    /// 申请一个新的客户端节点
    /// </summary>
    /// <param name="ip">IP 地址</param>
    /// <param name="port">端口</param>
    private TCPClient GetClient(string ip, ushort port)
    {
        var key = $"{ip}:{port}";
        // 如果池子未有客户端节点，需要等待主线程分配客户端节点
        if (false == clients.TryGetValue(key, out var client))
        {
            while (true)
            {
                if (engine.ethread) BebornClients();
                if (bornclients.TryDequeue(out client))
                {
                    clients.TryAdd(key, client);
                    break;
                }
                Thread.Sleep(1);
            }
        }

        // 目标主机建立短链接
        if (false == client.connected) client.Connect(ip, port);

        return client;
    }

    /// <summary>
    /// 生成需求的客户端节点数
    /// </summary>
    private void BebornClients()
    {
        // 新增客户端节点（需要在主线才能新增）
        while (idlecc > bornclients.Count)
        {
            var client = AddComp<TCPClient>();
            client.Initialize(true);
            client.Create();
            bornclients.Enqueue(client);
        }
    }

    /// <summary>
    /// RPC 消息通知
    /// </summary>
    private void NotifyRPC()
    {
        while (reqlist.TryDequeue(out var req))
        {
            if (false == ractions.TryGetValue(req.Item2.route, out var action))
            {
                req.Item1.Send(new ResCrossMessage { id = req.Item2.id, state = CROSS_STATE.NOTFOUND });

                return;
            }

            action.Invoke(new CrossContext(req.Item1, req.Item2.id, req.Item2.route, req.Item2.content));
        }
    }

    /// <summary>
    /// 收到 RPC 消息
    /// </summary>
    /// <param name="channel">通信渠道</param>
    /// <param name="msg">消息</param>
    private void OnReqCross(NetChannel channel, ReqCrossMessage msg)
    {
        if (msg.route.Equals("online"))
        {
            channel.Send(new ResCrossMessage { id = msg.id, state = CROSS_STATE.SUCCESS });
            return;
        }

        // 发送 ACK
        channel.Send(new ACKCrossMessage { id = msg.id });
        // RPC 消息进入队列
        reqlist.Enqueue((channel, msg));
    }

    private void OnExecute(ExecuteEvent e)
    {
        NotifyRPC();
        BebornClients();
    }
}
