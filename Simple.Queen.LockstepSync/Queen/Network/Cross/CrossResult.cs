namespace Queen.Network.Cross;

/// <summary>
/// RPC 状态
/// </summary>
public static class CROSS_STATE
{
    /// <summary>
    /// 等待
    /// </summary>
    public const ushort WAIT = 1;
    /// <summary>
    /// 成功
    /// </summary>
    public const ushort SUCCESS = 2;
    /// <summary>
    /// 错误
    /// </summary>
    public const ushort ERROR = 3;
    /// <summary>
    /// 超时
    /// </summary>
    public const ushort TIMEOUT = 4;
    /// <summary>
    /// 404
    /// </summary>
    public const ushort NOTFOUND = 5;
}

/// <summary>
/// RPC 结果
/// </summary>
public class CrossResult : CrossContentConv
{
    /// <summary>
    /// RPC 状态
    /// </summary>
    public ushort state { get; set; } = CROSS_STATE.WAIT;
    /// <summary>
    /// 传输内容
    /// </summary>
    public string content { get; set; }
}
