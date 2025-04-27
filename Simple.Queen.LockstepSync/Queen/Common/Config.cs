using Luban;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Queen.Core;

namespace Queen.Common;

/// <summary>
/// 游戏配置
/// </summary>
public class Config : Comp
{
    /// <summary>
    /// 配置表定位器
    /// </summary>
    public Tables location { get; set; }
    /// <summary>
    /// 系数 0.5
    /// </summary>
    public const float half = 0.5f;
    /// <summary>
    /// 系数 1
    /// </summary>
    public const float one = 1f;
    /// <summary>
    /// 系数 1000
    /// </summary>
    public const float thousand = 1000f;
    /// <summary>
    /// 浮点数转整型的乘法系数（10000 表示 1）
    /// </summary>
    public const int float2Int = 10000;
    /// <summary>
    /// 整型转浮点的乘法系数（10000 表示 1）
    /// </summary>
    public const float int2Float = 0.0001f;
    /// <summary>
    /// 资源目录
    /// </summary>
    private string respath { get { return $"{Directory.GetCurrentDirectory()}/Res/"; } }

    protected override void OnCreate()
    {
        base.OnCreate();

        location = new Tables((cfgName) => { return new ByteBuf(File.ReadAllBytes($"{respath}Configs/{cfgName}.bytes")); });
    }
}
