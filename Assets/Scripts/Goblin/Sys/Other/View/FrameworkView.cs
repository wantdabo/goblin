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

        public override bool quickclose => false;

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
#if UNITY_WEBGL
                engine.net.Connect("127.0.0.1", 12802);
#else
                engine.net.Connect("127.0.0.1", 12801);
#endif
            });

            AddUIEventListener("DisconnectBtn", (e) =>
            {
                engine.net.Disconnect();
            });
        }

        private void OnMessageBlow(MessageBlowEvent e)
        {
            var msgGo = ObjectPool.Get<GameObject>("MESSAGE_BLOW_GO_KEY");
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
                ObjectPool.Set(msgGo, "MESSAGE_BLOW_GO_KEY");
                msgGo.SetActive(false);
            }, 3.5f, 1);
        }

        private void OnTick(TickEvent e)
        {
            connectBtnGo.SetActive(false == engine.net.connected);
            disconnectBtnGo.SetActive(engine.net.connected);
            connectStateText.text = engine.net.connected ? "<color=#C3F002>CONNECTED</color>" : "<color=#D93500>DISCONNECTED</color>";
        }
    }
}
