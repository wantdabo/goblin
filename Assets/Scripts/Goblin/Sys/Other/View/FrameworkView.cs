﻿using Goblin.Common;
using Goblin.Sys.Common;
using Queen.Network.Protocols.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Goblin.Sys.Other.View
{
    /// <summary>
    /// 消息事件
    /// </summary>
    public struct MessageBlowEvent : IEvent
    {
        /// <summary>
        /// 消息类型，1 绿底，2 红底
        /// </summary>
        public int type;
        /// <summary>
        /// 消息内容
        /// </summary>
        public string desc;
    }

    /// <summary>
    /// 框架面板
    /// </summary>
    public class FrameworkView : UIBaseView
    {
        public override UILayer layer => UILayer.UITop;

        protected override string res => "Other/FrameworkView";

        private Text netDesc;
        private GameObject connectBtn;
        private GameObject disconnectBtn;
        private GameObject messageContent;
        private GameObject messageOrg;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            netDesc = engine.u3dkit.SeekNode<Text>(gameObject, "NetDesc");
            connectBtn = engine.u3dkit.SeekNode<GameObject>(gameObject, "ConnectBtn");
            disconnectBtn = engine.u3dkit.SeekNode<GameObject>(gameObject, "DisconnectBtn");
            messageContent = engine.u3dkit.SeekNode<GameObject>(gameObject, "MessageContent");
            messageOrg = engine.u3dkit.SeekNode<GameObject>(gameObject, "MessageORG");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();

            AddUIEventListener("ConnectBtn", (e) =>
            {
                engine.net.Connect("127.0.0.1", 8080);
            });

            AddUIEventListener("DisconnectBtn", (e) =>
            {
                engine.net.Disconnect();
            });
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.eventor.Listen<MessageBlowEvent>(OnMessageBlow);
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
            engine.net.Recv<NodeConnectMsg>(OnNodeConnect);
            engine.net.Recv<NodeDisconnectMsg>(OnNodeDisconnect);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.eventor.Listen<MessageBlowEvent>(OnMessageBlow);
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
            engine.net.UnRecv<NodeConnectMsg>(OnNodeConnect);
            engine.net.UnRecv<NodeDisconnectMsg>(OnNodeDisconnect);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            connectBtn.SetActive(false == engine.net.connected);
            disconnectBtn.SetActive(engine.net.connected);
            netDesc.text = engine.net.connected ? "<color=#C3F002>CONNECTED</color>" : "<color=#D93500>DISCONNECTED</color>";
        }

        private void OnMessageBlow(MessageBlowEvent e)
        {
            var msgGo = engine.pool.Get<GameObject>("MESSAGE_BLOW_GO_KEY");
            if (null == msgGo)
            {
                msgGo = GameObject.Instantiate(messageOrg, messageContent.transform);
            }

            msgGo.transform.SetAsLastSibling();
            engine.u3dkit.SeekNode<Text>(msgGo, "Desc").text = $"{DateTime.Now.ToLongTimeString()} : {e.desc}";
            engine.u3dkit.SeekNode<GameObject>(msgGo, "BG1").SetActive(false);
            engine.u3dkit.SeekNode<GameObject>(msgGo, "BG2").SetActive(false);
            engine.u3dkit.SeekNode<GameObject>(msgGo, $"BG{e.type}").SetActive(true);
            msgGo.SetActive(true);
            engine.ticker.Timing((t) =>
            {
                engine.pool.Set("MESSAGE_BLOW_GO_KEY", msgGo);
                msgGo.SetActive(false);
            }, 3.5f, 1);
        }

        private void OnTick(TickEvent e)
        {
            var str = engine.net.connected ? $"PING : {engine.net.ping} MS" : "";
            engine.u3dkit.SeekNode<Text>(gameObject, "Ping").text = str;
        }

        private void OnNodeConnect(NodeConnectMsg msg)
        {
            UpdateInfo();
        }

        private void OnNodeDisconnect(NodeDisconnectMsg msg)
        {
            UpdateInfo();
        }
    }
}