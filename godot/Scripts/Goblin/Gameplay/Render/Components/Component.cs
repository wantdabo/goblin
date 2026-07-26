using Goblin.Common;

namespace Goblin.Gameplay.Render.Components;

/// <summary>
/// 渲染组件标记基类 — 供 Mirror 查询类型约束，实现 IGBL 以支持对象池
/// </summary>
public abstract class Component : IGBL
{
    /// <summary>
    /// 浅拷贝（子类重写以支持深拷贝）
    /// </summary>
    public virtual IGBL Clone()
    {
        return (IGBL)MemberwiseClone();
    }

    /// <summary>
    /// 重置状态（对象池回收前调用，子类有状态时重写）
    /// </summary>
    public virtual void Reset()
    {
    }
}
