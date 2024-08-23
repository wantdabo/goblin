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
using System.Net.Sockets;
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
        /// 消息回调接口
        /// </summary>
        private interface INetMessageAction
        {
            /// <summary>
            /// 消息执行
            /// </summary>
            /// <param name="msg">消息</param>
            void Invoke(object msg);
        }

        /// <summary>
        /// 消息回调代理类
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        private class NetMessageAction<T> : INetMessageAction
        {
            private readonly Action<T> action;

            public NetMessageAction(Action<T> action)
            {
                this.action = action;
            }

            public void Invoke(object msg)
            {
                action.Invoke((T)msg);
            }
        }

        /// <summary>
        /// 消息回调映射
        /// </summary>
        private Dictionary<Type, List<Delegate>> messageActionDict = new();
        /// <summary>
        /// 消息回调
        /// </summary>
        private Dictionary<Delegate, INetMessageAction> netMessageActionDict = new();
        /// <summary>
        /// 网络消息包缓存
        /// </summary>
        private ConcurrentQueue<NetPackage> netPackages = new();

        /// <summary>
        /// Socket 线程
        /// </summary>
        private Thread thread { get; set; }
        /// <summary>
        /// 通信 Socket
        /// </summary>
        private TcpClient socket { get; set; }
        /// <summary>
        /// 服务器 IP
        /// </summary>
        public string ip { get; private set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort port { get; private set; }
        /// <summary>
        /// 是否在运行中
        /// </summary>
        public bool running { get; private set; }
        /// <summary>
        /// 连接状态
        /// </summary>
        public bool connected => null != socket && socket.Connected;
        
        /// <summary>
        /// 缓冲区
        /// </summary>
        private List<byte> buffer = new();
        /// <summary>
        /// bytes 读取区
        /// </summary>
        private byte[] readbytes = new byte[1024];
        /// <summary>
        /// 包尺寸
        /// </summary>
        private byte[] psize = new byte[ProtoPack.INT32_LEN];

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
            running = true;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
            running = false;
        }
        
        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip">地址</param>
        /// <param name="port">端口</param>
        public void Connect(string ip, ushort port)
        {
            if (connected) return;

            this.ip = ip;
            this.port = port;

            socket = new TcpClient();
            try
            {
                socket.Connect(ip, port);
                thread = new Thread(() =>
                {
                    try
                    {
                        while (true)
                        {
                            while (buffer.Count >= ProtoPack.INT32_LEN)
                            {
                                psize[0] = buffer[0];
                                psize[1] = buffer[1];
                                psize[2] = buffer[2];
                                psize[3] = buffer[3];
                                var size = BitConverter.ToInt32(psize.ToArray()) + psize.Length;

                                if (buffer.Count < size) break;
                                var data = buffer.GetRange(ProtoPack.INT32_LEN, size - ProtoPack.INT32_LEN).ToArray();
                                buffer.RemoveRange(0, size);

                                if (false == ProtoPack.UnPack(data, out var msgType, out var msg)) break;

                                EnqueuePackage(msgType, msg);
                            }

                            if (false == connected) continue;
                            var len = socket.GetStream().Read(readbytes, 0, readbytes.Length);
                            for (int i = 0; i < len; i++) buffer.Add(readbytes[i]);
                        }
                    }
                    catch (Exception e)
                    {
                        socket.Close();
                        socket.Dispose();
                        EnqueuePackage(typeof(NodeDisconnectMsg), new NodeDisconnectMsg());
                    }
                });
                thread.IsBackground = true;
                thread.Start();
                EnqueuePackage(typeof(NodeConnectMsg), new NodeConnectMsg());
            }
            catch (Exception e)
            {
                engine.eventor.Tell(new MessageBlowEvent{ type = 2, desc = "服务器未响应，请检查网络."});
                socket.Close();
                socket.Dispose();
            }
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (false == connected) return;
            if (thread.IsAlive) thread.Abort();
            socket.Close();
            socket.Dispose();
            EnqueuePackage(typeof(NodeDisconnectMsg), new NodeDisconnectMsg());
        }

        /// <summary>
        /// 注销消息接收
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="action">回调方法</param>
        public void UnRecv<T>(Action<T> action) where T : INetMessage
        {
            if (false == messageActionDict.TryGetValue(typeof(T), out var actions)) return;
            if (false == actions.Contains(action)) return;
            netMessageActionDict.Remove(action);
            actions.Remove(action);
        }

        /// <summary>
        /// 注册消息接收
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="action">回调方法</param>
        public void Recv<T>(Action<T> action) where T : INetMessage
        {
            if (false == messageActionDict.TryGetValue(typeof(T), out var actions))
            {
                actions = new();
                messageActionDict.Add(typeof(T), actions);
            }

            if (actions.Contains(action)) return;
            netMessageActionDict.Add(action, new NetMessageAction<T>(action));
            actions.Add(action);
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="msg">消息</param>
        public void Send<T>(T msg) where T : INetMessage
        {
            if (false == connected) return;
            if (ProtoPack.Pack(msg, out var bytes))
            {
                var data = new byte[ProtoPack.INT32_LEN + bytes.Length];
                var sizebs = BitConverter.GetBytes(bytes.Length);
                Array.Copy(sizebs, 0, data, 0, sizebs.Length);
                Array.Copy(bytes, 0, data, sizebs.Length, bytes.Length);
                socket.GetStream().Write(data);
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
                if (false == messageActionDict.TryGetValue(package.msgType, out var actions)) continue;
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    if (netMessageActionDict.TryGetValue(actions[i], out var action)) action.Invoke(package.msg);
                }
            }
        }
      
        private void OnTick(TickEvent e)
        {
            Notify();
        }
    }
}
