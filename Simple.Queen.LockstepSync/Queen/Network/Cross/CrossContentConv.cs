namespace Queen.Network.Cross;

/// <summary>
/// RPC 通信传输数据转换
/// </summary>
public abstract class CrossContentConv
{
    /// <summary>
    /// 实例转 Json
    /// </summary>
    /// <param name="content">传输内容</param>
    /// <typeparam name="T">NewtonJson 的转化类型</typeparam>
    /// <returns>对应类型的实例</returns>
    public T Conv<T>(string content)
    {
        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
    }
    
    /// <summary>
    /// Json 转实例
    /// </summary>
    /// <param name="content">传输内容</param>
    /// <typeparam name="T">NewtonJson 的转化类型</typeparam>
    /// <returns>对应实例的 Json</returns>
    public string Conv<T>(T content)
    {
        return Newtonsoft.Json.JsonConvert.SerializeObject(content);
    }
}