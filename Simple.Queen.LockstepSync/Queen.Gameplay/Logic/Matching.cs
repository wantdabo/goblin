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
    private ConcurrentDictionary<NetChannel, int> matchings { get; set; } = new();

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
        if (matchings.Count < 2) return;

        var channels = matchings.Keys.ToList();
        foreach (var channel in channels)
        {
            if (false == channel.alive) matchings.TryRemove(channel, out _);
        }

        var matcheds = matchings.Take(2).ToList();

        Dictionary<PlayerData, NetChannel> pnc = new();
        List<PlayerData> players = new();
        ulong index = 1;
        foreach (var match in matcheds)
        {
            if (false == match.Key.alive) continue;
            matchings.TryRemove(match.Key, out _);
            PlayerData player = new PlayerData();
            player.seat = index;
            player.hero = match.Value;
            player.position = new Vector3() { x = Random.Shared.Next(0, 10) * 1000, y = 0, z = Random.Shared.Next(0, 10) * 1000 };
            player.euler = new Vector3();
            player.scale = new Vector3() { x = 1000, y = 1000, z = 1000 };
            players.Add(player);
            index++;
        }

        GameData data = new GameData();
        data.id = (ulong)Random.Shared.Next(1010, 100000000);
        data.skey = 10000;
        data.sdata = new StageData();
        data.sdata.seed = 19481001;
        data.sdata.players = players.ToArray();
        
        foreach (var player in players)
        {
            if (false == pnc.TryGetValue(player, out var channel)) continue;
            if (false == channel.alive) continue;
            data.seat = player.seat;
            channel.Send(new S2CStartGameMsg
            {
                gamehost = engine.settings.gamehost,
                gameport = engine.settings.gameport,
                data = data,
            });
        }
        
        // 清理匹配列表
        foreach (var channel in matcheds)
        {
            if (false == channel.Key.alive) continue;
            matchings.TryRemove(channel.Key, out _);
        }
    }

    private void OnC2SStartMatching(NetChannel channel, C2SStartMatchingMsg msg)
    {
        if (matchings.ContainsKey(channel)) return;
        matchings.TryAdd(channel, msg.hero);
        
        channel.Send(new S2CStartMatchingMsg
        {
        });
    }

    private void OnC2SEndMatching(NetChannel channel, C2SEndMatchingMsg msg)
    {
        if (false == matchings.ContainsKey(channel)) return;
        matchings.TryRemove(channel, out _);
        
        channel.Send(new S2CEndMatchingMsg
        {
        });
    }
}