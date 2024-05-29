using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Goblin.Sys.Lobby.View
{
    /// <summary>
    /// 房间界面
    /// </summary>
    public class LobbyRoomView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;

        protected override string res => "Lobby/LobbyRoomView";

        private Text roomNameText;
        private Text roomMemberCountText;
        private GameObject memberContentGo;

        private List<LobbyRoomMember> roomMembers = new();

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
            roomNameText = engine.u3dkit.SeekNode<Text>(gameObject, "RoomName");
            roomMemberCountText = engine.u3dkit.SeekNode<Text>(gameObject, "RoomMemberCount");
            memberContentGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "MemberContent");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            AddUIEventListener("PlayBtn", (e) =>
            {
                engine.proxy.lobby.C2SRoom2Game();
            });

            AddUIEventListener("BackBtn", (e) =>
            {
                engine.gameui.Close<LobbyRoomView>();
            });

            AddUIEventListener("ExitRoomBtn", (e) =>
            {
                engine.proxy.lobby.C2SExitRoom();
            });
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            var room = engine.proxy.lobby.GetRoom(engine.proxy.lobby.data.myRoom);
            if (null == room) return;
            roomNameText.text = room.name;
            roomMemberCountText.text = $"房间人数 : {room.members.Length}/{room.mlimit}";

            foreach (var roomMember in roomMembers) roomMember.SetActive(false);
            for (int i = 0; i < room.members.Length; i++)
            {
                if (i + 1 > roomMembers.Count) roomMembers.Add(AddUICell<LobbyRoomMember>(memberContentGo));
                var m = roomMembers[i];
                m.SetMemberInfo(room.owner, room.members[i]);
                m.SetActive(true);
            }
        }

        private void OnRoomChanged(RoomChangedEvent e) 
        {
            UpdateInfo();
        }
    }
}
