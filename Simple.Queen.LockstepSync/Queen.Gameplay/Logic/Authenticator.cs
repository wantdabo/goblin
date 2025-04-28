
using Queen.Gameplay.Core;
using Queen.Network.Common;
using Queen.Protocols;

namespace Queen.Gameplay.Logic;

/// <summary>
/// 认证
/// </summary>
public class Authenticator : Comp
{
    public Dictionary<string, NetChannel> users { get; private set; } = new();
    public Dictionary<NetChannel, string> channels { get; private set; } = new();
    
    protected override void OnCreate()
    {
        base.OnCreate();
        engine.slave.Recv<C2SLoginMsg>(OnC2SLogin);
    }

    private void OnC2SLogin(NetChannel channel, C2SLoginMsg msg)
    {
        if (users.ContainsKey(msg.username))
        {
            users.Remove(msg.username);
            channels.Remove(channel);
        }
        
        channel.Send(new S2CLoginMsg
        {
            code = 1,
            uuid = msg.username,
        });
        
        users.Add(msg.username, channel);
        channels.Add(channel, msg.username);
        
        // 尝试重连
        engine.gaming.Reconnect(msg.username);
    }
}