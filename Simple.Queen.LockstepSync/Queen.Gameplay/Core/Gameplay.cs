using Queen.Core;
using Queen.Gameplay.Logic;
using Queen.Network;
using Queen.Network.Common;

namespace Queen.Gameplay.Core;

/// <summary>
/// Gameplay 引擎
/// </summary>
public class Gameplay : Engine<Gameplay>
{
    /// <summary>
    /// Gameplay 配置
    /// </summary>
    public Settings settings { get; private set; }
    /// <summary>
    /// 网络
    /// </summary>
    public Slave slave { get; private set; }
    /// <summary>
    /// 认证器
    /// </summary>
    public Authenticator auth { get; private set; }
    /// <summary>
    /// 匹配
    /// </summary>
    public Matching matching { get; private set; }
    /// <summary>
    ///  游戏逻辑
    /// </summary>
    public Gaming gaming { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        settings = AddComp<Settings>();
        settings.Create();
        
        slave = AddComp<Slave>();
        slave.Initialize(settings.host, settings.port, settings.wsport, settings.maxconn, settings.sthread, settings.maxpps);
        slave.Create();

        auth = AddComp<Authenticator>();
        auth.Create();

        matching = AddComp<Matching>();
        matching.Create();
        
        gaming = AddComp<Gaming>();
        gaming.Create();

        engine.logger.Info(
            $"\n\tname: {settings.name}\n\tipaddress: {settings.host}\n\tport: {settings.port}\n\twsport: {settings.wsport}\n\tmaxconn: {settings.maxconn}\n\tgamefps: {settings.gamefps}\n\tgameplayercnt: {settings.gameplayercnt}" 
        , ConsoleColor.Yellow);
        
        engine.logger.Info($"{settings.name} is running...");
        Console.Title = settings.name;
    }
}