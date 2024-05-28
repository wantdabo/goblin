using Goblin.Sys.Common;
using Goblin.Sys.Other.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Goblin.Sys.Lobby.View
{
    /// <summary>
    /// 房间列表子项
    /// </summary>
    public class LobbyRoomCard : UIBaseCell
    {
        protected override string res => "Lobby/LobbyRoomCard";

        private uint id;

        private Image bgImg;
        private Text idenityText;
        private Text nameText;
        private Text memberText;
        private Text stateText;

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.proxy.lobby.eventor.Listen<RoomChangedEvent>(OnRoomChanged);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.proxy.lobby.eventor.UnListen<RoomChangedEvent>(OnRoomChanged);
        }

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            bgImg = engine.u3dkit.SeekNode<Image>(gameObject, "BG");
            idenityText = engine.u3dkit.SeekNode<Text>(gameObject, "Idenity");
            nameText = engine.u3dkit.SeekNode<Text>(gameObject, "Name");
            memberText = engine.u3dkit.SeekNode<Text>(gameObject, "Member");
            stateText = engine.u3dkit.SeekNode<Text>(gameObject, "State");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            AddUIEventListener("BG", async (e) =>
            {
                if (id == engine.proxy.lobby.data.myRoom)
                {
                    engine.gameui.Open<LobbyRoomView>();

                    return;
                }

                var view = await engine.gameui.Load<LobbyRoomJoinView>();
                view.id = id;
                view.Open();
            });
        }

        private void UpateInfo()
        {
            var room = engine.proxy.lobby.GetRoom(id);
            if (null == room) return;

            bgImg.color = id == engine.proxy.lobby.data.myRoom ? new UnityEngine.Color(195f / 255f, 240f / 255f, 2f / 255f, 255f / 255f) : new UnityEngine.Color(195f / 255f, 240f / 255f, 2f / 255f, 150f / 255f);
            var idenity = room.needpwd ? "PRIVATE" : "PUBLIC";
            idenity += id == engine.proxy.lobby.data.myRoom ? ".SELF" : ".OTHER";
            idenityText.text = idenity;
            nameText.text = room.name;
            memberText.text = $"{room.members.Length}/{room.mlimit}";
            stateText.text = RoomState.WAITING == room.state ? "等待中" : "游戏中";
        }

        public void SetRoom(uint id)
        {
            this.id = id;
            UpateInfo();
        }

        private void OnRoomChanged(RoomChangedEvent e)
        {
            if (e.id != id) return;
            UpateInfo();
        }
    }
}
