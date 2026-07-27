using Goblin.Common;
using Goblin.Gameplay.Logic.Common;

namespace Goblin.Gameplay.Logic.Commands.Common;

/// <summary>
/// 输入指令 — 实现 IGBL，走池生命周期
/// </summary>
public abstract class Command : IGBL
{
    /// <summary>
    /// 指令
    /// </summary>
    public abstract ushort id { get; }

    /// <summary>
    /// 重置
    /// </summary>
    public virtual void Reset()
    {
        OnReset();
    }

    /// <summary>
    /// 克隆，复用参数版本
    /// </summary>
    /// <param name="clone">目标</param>
    /// <returns>目标克隆后</returns>
    public Command Clone(Command clone)
    {
        OnClone(clone);
        return clone;
    }

    /// <summary>
    /// 克隆，无参 IGBL 版本
    /// </summary>
    IGBL IGBL.Clone()
    {
        var c = ObjectCache.Ensure(this.GetType()) as Command;
        OnClone(c);
        return c;
    }

    /// <summary>
    /// 重置
    /// </summary>
    protected abstract void OnReset();
    /// <summary>
    /// 克隆
    /// </summary>
    /// <param name="clone">目标</param>
    protected abstract void OnClone(Command clone);
}