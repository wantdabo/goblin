using Queen.Core;
using Queen.Network.Common;
using Queen.Protocols.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Queen.Network;

/// <summary>
/// 主网组件
/// </summary>
public class Slave : Comp
{
    private TCPServer tcp { get; set; }
    private WebSocketServer ws { get; set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        engine.eventor.Listen<ExecuteEvent>(OnExecute);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        engine.eventor.UnListen<ExecuteEvent>(OnExecute);
    }

    /// <summary>
    /// 配置主网
    /// </summary>
    /// <param name="ip">地址</param>
    /// <param name="port">端口</param>
    /// <param name="wsport">WS 端口</param>
    /// <param name="maxconn">最大连接数</param>
    /// <param name="sthread">Slave（主网）最大工作线程</param>
    /// <param name="maxpps">最大网络收发包每秒</param>
    public void Initialize(string ip, ushort port, ushort wsport, int maxconn, int sthread, int maxpps)
    {
        tcp = AddComp<TCPServer>();
        tcp.Initialize(ip, port, false, maxconn, sthread, maxpps);
        tcp.Create();

        ws = AddComp<WebSocketServer>();
        ws.Initialize(ip, wsport, false, maxconn, sthread, maxpps);
        ws.Create();
    }

    private void OnExecute(ExecuteEvent e)
    {
        tcp.Notify();
        ws.Notify();
    }

    /// <summary>
    /// 注销消息接收
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="action">回调</param>
    public void UnRecv<T>(Action<NetChannel, T> action) where T : INetMessage
    {
        engine.EnsureThread();
        tcp.UnRecv(action);
        ws.UnRecv(action);
    }

    /// <summary>
    /// 注销消息接收
    /// </summary>
    /// <typeparam name="T">消息类型</typeparam>
    /// <param name="action">回调</param>
    public void Recv<T>(Action<NetChannel, T> action) where T : INetMessage
    {
        engine.EnsureThread();
        tcp.Recv(action);
        ws.Recv(action);
    }
}
