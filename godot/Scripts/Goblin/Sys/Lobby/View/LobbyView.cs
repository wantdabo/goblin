using Goblin.Common;
using Goblin.Gameplay.Logic.Common.BuildDatas;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay;
using Goblin.Sys.Gameplay.View;
using Kowtow.Math;
using System.Collections.Generic;

namespace Goblin.Sys.Lobby.View;

public class LobbyView : UIBaseView
{
    public override UILayer layer => UILayer.UIMain;
    protected override string res => "Lobby/LobbyView";

    protected override void OnBindEvent()
    {
        base.OnBindEvent();

        AddUIEventListener("LogoutBtn", () =>
        {
            engine.proxy.login.C2SLogout();
        });

        AddUIEventListener("LocalGameBtn", () =>
        {
            var players = new List<PlayerData>
            {
                new PlayerData { seat = 1, hero = 200001, position = new IntVector3(0, 0, 0), euler = new IntVector3(0, 0, 0), scale = 1000 },
            };
            var enemies = new List<EnemyData>
            {
                new EnemyData { enemy = 300001, position = new IntVector3(3000, 0, 0), euler = new IntVector3(0, 0, 0), scale = 1000 },
                new EnemyData { enemy = 300001, position = new IntVector3(-3000, 0, 0), euler = new IntVector3(0, 0, 0), scale = 1000 },
                new EnemyData { enemy = 300001, position = new IntVector3(0, 0, 3000), euler = new IntVector3(0, 0, 0), scale = 1000 },
                new EnemyData { enemy = 300001, position = new IntVector3(0, 0, -3000), euler = new IntVector3(0, 0, 0), scale = 1000 },
            };
            var data = new BuildData
            {
                id = 10086,
                seat = 1,
                sdata = new StageData
                {
                    seed = 19491001,
                    players = players.ToArray(),
                    enemies = enemies.ToArray(),
                    sequence = new StageSequenceData
                    {
                        win = StageSequenceCondition.AllEnemiesDead,
                        lose = StageSequenceCondition.HeroDead,
                    },
                },
            };
            engine.gameui.Close(this);
            engine.gameui.Open<GameplayView>();
            engine.gameui.Open<HUDView>();
            engine.gameui.Open<ResultView>();
            engine.proxy.gameplay.CreateGame(data, true);
            engine.proxy.gameplay.StartGame();
        });
    }
}
