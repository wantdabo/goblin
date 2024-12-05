using Goblin.Core;
using Goblin.Sys.Other.View;
using Queen.Protocols.Common;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Goblin.Common.Network
{
    /// <summary>
    /// 网络节点
    /// </summary>
    public abstract class NetNode : Comp
    {
        /// <summary>
        /// 消息包结构
        /// </summary>
        private struct NetPackage
        {
            /// <summary>
            /// 消息类型
            /// </summary>
            public Type msgType { get; set; }
            /// <summary>
            /// 消息
            /// </summary>
            public INetMessage msg { get; set; }
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
        private ConcurrentQueue<NetPackage> netpackages = new();
        
        /// <summary>
        /// 服务器 IP
        /// </summary>
        public string ip { get; protected set; }
        /// <summary>
        /// 服务器端口
        /// </summary>
        public ushort port { get; protected set; }
        /// <summary>
        /// 是否在运行中
        /// </summary>
        public bool running { get; private set; }
        /// <summary>
        /// 连接状态
        /// </summary>
        public abstract bool connected { get; }

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
            
            OnConnect();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            if (false == connected) return;
            
            OnDisconnect();
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
                engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "未连接网络，请检查网络连接后再尝试." });

                return;
            }
            
            OnSend(msg);
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
        /// 消息包入队
        /// </summary>
        /// <param name="msgType">消息类型</param>
        /// <param name="msg">消息</param>
        protected void EnqueuePackage(Type msgType, INetMessage msg)
        {
            netpackages.Enqueue(new NetPackage { msgType = msgType, msg = msg });
        }

        /// <summary>
        /// 消息通知
        /// </summary>
        private void Notify()
        {
            if (false == running) return;

            while (netpackages.TryDequeue(out var package))
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
        
        /// <summary>
        /// 连接
        /// </summary>
        protected abstract void OnConnect();
        /// <summary>
        /// 断开连接
        /// </summary>
        protected abstract void OnDisconnect();
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <typeparam name="T">消息类型</typeparam>
        /// <param name="msg">消息</param>
        protected abstract void OnSend<T>(T msg) where T : INetMessage;
    }
}
