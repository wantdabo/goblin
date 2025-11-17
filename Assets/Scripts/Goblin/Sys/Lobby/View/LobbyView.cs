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
using Goblin.Gameplay.Logic.Common.BuildDatas;
using Kowtow.Math;
using Goblin.Sys.Gameplay;
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

            AddUIEventListener("LocalGameBtn", (e) =>
            {
                List<PlayerData> players = new()
                {
                    new PlayerData
                    {
                        seat = 1,
                        hero = 100001,
                        position = new IntVector3(0, 0, 0),
                        euler = new IntVector3(0, 0, 0),
                        scale = 1000,
                    },
                };
                ulong seat = 2;
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        players.Add(new PlayerData
                        {
                            seat = seat,
                            hero = 100001,
                            position = new IntVector3(1000 * i, 0, 1000 * j),
                            euler = new IntVector3(0, (i + 1) * 20000 + (j + 1) * 20000, 0),
                            scale = 1000,
                        });
                        seat++;
                    }
                }
                
                BuildData data = new BuildData();
                data.id = 10086;
                data.seat = 1;
                data.sdata = new StageData
                {
                    seed = 19491001,
                    players = players.ToArray(),
                };
                
                engine.gameui.Close(this);
                engine.gameui.Open<GameplayView>();
                engine.proxy.gameplay.Load<LocalDirector>(data, true);
                engine.proxy.gameplay.director.StartGame();
            });
        }
    }
}
