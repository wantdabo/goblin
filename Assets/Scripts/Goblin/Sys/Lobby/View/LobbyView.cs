using Goblin.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Other.View;
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
    /// 大厅界面
    /// </summary>
    public class LobbyView : UIBaseView
    {
        public override UILayer layer => UILayer.UIMain;

        protected override string res => "Lobby/LobbyView";

        private GameObject emptyRoomTipsGo;
        private GameObject roomContentGo;

        private List<LobbyRoomCard> roomCards = new();

        protected override void OnLoad()
        {
            base.OnLoad();
            engine.proxy.lobby.eventor.Listen<RoomsChangedEvent>(OnRoomsChanged);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            engine.proxy.lobby.eventor.UnListen<RoomsChangedEvent>(OnRoomsChanged);
        }

        protected override void OnBuildUI()
        {
            base.OnBuildUI();
            emptyRoomTipsGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "EmptyRoomTips");
            roomContentGo = engine.u3dkit.SeekNode<GameObject>(gameObject, "RoomContent");
        }

        protected override void OnBindEvent()
        {
            base.OnBindEvent();

            AddUIEventListener("MyRoomBtn", (e) =>
            {
                if (false == engine.proxy.lobby.data.hasRoom)
                {
                    engine.eventor.Tell(new MessageBlowEvent { type = 2, desc = "未进入任何房间." });
                    return;
                }

                engine.gameui.Open<LobbyRoomView>();
            });

            AddUIEventListener("CreateRoomBtn", (e) =>
            {
                engine.gameui.Open<LobbyRoomNewView>();
            });
            AddUIEventListener("RefreshRoomBtn", (e) =>
            {
                engine.proxy.lobby.C2SPullRooms();
            });

            AddUIEventListener("LogoutBtn", (e) =>
            {
                engine.proxy.login.C2SLogout();
            });
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            UpateInfo();
        }

        private void UpateInfo()
        {
            foreach (var roomCard in roomCards)
            {
                roomCard.SetRoom(0);
                roomCard.SetActive(false);
            }

            bool empty = 0 >= engine.proxy.lobby.data.rooms.Count;
            emptyRoomTipsGo.SetActive(empty);
            if (empty) return;

            for (int i = 0; i < engine.proxy.lobby.data.rooms.Count; i++)
            {
                if (i + 1 > roomCards.Count) roomCards.Add(AddUICell<LobbyRoomCard>(roomContentGo));
                var room = engine.proxy.lobby.data.rooms[i];
                var roomCard = roomCards[i];
                roomCard.SetRoom(room.id);
                roomCard.SetActive(true);
            }
        }

        private void OnRoomsChanged(RoomsChangedEvent e)
        {
            UpateInfo();
        }
    }
}
