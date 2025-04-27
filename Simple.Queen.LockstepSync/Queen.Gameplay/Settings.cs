using Newtonsoft.Json.Linq;
using Queen.Gameplay.Core;

namespace Queen.Gameplay;

public class Settings : Comp
{
    /// <summary>
    /// 名字
    /// </summary>
    public string name { get; private set; }
    /// <summary>
    /// 主机
    /// </summary>
    public string host { get; private set; }
    /// <summary>
    /// 端口
    /// </summary>
    public ushort port { get; private set; }
    /// <summary>
    /// WS 端口
    /// </summary>
    public ushort wsport { get; private set; }
    /// <summary>
    /// 最大连接数
    /// </summary>
    public int maxconn { get; private set; }
    /// <summary>
    /// Slave（主网）最大工作线程
    /// </summary>
    public int sthread { get; private set; }
    /// <summary>
    /// 最大网络收发包每秒
    /// </summary>
    public int maxpps { get; private set; }
    /// <summary>
    /// 游戏主机
    /// </summary>
    public string gamehost { get; private set; }
    /// <summary>
    /// 游戏端口
    /// </summary>
    public ushort gameport { get; private set; }
    /// <summary>
    /// 游戏帧率
    /// </summary>
    public byte gamefps { get; private set; }
    
    protected override void OnCreate()
    {
        base.OnCreate();
        var jobj = JObject.Parse(File.ReadAllText($"{Directory.GetCurrentDirectory()}/Res/settings.json"));
        name = jobj.Value<string>("name");
        host = jobj.Value<string>("host");
        port = jobj.Value<ushort>("port");
        wsport = jobj.Value<ushort>("wsport");
        maxconn = jobj.Value<int>("maxconn");
        sthread = jobj.Value<int>("sthread");
        maxpps = jobj.Value<int>("maxpps");
        gamehost = jobj.Value<string>("gamehost");
        gameport = jobj.Value<ushort>("gameport");
        gamefps = jobj.Value<byte>("gamefps");
    }
}