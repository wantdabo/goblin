﻿using Goblin.Common;
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
        }
    }
}