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
using Goblin.Gameplay.Director;
using Goblin.Gameplay.Logic.Common.GPDatas;
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
                GPData data = new GPData();
                data.id = 10086;
                data.skey = 123456;
                data.seat = 1;
                data.sdata = new GPStageData
                {
                    seed = 19491001,
                    players = new[]
                    {
                        new GPPlayerData
                        {
                            seat = 1,
                            hero = 100001,
                            position = new GPVector3(0, 0, 0),
                            euler = new GPVector3(0, 0, 0),
                            scale = new GPVector3(1000, 1000, 1000),
                        },
                        new GPPlayerData
                        {
                            seat = 2,
                            hero = 100001,
                            position = new GPVector3(0, 0, 0),
                            euler = new GPVector3(0, 0, 0),
                            scale = new GPVector3(1000, 1000, 1000),
                        },
                    }
                };
                
                engine.gameui.Close(this);
                engine.gameui.Open<GameplayView>();
                engine.proxy.gameplay.Load<LocalDirector>(data);
                engine.proxy.gameplay.director.StartGame();
            });
        }
    }
}
