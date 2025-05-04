using System.Collections.Concurrent;
using Queen.Core;
using Queen.Network.Common;
using Queen.Protocols;
using Comp = Queen.Gameplay.Core.Comp;

namespace Queen.Gameplay.Logic;

/// <summary>
/// 匹配
/// </summary>
public class Matching : Comp
{
    private ConcurrentDictionary<string, int> matchings { get; set; } = new();

    protected override void OnCreate()
    {
        base.OnCreate();
        engine.eventor.Listen<ExecuteEvent>(OnExecute);
        engine.slave.Recv<C2SStartMatchingMsg>(OnC2SStartMatching);
        engine.slave.Recv<C2SEndMatchingMsg>(OnC2SEndMatching);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        engine.eventor.UnListen<ExecuteEvent>(OnExecute);
        engine.slave.UnRecv<C2SStartMatchingMsg>(OnC2SStartMatching);
        engine.slave.UnRecv<C2SEndMatchingMsg>(OnC2SEndMatching);
    }

    private void OnExecute(ExecuteEvent e)
    {
        if (matchings.Count < engine.settings.gameplayercnt) return;

        var usermatchings = matchings.Take(2).ToList();
        List<(string username, int hero)> users = new();
        foreach (var usermatching in usermatchings) users.Add((usermatching.Key, usermatching.Value));
        var gamedata = engine.gaming.CreateGame(users);
        
        foreach (var player in gamedata.sdata.players)
        {
            matchings.TryRemove(player.username, out _);

            if (false == engine.auth.users.TryGetValue(player.username, out var channel)) continue;
            if (false == channel.alive) continue;
            channel.Send(new S2CStartGameMsg
            {
                seat = player.seat,
                data = gamedata,
            });
        }
    }
    
    public void Reconnect(string username)
    {
        if (false == engine.gaming.usergames.TryGetValue(username, out var id)) return;
        if (false == engine.gaming.userseats.TryGetValue(username, out var seat)) return;
        if (false == engine.gaming.games.TryGetValue(id, out var game)) return;
        if (false == engine.gaming.gamedatas.TryGetValue(id, out var gamedata)) return;
        
        if (false == engine.auth.users.TryGetValue(username, out var channel)) return;
        if (false == channel.alive) return;
        channel.Send(new S2CStartGameMsg
        {
            seat = seat,
            data = gamedata,
        });
        
        List<FrameData> framedatas = new();
        foreach (var kv in game.frames)
        {
            framedatas.Add(new FrameData
            {
                frame = kv.Key,
                inputs = kv.Value.ToArray(),
            });
        }
        framedatas.Sort((a, b) =>
        {
            return a.frame.CompareTo(b.frame);
        });
        
        channel.Send(new S2CGameFrameMsg
        {
            frames = framedatas.ToArray(),
        });
    }

    private void OnC2SStartMatching(NetChannel channel, C2SStartMatchingMsg msg)
    {
        if (false == engine.auth.channels.TryGetValue(channel, out var username)) return;
        if (engine.gaming.usergames.ContainsKey(username)) return;
        if (matchings.ContainsKey(username)) return;
        matchings.TryAdd(username, msg.hero);

        channel.Send(new S2CStartMatchingMsg
        {
        });
    }

    private void OnC2SEndMatching(NetChannel channel, C2SEndMatchingMsg msg)
    {
        if (false == engine.auth.channels.TryGetValue(channel, out var username)) return;
        if (engine.gaming.usergames.ContainsKey(username)) return;
        if (false == matchings.ContainsKey(username)) return;
        matchings.TryRemove(username, out _);

        channel.Send(new S2CEndMatchingMsg
        {
        });
    }
}