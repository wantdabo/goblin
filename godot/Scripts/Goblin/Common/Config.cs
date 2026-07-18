using Goblin.Core;
using Luban;
#if DEV
using System.Text.Json;
#endif

namespace Goblin.Common;

/// <summary>
/// 游戏配置
/// </summary>
public class Config : Comp
{
    /// <summary>
    /// 配置表定位器
    /// </summary>
    public Tables location { get; private set; }

    /// <summary>
    /// 浮点数转整型的乘法系数（1000 表示 1）
    /// </summary>
    public const int Float2Int = 1000;

    /// <summary>
    /// 整型转浮点的乘法系数（1000 表示 1）
    /// </summary>
    public const float Int2Float = 0.001f;

    protected override void OnCreate()
    {
        base.OnCreate();
#if DEV
        location = new Tables((cfgname) =>
        {
            var json = engine.gameres.location.LoadConfigJson(cfgname);
            return JsonDocument.Parse(json).RootElement.Clone();
        });
#else
        location = new Tables((cfgname) => new ByteBuf(engine.gameres.location.LoadConfigSync(cfgname)));
#endif
    }
}