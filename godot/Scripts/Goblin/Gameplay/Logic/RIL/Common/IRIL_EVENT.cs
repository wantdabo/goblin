namespace Goblin.Gameplay.Logic.RIL.Common;

/// <summary>
/// RIL 事件, 不允许存在引用 Field, 请使用纯值类型
/// </summary>
public abstract class IRIL_EVENT
{
    /// <summary>
    /// 指令
    /// </summary>
    public abstract ushort id { get; }
        
    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        OnReset();
    }
        
    /// <summary>
    /// 克隆
    /// </summary>
    /// <param name="clone">目标</param>
    /// <returns>目标克隆后</returns>
    public IRIL_EVENT Clone(IRIL_EVENT clone)
    {
        OnClone(clone);

        return clone;
    }
        
    /// <summary>
    /// 重置
    /// </summary>
    protected abstract void OnReset();
    /// <summary>
    /// 克隆
    /// </summary>
    /// <param name="clone">目标</param>
    protected abstract void OnClone(IRIL_EVENT clone);
}