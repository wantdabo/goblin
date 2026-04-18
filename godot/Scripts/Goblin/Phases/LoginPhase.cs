using Goblin.Common.FSM;
using System;
using System.Collections.Generic;

namespace Goblin.Phases;

/// <summary>
/// 登录阶段
/// </summary>
public class LoginPhase : State
{
    protected override List<Type> passes => new() { typeof(GamingPhase) };

    public override bool OnValid() => !engine.proxy.login.data.signined;

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