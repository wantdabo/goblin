using Goblin.Common;
using Goblin.Core;
using Goblin.Sys.Common;
using Godot;
using System;

namespace Goblin.Sys.Other.View
{
    public class FrameworkView : UIBaseView
    {
        public override UILayer layer => UILayer.UITop;
        protected override string res => "Other/FrameworkView";
        public override bool quickclose => false;

        private RichTextLabel connectStateText;
        private Control connectBtnGo;
        private Control disconnectBtnGo;
        private Control messageContentGo;
        private Control messageOrgGo;

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.eventor.Listen<MessageBlowEvent>(OnMessageBlow);
            engine.ticker.eventor.Listen<TickEvent>(OnTick);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.eventor.UnListen<MessageBlowEvent>(OnMessageBlow);
            engine.ticker.eventor.UnListen<TickEvent>(OnTick);
        }

        protected override void OnBuildUI()
        {
            connectStateText = node.FindChild("ConnectState", true, false) as RichTextLabel;
            if (connectStateText != null) connectStateText.BbcodeEnabled = true;
            connectBtnGo = node.FindChild("ConnectBtn", true, false) as Control;
            disconnectBtnGo = node.FindChild("DisconnectBtn", true, false) as Control;
            messageContentGo = node.FindChild("MessageContent", true, false) as Control;
            messageOrgGo = node.FindChild("MessageORG", true, false) as Control;
        }

        protected override void OnBindEvent()
        {
            AddUIEventListener("ConnectBtn", () =>
            {
#if GODOT_WEB
                engine.net.Connect("127.0.0.1", 12802);
#else
                engine.net.Connect("127.0.0.1", 12801);
#endif
            });
            AddUIEventListener("DisconnectBtn", () => engine.net.Disconnect());
        }

        private void OnMessageBlow(MessageBlowEvent e)
        {
            var msgNode = ObjectPool.Get<Control>("MESSAGE_BLOW_GO_KEY");
            if (null == msgNode) msgNode = messageOrgGo?.Duplicate() as Control;

            messageContentGo?.AddChild(msgNode);
            messageContentGo?.MoveChild(msgNode, -1);

            if (msgNode.FindChild("Desc", true, false) is Label desc)
                desc.Text = $"{DateTime.Now.ToLongTimeString()} : {e.desc}";

            var bg1 = msgNode.FindChild("BG1", true, false) as Control;
            var bg2 = msgNode.FindChild("BG2", true, false) as Control;
            if (bg1 != null) bg1.Visible = false;
            if (bg2 != null) bg2.Visible = false;
            var bg = msgNode.FindChild($"BG{e.type}", true, false) as Control;
            if (bg != null) bg.Visible = true;
            msgNode.Visible = true;

            engine.ticker.Timing((t) =>
            {
                ObjectPool.Set(msgNode, "MESSAGE_BLOW_GO_KEY");
                msgNode.Visible = false;
            }, 3.5f, 1);
        }

        private void OnTick(TickEvent e)
        {
            if (connectBtnGo != null) connectBtnGo.Visible = !engine.net.connected;
            if (disconnectBtnGo != null) disconnectBtnGo.Visible = engine.net.connected;
            if (connectStateText != null)
                connectStateText.Text = engine.net.connected
                    ? "[color=#C3F002]CONNECTED[/color]"
                    : "[color=#D93500]DISCONNECTED[/color]";
        }
    }
}
