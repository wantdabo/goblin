using Goblin.Common.FSM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Phases
{
    /// <summary>
    /// 游戏阶段
    /// </summary>
    public class GamingPhase : State
    {
        protected override List<Type> passes => new() { typeof(LoginPhase) };

        public override bool OnCheck()
        {
            var hotfixp = engine.phase.GetPhase<HotfixPhase>();

            return engine.proxy.login.data.signined && hotfixp.finished;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            engine.gameui.Open<Sys.Lobby.View.LobbyView>();
            engine.gameui.Open<Sys.Gameplay.View.GameplayView>();
        }

        public override void OnExit()
        {
            base.OnExit();
            engine.gameui.Close<Sys.Lobby.View.LobbyView>();
            engine.gameui.Close<Sys.Gameplay.View.GameplayView>();
        }
    }
}
