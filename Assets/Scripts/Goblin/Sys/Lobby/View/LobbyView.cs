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
using Random = UnityEngine.Random;

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
                        position = new IntVector3(21000, 0, 21000),
                        euler = new IntVector3(0, 0, 0),
                        scale = 1000,
                    },
                    new PlayerData
                    {
                        seat = 2,
                        hero = 100001,
                        position = new IntVector3(20000, 0, 20000),
                        euler = new IntVector3(0, 0, 0),
                        scale = 1000,
                    },
                };
                ulong seat = 3;
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        players.Add(new PlayerData
                        {
                            seat = seat,
                            hero = 100001,
                            position = new IntVector3(1100 * i, 0, 1100 * j),
                            euler = new IntVector3(0, 0, 0),
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
