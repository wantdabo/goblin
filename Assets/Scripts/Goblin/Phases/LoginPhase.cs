using Goblin.Common.FSM;
using Goblin.Sys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goblin.Phases
{
    /// <summary>
    /// 登录阶段
    /// </summary>
    public class LoginPhase : State
    {
        protected override List<Type> passes => new() { typeof(GamingPhase) };

        public override bool OnCheck()
        {
            var hotfixp = engine.phase.GetPhase<HotfixPhase>();

            return false == engine.proxy.login.data.signined && hotfixp.finished;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            engine.gameui.Open<Sys.Login.View.LoginView>();
        }

        public override void OnExit()
        {
            base.OnExit();
            engine.gameui.Close<Sys.Login.View.LoginView>();
        }
    }
}
