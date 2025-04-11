using Goblin.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
using Goblin.Sys.Other.View;
using Queen.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goblin.Gameplay.Directors.Local.Common;
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

        protected override void OnBindEvent()
        {
            base.OnBindEvent();

            AddUIEventListener("LogoutBtn", (e) =>
            {
                engine.proxy.login.C2SLogout();
            });

            AddUIEventListener("LocalGame", (e) =>
            {
                engine.gameui.Close(this);
                engine.gameui.Open<GameplayView>();
                engine.proxy.gameplay.Load<LocalDirector>();
                engine.proxy.gameplay.director.StartGame();
            });
        }
    }
}
