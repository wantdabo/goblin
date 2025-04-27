namespace Queen.Common.Parallel.Instructions;

/// <summary>
/// 协程指令
/// </summary>
public abstract class Instruction
{
    /// <summary>
    /// 指令已经完成
    /// </summary>
    public bool finished { get; protected set; }
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="tick">tick</param>
    public abstract void Update(float tick);
}