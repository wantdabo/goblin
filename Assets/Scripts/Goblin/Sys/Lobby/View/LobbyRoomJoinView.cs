using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Goblin.Sys.Lobby.View
{
    /// <summary>
    /// 加入房间
    /// </summary>
    public class LobbyRoomJoinView : UIBaseView
    {
        public override UILayer layer => UILayer.UIAlert;

        protected override string res => "Lobby/LobbyRoomJoinView";

        /// <summary>
        /// 房间 ID
        /// </summary>
        public uint id;

        private Text roomNameText;
        private InputField roomPasswordInput;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            roomNameText = engine.u3dkit.SeekNode<Text>(gameObject, "RoomName");
            roomPasswordInput = engine.u3dkit.SeekNode<InputField>(gameObject, "RoomPassword");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            AddUIEventListener("CancelBtn", (e) =>
            {
                engine.gameui.Close<LobbyRoomJoinView>();
            });

            AddUIEventListener("JoinBtn", (e) =>
            {
                uint password = string.IsNullOrEmpty(roomPasswordInput.text) ? 0 : uint.Parse(roomPasswordInput.text);
                engine.proxy.lobby.C2SJoinRoom(id, password);
            });
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            UpdateInfo();
        }

        private void UpdateInfo() 
        {
            var room = engine.proxy.lobby.GetRoom(id);
            if (null == room) return;

            roomNameText.text = room.name;
            roomPasswordInput.gameObject.SetActive(room.needpwd);
        }
    }
}
