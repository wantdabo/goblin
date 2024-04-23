using Goblin.Core;
using Queen.Network.Protocols;
using Queen.Network.Protocols.Common;
using Supyrb;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Common.Network
{
    /// <summary>
    /// 网络节点
    /// </summary>
    public class NetNode : Comp
    {
        private ENetClient client;

        /// <summary>
        /// 注册消息回调集合
        /// </summary>
        private Dictionary<Type, List<Delegate>> messageActionMap = new();

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        /// <summary>
        /// 连接
        /// </summary>
        /// <param name="ip">地址</param>
        /// <param name="port">端口</param>
        public void Connect(string ip, int port)
        {
            if (null != client) client.Dispose();

            client = new ENetClient();
            client.OnConnect = () => { Notify(typeof(NodeConnectMsg), new NodeConnectMsg { }); };
            client.OnDisconnect = () => { Notify(typeof(NodeDisconnectMsg), new NodeDisconnectMsg { }); };
            client.OnTimeout = () => { Notify(typeof(NodeTimeoutMsg), new NodeTimeoutMsg { }); };
            client.Connect(ip, port);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (null == client) return;
            client.Disconnect();
        }

        /// <summary>
        /// 注销消息接收
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="action">回调方法</param>
        public void UnListen<T>(Action<T> action) where T : INetMessage
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
        public void Listen<T>(Action<T> action) where T : INetMessage
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
            if (ProtoPack.Pack(msg, out var bytes)) client.Send(bytes);
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        private void Notify(Type msgType, INetMessage msg)
        {
            if (false == messageActionMap.TryGetValue(msgType, out var actions)) return;
            if (null == actions) return;
            for (int i = actions.Count - 1; i >= 0; i--) actions[i].DynamicInvoke(msg);
        }

        private void OnTick(TickEvent e)
        {
            bool connected = client != null && client.IsConnected;

            if (!connected || client.BufferPointer.Count == 0)
            {
                return;
            }

            while (client.BufferPointer.Count > 0)
            {
                var pointer = client.BufferPointer.Dequeue();
                var bytes = new byte[pointer.Length];
                Array.Copy(client.Buffer, pointer.Start, bytes, 0, pointer.Length);
                if (ProtoPack.UnPack(bytes, out var msgType, out var msg)) Notify(msgType, msg);
            }
        }
    }
}
