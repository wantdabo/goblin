using Goblin.Core;
using Goblin.Sys.Other.View;
using Queen.Protocols.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Goblin.Common.Network
{
    /// <summary>
    /// TCP 网络节点
    /// </summary>
    public class NetTCP : NetNode
    {
        /// <summary>
        /// Socket 线程
        /// </summary>
        private Thread thread { get; set; }
        /// <summary>
        /// 通信 Socket
        /// </summary>
        private TcpClient socket { get; set; }
        /// <summary>
        /// 连接状态
        /// </summary>
        public override bool connected => null != socket && socket.Connected;
        /// <summary>
        /// 缓冲区
        /// </summary>
        private List<byte> buffer = new();
        /// <summary>
        /// bytes 读取区
        /// </summary>
        private byte[] readbytes = new byte[1024 * 1024];
        /// <summary>
        /// 包尺寸
        /// </summary>
        private byte[] psize = new byte[ProtoPack.INT32_LEN];

        /// <summary>
        /// 连接
        /// </summary>
        protected override void OnConnect()
        {
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
                        RecycleSocket();
                        EnqueuePackage(typeof(NodeDisconnectMsg), new NodeDisconnectMsg());
                    }
                });
                thread.IsBackground = true;
                thread.Start();
                EnqueuePackage(typeof(NodeConnectMsg), new NodeConnectMsg());
            }
            catch (Exception e)
            {
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "服务器未响应，请检查网络." });
                RecycleSocket();
            }
        }
        
        private void RecycleSocket()
        {
            if (null == socket) return;
            socket.Close();
            socket = null;
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        protected override void OnDisconnect()
        {
            if (thread.IsAlive) thread.Abort();
            RecycleSocket();
            EnqueuePackage(typeof(NodeDisconnectMsg), new NodeDisconnectMsg());
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
                var data = new byte[ProtoPack.INT32_LEN + bytes.Length];
                var sizebs = BitConverter.GetBytes(bytes.Length);
                Array.Copy(sizebs, 0, data, 0, sizebs.Length);
                Array.Copy(bytes, 0, data, sizebs.Length, bytes.Length);
                socket.GetStream().Write(data);
            }
        }
    }
}
