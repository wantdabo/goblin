using System.Collections.Concurrent;
using Queen.Gameplay.Core;
using Queen.Network.Common;
using Queen.Protocols;
using TouchSocket.Core;

namespace Queen.Gameplay.Logic;

/// <summary>
/// 游戏逻辑
/// </summary>
public class Gaming : Comp
{
    /// <summary>
    /// Username, GameID
    /// </summary>
    public ConcurrentDictionary<string, ulong> usergames { get; private set; } = new();
    /// <summary>
    /// Username, Seat
    /// </summary>
    public ConcurrentDictionary<string, ulong> userseats { get; private set; } = new();
    /// <summary>
    /// GameID, GameData
    /// </summary>
    public ConcurrentDictionary<ulong, GameData> gamedatas { get; private set; } = new();
    /// <summary>
    /// GameID, Game
    /// </summary>
    public ConcurrentDictionary<ulong, Game> games { get; private set; } = new();

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
        gamedatas.TryAdd(data.id, data);
        foreach (var player in data.sdata.players)
        {
            usergames.TryAdd(player.username, data.id);
            userseats.TryAdd(player.username, player.seat);
        }
        
        var game = AddComp<Game>();
        game.Initialize(data.id).Create();
        games.TryAdd(data.id, game);
        
        return data;
    }

    private void OnC2SPlayerInput(NetChannel channel, C2SPlayerInputMsg msg)
    {
        if (false == engine.auth.channels.TryGetValue(channel, out var username)) return;
        if (false == usergames.TryGetValue(username, out var id)) return;
        if (msg.id != id) return;
        if (false == games.TryGetValue(msg.id, out var game)) return;
        foreach (var input in msg.inputs) game.SetInput(input);
    }
}

public class Game : Comp
{
    /// <summary>
    /// is destroyed
    /// </summary>
    public bool disposed { get;  private set; }
    /// <summary>
    /// GameID
    /// </summary>
    public ulong id { get; private set; }
    /// <summary>
    /// Game Current Frame
    /// </summary>
    public uint frame { get; private set; }
    /// <summary>
    /// Game Frame, FrameData
    /// </summary>
    public ConcurrentDictionary<uint, List<PlayerInputData>> frames { get; set; } = new();
    /// <summary>
    /// CurrentFrame PlayerInputDatas
    /// </summary>
    private ConcurrentList<PlayerInputData> curinputs { get; set; } = new();
    
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
                List<PlayerInputData> inputs = new();
                foreach (var input in curinputs)
                {
                    inputs.Add(new PlayerInputData
                    {
                        seat = input.seat,
                        type = input.type,
                        press = input.press,
                        dire = new Vector2
                        {
                            x = input.dire.x,
                            y = input.dire.y,
                        },
                    });
                }
                frames.TryAdd(frame, inputs);
                curinputs.Clear();

                if (false == engine.gaming.gamedatas.TryGetValue(id, out var data)) continue;
                S2CGameFrameMsg msg = new S2CGameFrameMsg
                {
                    frame = new FrameData
                    {
                        frame = frame,
                        inputs = inputs.ToArray(),
                    },
                };
                
                foreach (var player in data.sdata.players)
                {
                    if (false == engine.auth.users.TryGetValue(player.username, out var channel)) continue;
                    if (false == channel.alive) continue;
                    channel.Send(msg);
                }
            }
        });
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        disposed = true;
    }

    public Game Initialize(ulong id)
    {
        this.id = id;

        return this;
    }

    public void SetInput(PlayerInputData input)
    {
        PlayerInputData oldinput = default;
        foreach (var i in curinputs)
        {
            if (i.seat == input.seat && i.type == input.type)
            {
                oldinput = i;
                break;
            }
        }
        if (null != oldinput) curinputs.Remove(oldinput);
        
        curinputs.Add(input);
    }
}