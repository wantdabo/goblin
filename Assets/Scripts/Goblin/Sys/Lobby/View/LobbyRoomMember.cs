using Goblin.Sys.Common;
using Queen.Protocols;
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
    /// 房间成员
    /// </summary>
    public class LobbyRoomMember : UIBaseCell
    {
        protected override string res => "Lobby/LobbyRoomMember";

        private Text idenityText;
        private Text nicknameText;
        private GameObject kickBtnGo;

        private string pid;

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            idenityText = engine.u3dkit.SeekNode<Text>(gameObject, "Idenity");
            nicknameText = engine.u3dkit.SeekNode<Text>(gameObject, "Nickname");
            kickBtnGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "KickBtn");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();
            AddUIEventListener("KickBtn", (e) =>
            {
                engine.proxy.lobby.C2SKickRoom(pid);
            });
        }

        public void SetMemberInfo(string owner, RoomMemberInfo member) 
        {
            pid = member.pid;
            var identity = member.pid.Equals(owner) ? "OWNER" : "MEMBER";
            identity += member.pid.Equals(engine.proxy.login.data.pid) ? ".SELF" : ".OTHER";
            idenityText.text = identity;
            nicknameText.text = member.nickname;

            kickBtnGo.SetActive(engine.proxy.lobby.data.ownerRoom && false == member.pid.Equals(engine.proxy.login.data.pid));
        }
    }
}
