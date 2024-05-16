using ENet;
using Goblin.Core;
using Goblin.Sys.Other.View;
using Queen.Network.Protocols;
using Queen.Network.Protocols.Common;
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
            public Type msgType;
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

        private Thread thread;
        private Host host;
        private Peer peer;
        public string ip { get; private set; }
        public ushort port { get; private set; }
        public int timeout { get; private set; }

        public bool running { get; private set; }

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool connected => PeerState.Connected == peer.State;

        private uint sendPingTimingId;

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);

            Recv<NodePingMsg>(OnNodePing);
            sendPingTimingId = engine.ticker.Timing((t) =>
            {
                if (false == connected) return;
                SendPing();
            }, 0.5f, -1);

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
                            break;
                        case EventType.Receive:
                            var data = new byte[netEvent.Packet.Length];
                            netEvent.Packet.CopyTo(data);
                            netEvent.Packet.Dispose();
                            if (false == ProtoPack.UnPack(data, out var msgType, out var msg)) return;

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
            thread.Abort();
            host.Dispose();
            running = false;
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip">地址</param>
        /// <param name="port">端口</param>
        /// <param name="timeout">端口</param>
        public void Connect(string ip, ushort port, int timeout = 15)
        {
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
                if (false == messageActionMap.TryGetValue(package.msgType, out var actions)) return;
                if (null == actions) return;
                for (int i = actions.Count - 1; i >= 0; i--) actions[i].DynamicInvoke(package.msg);
            }
        }

        private void OnTick(TickEvent e)
        {
            Notify();
        }

        private struct PingInfo
        {
            public int seconds;
            public int millisecond;
        }

        private PingInfo sping;
        private PingInfo rping;
        public int ping { get; private set; }

        private void SendPing()
        {
            sping.seconds = DateTime.UtcNow.Second;
            sping.millisecond = DateTime.UtcNow.Millisecond;
            Send(new NodePingMsg());
        }

        private void OnNodePing(NodePingMsg msg)
        {
            rping.seconds = DateTime.UtcNow.Second;
            rping.millisecond = DateTime.UtcNow.Millisecond;
            ping = (rping.seconds * 1000 + rping.millisecond) - (sping.seconds * 1000 + sping.millisecond);
        }
    }
}
