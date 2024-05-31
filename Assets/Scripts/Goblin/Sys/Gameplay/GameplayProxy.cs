using Goblin.Gameplay.Common;
using Goblin.Sys.Common;
using Goblin.Sys.Gameplay.View;
using Goblin.Sys.Lobby;
using Goblin.Sys.Lobby.View;
using Goblin.Sys.Login;
using Goblin.Sys.Other.View;
using Queen.Protocols;
using Queen.Protocols.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Sys.Gameplay
{
    /// <summary>
    /// Gameplay Proxy
    /// </summary>
    public class GameplayProxy : Proxy<GameplayModel>
    {
        public Stage stage { get; private set; }

        protected override void OnCreate()
        {
            base.OnCreate();
            engine.net.Recv<NodeDisconnectMsg>(OnNodeDisconnect);
            engine.net.Recv<S2C_GameInfoMsg>(OnS2CGameInfo);
            engine.proxy.login.eventor.Listen<LogoutEvent>(OnLogout);
            engine.proxy.lobby.eventor.Listen<RoomDestroyedEvent>(OnRoomDestroyed);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            engine.net.UnRecv<NodeDisconnectMsg>(OnNodeDisconnect);
            engine.net.UnRecv<S2C_GameInfoMsg>(OnS2CGameInfo);
            engine.proxy.login.eventor.UnListen<LogoutEvent>(OnLogout);
            engine.proxy.lobby.eventor.UnListen<RoomDestroyedEvent>(OnRoomDestroyed);
        }

        /// <summary>
        /// 销毁对局
        /// </summary>
        private void DestroyStage() 
        {
            if (null == stage) return;

            stage.Destroy();
            RmvComp(stage);
            stage = null;
        }

        private void OnS2CGameInfo(S2C_GameInfoMsg msg)
        {
            if (null != stage) return;

            engine.eventor.Tell(new MessageBlowEvent { type = 1, desc = "进入对局房间." });

            engine.ticker.Timing((t) =>
            {
                engine.gameui.Close<LobbyView>();
                engine.gameui.Close<LobbyRoomView>();
                engine.gameui.Open<GameplayView>();
            }, 0.5f, 1);

            stage = AddComp<Stage>();
            stage.stage = stage;
            stage.Create();
            stage.Analyze(msg);
        }

        private void OnLogout(LogoutEvent e) 
        {
            DestroyStage();
        }

        private void OnRoomDestroyed(RoomDestroyedEvent e) 
        {
            if (e.id != engine.proxy.lobby.data.myRoom) return;
            DestroyStage();
        }

        private void OnNodeDisconnect(NodeDisconnectMsg msg) 
        {
            DestroyStage();
        }
    }
}
