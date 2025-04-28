using Queen.Gameplay.Core;
using Queen.Network.Common;
using Queen.Protocols;

namespace Queen.Gameplay.Logic;

/// <summary>
/// 游戏逻辑
/// </summary>
public class Gaming : Comp
{
    public Dictionary<string, ulong> usergames { get; private set; } = new();
    public Dictionary<ulong, GameData> gamedatas { get; private set; } = new();
    public Dictionary<ulong, Game> games { get; private set; } = new();
    public Dictionary<ulong, Dictionary<ulong, ulong>> gameseats { get; private set; } = new();

    protected override void OnCreate()
    {
        base.OnCreate();
        engine.slave.Recv<C2SPlayerInputMsg>(OnC2SPlayerInput);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        engine.slave.UnRecv<C2SPlayerInputMsg>(OnC2SPlayerInput);
    }

    private GameData CreateGameData(List<(string username, int hero)> users)
    {
        ulong index = 1;
        List<PlayerData> players = new();
        foreach (var user in users)
        {
            PlayerData player = new PlayerData
            {
                seat = index,
                username = user.username,
                hero = user.hero,
                position = new Vector3 { x = Random.Shared.Next(0, 10) * 1000, y = 0, z = Random.Shared.Next(0, 10) * 1000 },
                euler = new Vector3(),
                scale = new Vector3 { x = 1000, y = 1000, z = 1000 }
            };
            players.Add(player);
            index++;
        }

        GameData data = new GameData
        {
            id = (ulong)Random.Shared.Next(1010, 100000000),
            sdata = new StageData
            {
                seed = 19491001,
                players = players.ToArray()
            }
        };

        return data;
    }

    public GameData CreateGame(List<(string username, int hero)> users)
    {
        var data = CreateGameData(users);
        gamedatas.Add(data.id, data);
        foreach (var player in data.sdata.players)
        {
            usergames.Add(player.username, data.id);
        }
        
        var game = AddComp<Game>();
        games.Add(data.id, game);
        game.Create();

        return data;
    }

    public void Reconnect(string username)
    {
        if (false == usergames.TryGetValue(username, out var id) || false == games.TryGetValue(id, out var game)) return;
    }

    private void OnC2SPlayerInput(NetChannel channel, C2SPlayerInputMsg msg)
    {
    }
}

public class Game : Comp
{
    public bool disposed { get;  private set; }
    public uint frame { get; private set; }
    public Dictionary<uint, List<PlayerInputData>> frames { get; set; } = new();
    
    protected override void OnCreate()
    {
        base.OnCreate();
        Task.Run(() =>
        {
            while (true)
            {
                if (disposed) break;
                Thread.Sleep((int)(1f / engine.settings.gamefps * 1000));
                frame++;
            }
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        disposed = true;
    }
}