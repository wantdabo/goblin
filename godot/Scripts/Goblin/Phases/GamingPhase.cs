using Goblin.Common.FSM;
using System;
using System.Collections.Generic;

namespace Goblin.Phases
{
    /// <summary>
    /// 游戏阶段
    /// </summary>
    public class GamingPhase : State
    {
        protected override List<Type> passes => new() { typeof(LoginPhase) };

        public override bool OnValid() => engine.proxy.login.data.signined;

        public override void OnEnter()
        {
            base.OnEnter();
            engine.gameui.Open<Sys.Lobby.View.LobbyView>();
        }

        public override void OnExit()
        {
            base.OnExit();
            engine.gameui.Close<Sys.Lobby.View.LobbyView>();
        }
    }
}
