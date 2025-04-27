namespace Queen.Common.Extensions;

/// <summary>
/// NewtonJson 扩展
/// </summary>
public static class NewtonJsonExtensions
{
    /// <summary>
    /// Dictionary(string, string) 转换为 Json 字符串
    /// </summary>
    /// <param name="ssd">Dictionary(string, string)</param>
    /// <returns>Json : string</returns>
    public static string SSD2Json(this Dictionary<string, string> ssd)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(ssd);
    }

    /// <summary>
    /// Json 字符串转换为 Dictionary(string, string)
    /// </summary>
    /// <param name="content">Json : string</param>
    /// <returns>Dictionary(string, string)</returns>
    public static Dictionary<string, string> Json2SSD(this string content)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(content);
    }
}
