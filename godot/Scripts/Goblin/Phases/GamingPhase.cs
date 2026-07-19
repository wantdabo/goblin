using Goblin.Common.FSM;
using System;
using System.Collections.Generic;

namespace Goblin.Phases;

/// <summary>
/// 游戏阶段
/// </summary>
public class GamingPhase : State
{
    protected override List<Type> passes => new() { typeof(LoginPhase) };

    public override bool OnValid() => true; // 无服务器，跳过登录，与 Unity 一致

    public override void OnEnter()
    {
        base.OnEnter();
        engine.gameui.Open<Sys.Lobby.View.LobbyView>();
        engine.sound.PlayBGM(2000001);
    }

    public override void OnExit()
    {
        base.OnExit();
        engine.sound.StopBGM();
        engine.gameui.Close<Sys.Lobby.View.LobbyView>();
    }
}