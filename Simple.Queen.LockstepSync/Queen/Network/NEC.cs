namespace Queen.Network;

/// <summary>
/// NEC (NodeErrorCode)
/// 网络节点错误码
/// 10000 开头表示警告， 20000 开头表示错误
/// </summary>
public class NEC
{
    /// <summary>
    /// PPS 超过了服务器设定值
    /// </summary>
    public const uint PPS_LIMIT = 10001;
}