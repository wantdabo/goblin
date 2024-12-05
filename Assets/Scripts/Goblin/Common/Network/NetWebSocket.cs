using Goblin.Sys.Other.View;
using Queen.Protocols.Common;
using System;
using UnityWebSocket;

namespace Goblin.Common.Network
{
    /// <summary>
    /// WebSocket 网络节点
    /// </summary>
    public class NetWebSocket : NetNode
    {
        /// <summary>
        /// 通信 Socket
        /// </summary>
        private WebSocket socket { get; set; }
        public override bool connected => null != socket && WebSocketState.Open == socket.ReadyState;

        /// <summary>
        /// 连接
        /// </summary>
        protected override void OnConnect()
        {
            string address = $"ws://{ip}:{port}/ws";
            socket = new WebSocket(address);
            socket.OnOpen += ((sender, args) =>
            {
                EnqueuePackage(typeof(NodeConnectMsg), new NodeConnectMsg());
            });
            socket.OnClose += ((sender, args) =>
            {
                EnqueuePackage(typeof(NodeDisconnectMsg), new NodeDisconnectMsg());
            });
            socket.OnMessage += ((sender, args) =>
            {
                if (false == args.IsBinary) return;
                if (false == ProtoPack.UnPack(args.RawData, out var msgType, out var msg)) return;
                
                EnqueuePackage(msgType, msg);
            });
            socket.OnError += ((sender, args) =>
            {
                socket.CloseAsync();
            });
            socket.ConnectAsync();
        }
        
        /// <summary>
        /// 断开连接
        /// </summary>
        protected override void OnDisconnect()
        {
            socket.CloseAsync();
        }
        
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="msg">消息</param>
        protected override void OnSend<T>(T msg)
        {
            if (ProtoPack.Pack(msg, out var bytes))
            {
                socket.SendAsync(bytes);
            }
        }
    }
}
