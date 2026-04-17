using Queen.Protocols.Common;
using System;
using Godot;

namespace Goblin.Common.Network
{
    public class NetWebSocket : NetNode
    {
        private WebSocketPeer socket { get; set; }
        public override bool connected => null != socket && socket.GetReadyState() == WebSocketPeer.State.Open;

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.ticker.eventor.Listen<TickEvent>(OnTickPoll);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.ticker.eventor.UnListen<TickEvent>(OnTickPoll);
        }

        protected override void OnConnect()
        {
            socket = new WebSocketPeer();
            socket.ConnectToUrl($"ws://{ip}:{port}/ws");
        }

        protected override void OnDisconnect()
        {
            socket?.Close();
        }

        protected override void OnSend<T>(T msg)
        {
            if (ProtoPack.Pack(msg, out var bytes))
                socket.PutPacket(bytes);
        }

        private WebSocketPeer.State lastState = WebSocketPeer.State.Closed;

        private void OnTickPoll(TickEvent e)
        {
            if (null == socket) return;
            socket.Poll();

            var state = socket.GetReadyState();
            if (state != lastState)
            {
                if (state == WebSocketPeer.State.Open)
                    EnqueuePackage(typeof(NodeConnectMsg), new NodeConnectMsg());
                else if (lastState == WebSocketPeer.State.Open)
                    EnqueuePackage(typeof(NodeDisconnectMsg), new NodeDisconnectMsg());
                lastState = state;
            }

            while (socket.GetAvailablePacketCount() > 0)
            {
                var data = socket.GetPacket();
                if (ProtoPack.UnPack(data, out var msgType, out var msg))
                    EnqueuePackage(msgType, msg);
            }
        }
    }
}
