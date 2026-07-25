using System;
using System.Collections.Generic;
using Goblin.Common;

namespace Goblin.Gameplay.Projection;

/// <summary>
/// 表现实体 — Actor 的容器，关联一组 Component
/// 每个 Logic Actor 对应一个 Render Entity
/// </summary>
public class Entity
{
    /// <summary>
    /// ActorID
    /// </summary>
    public ulong actor { get; }

    /// <summary>
    /// 组件字典，键为 Component 类型
    /// </summary>
    private Dictionary<Type, Component> comps { get; set; }

    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="actor">ActorID</param>
    internal Entity(ulong actor)
    {
        this.actor = actor;
        comps = ObjectPool.Ensure<Dictionary<Type, Component>>();
    }

    /// <summary>
    /// 获取组件（泛型）
    /// </summary>
    public T GetComp<T>() where T : Component
    {
        if (comps.TryGetValue(typeof(T), out var c)) return c as T;
        return null;
    }

    /// <summary>
    /// 获取组件（非泛型，按 Type）
    /// </summary>
    internal Component GetComp(Type comptype)
    {
        return comps.TryGetValue(comptype, out var c) ? c : null;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    internal void AddComp(Component comp)
    {
        comp.entity = this;
        comps[comp.GetType()] = comp;
        comp.OnCreate();
    }

    /// <summary>
    /// 移除组件
    /// </summary>
    internal void RmvComp<T>() where T : Component
    {
        if (comps.TryGetValue(typeof(T), out var comp))
        {
            comp.OnDestroy();
            comps.Remove(typeof(T));
            ObjectPool.Set(comp);
        }
    }

    /// <summary>
    /// 销毁实体，回收所有组件
    /// </summary>
    internal void Destroy()
    {
        foreach (var comp in comps.Values)
        {
            comp.OnDestroy();
            ObjectPool.Set(comp);
        }
        comps.Clear();
        ObjectPool.Set(comps);
    }
}
