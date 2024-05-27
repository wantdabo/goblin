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
    /// 创建房间
    /// </summary>
    public class LobbyRoomNewView : UIBaseView
    {
        public override UILayer layer => UILayer.UIAlert;

        protected override string res => "Lobby/LobbyRoomNewView";

        private InputField roomName;
        private Dropdown roomMemberLimit;
        private Toggle privateRoom;
        private InputField roomPassword;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            roomName = engine.u3dkit.SeekNode<InputField>(gameObject, "RoomName");
            roomMemberLimit = engine.u3dkit.SeekNode<Dropdown>(gameObject, "RoomMemberLimit");
            privateRoom = engine.u3dkit.SeekNode<Toggle>(gameObject, "PrivateRoom");
            roomPassword = engine.u3dkit.SeekNode<InputField>(gameObject, "RoomPassword");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            AddUIEventListener("CancelBtn", (e) =>
            {
                engine.gameui.Close<LobbyRoomNewView>();
            });

            AddUIEventListener("CreateBtn", (e) =>
            {
                var roomMemberLimitMap = new int[] { 2, 4, 8 };
                uint password = string.IsNullOrEmpty(roomPassword.text) ? 0 : uint.Parse(roomPassword.text);
                engine.proxy.lobby.C2SCreateRoom(roomName.text, privateRoom.isOn, password, roomMemberLimitMap[roomMemberLimit.value]);
            });

            privateRoom.onValueChanged.AddListener((t) =>
            {
                roomPassword.text = "";
                UpdateInfo();
            });
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            roomPassword.gameObject.SetActive(privateRoom.isOn);
        }
    }
}
