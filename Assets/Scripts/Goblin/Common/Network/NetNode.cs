using ENet;
using Goblin.Core;
using Goblin.Sys.Other.View;
using Queen.Protocols.Common;
using Supyrb;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Goblin.Common.Network
{
    /// <summary>
    /// 网络节点
    /// </summary>
    public class NetNode : Comp
    {
        /// <summary>
        /// 消息包结构
        /// </summary>
        private struct NetPackage
        {
            /// <summary>
            /// 消息类型
            /// </summary>
            public Type msgType;
            /// <summary>
            /// 消息
            /// </summary>
            public INetMessage msg;
        }

        /// <summary>
        /// 注册消息回调集合
        /// </summary>
        private Dictionary<Type, List<Delegate>> messageActionMap = new();
        /// <summary>
        /// 网络消息包缓存
        /// </summary>
        private ConcurrentQueue<NetPackage> netPackages = new();

        /// <summary>
        /// ENet 线程
        /// </summary>
        private Thread thread;
        /// <summary>
        /// ENet.Host
        /// </summary>
        private Host host;
        /// <summary>
        /// ENet.Peer
        /// </summary>
        private Peer peer;
        /// <summary>
        /// 服务器 IP
        /// </summary>
        public string ip { get; private set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort port { get; private set; }
        /// <summary>
        /// ENet 轮询超时时间
        /// </summary>
        public int timeout { get; private set; }
        /// <summary>
        /// 是否在运行中
        /// </summary>
        public bool running { get; private set; }
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool connected => PeerState.Connected == peer.State;
        /// <summary>
        /// 网络延迟
        /// </summary>
        public int ping { get; private set; }
        /// <summary>
        /// 发送检测包计时
        /// </summary>
        private PingInfo sping;
        /// <summary>
        /// 接收检测包计时
        /// </summary>
        private PingInfo rping;
        /// <summary>
        /// 每秒接收数据
        /// </summary>
        public ulong bytesRecvPerSeconds { get; private set; }
        /// <summary>
        /// 每秒发送数据
        /// </summary>
        public ulong bytesSentPerSeconds { get; private set; }
        /// <summary>
        /// 记录每秒间隔上一次接收的数据量
        /// </summary>
        private ulong bytesRecv = 0;
        /// <summary>
        /// 记录每秒间隔上一次发送的数据量
        /// </summary>
        private ulong bytesSent = 0;

        /// <summary>
        /// PING 计时器 ID
        /// </summary>
        private uint pingTimingId;
        /// <summary>
        /// 计算每秒收发数据量的计时器 ID
        /// </summary>
        private uint bytesSRTimingId;

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
            pingTimingId = engine.ticker.Timing((t) =>
            {
                if (false == connected) return;

                SendPing();
            }, 1f, -1);

            bytesSRTimingId = engine.ticker.Timing((t) =>
            {
                if (false == connected) return;

                bytesRecvPerSeconds = peer.BytesReceived - bytesRecv;
                bytesSentPerSeconds = peer.BytesSent - bytesSent;
                bytesRecv = peer.BytesReceived;
                bytesSent = peer.BytesSent;
            }, 1, -1);

            host = new Host();
            host.Create();
            thread = new Thread(() =>
            {
                while (true)
                {
                    if (host.CheckEvents(out var netEvent) <= 0) if (host.Service(timeout, out netEvent) <= 0) continue;

                    switch (netEvent.Type)
                    {
                        case EventType.Connect:
                            EnqueuePackage(typeof(NodeConnectMsg), new NodeConnectMsg());
                            break;
                        case EventType.Disconnect:
                            EnqueuePackage(typeof(NodeDisconnectMsg), new NodeDisconnectMsg());
                            break;
                        case EventType.Timeout:
                            EnqueuePackage(typeof(NodeTimeoutMsg), new NodeTimeoutMsg());
                            peer.DisconnectNow(0);
                            EnqueuePackage(typeof(NodeDisconnectMsg), new NodeDisconnectMsg());
                            break;
                        case EventType.Receive:
                            var data = new byte[netEvent.Packet.Length];
                            netEvent.Packet.CopyTo(data);
                            netEvent.Packet.Dispose();
                            if (false == ProtoPack.UnPack(data, out var msgType, out var msg)) break;
                            if (typeof(NodePingMsg) == msgType)
                            {
                                RecvPing(msg as NodePingMsg);
                                break;
                            }

                            EnqueuePackage(msgType, msg);
                            break;
                    }
                }
            });
            thread.IsBackground = true;
            thread.Start();

            running = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
            engine.ticker.StopTimer(pingTimingId);
            engine.ticker.StopTimer(bytesSRTimingId);
            thread.Abort();
            host.Flush();
            host.Dispose();
            running = false;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip">地址</param>
        /// <param name="port">端口</param>
        /// <param name="timeout">网络轮询时间（ms）</param>
        public void Connect(string ip, ushort port, int timeout = 15)
        {
            rping = default;
            sping = default;
            bytesRecv = 0;
            bytesSent = 0;
            bytesRecvPerSeconds = 0;
            bytesSentPerSeconds = 0;

            this.ip = ip;
            this.port = port;
            this.timeout = timeout;
            var address = new Address();
            address.SetIP(ip);
            address.Port = port;
            peer = host.Connect(address);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (false == connected) return;
            peer.DisconnectNow(0);
            EnqueuePackage(typeof(NodeDisconnectMsg), new NodeDisconnectMsg());
        }

        /// <summary>
        /// 注销消息接收
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="action">回调方法</param>
        public void UnRecv<T>(Action<T> action) where T : INetMessage
        {
            if (false == messageActionMap.TryGetValue(typeof(T), out var actions)) return;
            if (false == actions.Contains(action)) return;

            actions.Remove(action);
        }

        /// <summary>
        /// 注册消息接收
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="action">回调方法</param>
        public void Recv<T>(Action<T> action) where T : INetMessage
        {
            if (false == messageActionMap.TryGetValue(typeof(T), out var actions))
            {
                actions = new();
                messageActionMap.Add(typeof(T), actions);
            }

            if (actions.Contains(action)) return;
            actions.Add(action);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="msg">消息</param>
        public void Send<T>(T msg) where T : INetMessage
        {
            if (false == connected)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "此操作需要网络连接，请检查是否正确连接服务器." });

                return;
            }

            if (ProtoPack.Pack(msg, out var bytes))
            {
                var packet = new Packet();
                packet.Create(bytes, bytes.Length, PacketFlags.Reliable | PacketFlags.NoAllocate);
                peer.Send(0, ref packet);
                packet.Dispose();
            }
        }

        /// <summary>
        /// 消息包入队
        /// </summary>
        /// <param name="channel">通信渠道</param>
        /// <param name="msgType">消息类型</param>
        /// <param name="msg">消息</param>
        private void EnqueuePackage(Type msgType, INetMessage msg)
        {
            netPackages.Enqueue(new NetPackage { msgType = msgType, msg = msg });
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        private void Notify()
        {
            if (false == running) return;

            while (netPackages.TryDequeue(out var package))
            {
                if (false == messageActionMap.TryGetValue(package.msgType, out var actions)) continue;
                for (int i = actions.Count - 1; i >= 0; i--) actions[i].DynamicInvoke(package.msg);
            }
        }

        private void OnTick(TickEvent e)
        {
            Notify();
        }

        #region PING
        private struct PingInfo
        {
            /// <summary>
            /// 秒
            /// </summary>
            public int seconds;
            /// <summary>
            /// 毫秒
            /// </summary>
            public int millisecond;
        }

        /// <summary>
        /// 发送延迟检测
        /// </summary>
        private void SendPing()
        {
            sping.seconds = DateTime.UtcNow.Second;
            sping.millisecond = DateTime.UtcNow.Millisecond;
            Send(new NodePingMsg());
        }

        /// <summary>
        /// 接收服务器返回的延迟检测包
        /// </summary>
        /// <param name="msg">消息</param>
        private void RecvPing(NodePingMsg msg)
        {
            rping.seconds = DateTime.UtcNow.Second;
            rping.millisecond = DateTime.UtcNow.Millisecond;
            ping = (rping.seconds * 1000 + rping.millisecond) - (sping.seconds * 1000 + sping.millisecond);
        }
        #endregion
    }
}
