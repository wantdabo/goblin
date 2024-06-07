using Goblin.Common;
using Goblin.Sys.Common;
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

        private Text connectStateText;
        private GameObject connectBtnGo;
        private GameObject disconnectBtnGo;
        private GameObject messageContentGo;
        private GameObject messageOrgGo;

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.eventor.Listen<MessageBlowEvent>(OnMessageBlow);
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.eventor.Listen<MessageBlowEvent>(OnMessageBlow);
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            connectStateText = engine.u3dkit.SeekNode<Text>(gameObject, "ConnectState");
            connectBtnGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "ConnectBtn");
            disconnectBtnGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "DisconnectBtn");
            messageContentGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "MessageContent");
            messageOrgGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "MessageORG");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();

            AddUIEventListener("ConnectBtn", (e) =>
            {
                engine.net.Connect("192.168.2.156", 12801);
            });

            AddUIEventListener("DisconnectBtn", (e) =>
            {
                engine.net.Disconnect();
            });
        }

        private void OnMessageBlow(MessageBlowEvent e)
        {
            var msgGo = engine.pool.Get<GameObject>("MESSAGE_BLOW_GO_KEY");
            if (null == msgGo)
            {
                msgGo = GameObject.Instantiate(messageOrgGo, messageContentGo.transform);
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
            connectBtnGo.SetActive(false == engine.net.connected);
            disconnectBtnGo.SetActive(engine.net.connected);
            connectStateText.text = engine.net.connected ? "<color=#C3F002>CONNECTED</color>" : "<color=#D93500>DISCONNECTED</color>";

            var ping = engine.net.connected ? $"PING : {engine.net.ping} MS" : "";
            engine.u3dkit.SeekNode<Text>(gameObject, "Ping").text = ping;
            var bytessr = engine.net.connected ? $"UPLOAD : {engine.net.bytesSentPerSeconds / 1024f} KB /S \tDOWNLOAD : {engine.net.bytesRecvPerSeconds / 1024f} KB /S" : "";
            engine.u3dkit.SeekNode<Text>(gameObject, "BytesSR").text = bytessr;
        }
    }
}
