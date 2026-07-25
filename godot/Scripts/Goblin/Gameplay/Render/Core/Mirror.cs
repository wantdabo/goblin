using System;
using System.Collections.Generic;
using Goblin.Common;
using Goblin.Gameplay.Projection.Core;
using Goblin.Gameplay.Render.Components;

namespace Goblin.Gameplay.Render.Core;

/// <summary>
/// 数据镜像 — Logic 层 BehaviorInfo 数据的纯数据主线程副本
/// ActorID → (ComponentType → Component实例) 扁平组织
/// </summary>
public class Mirror
{
    /// <summary>
    /// ActorID → (ComponentType → Component实例)
    /// </summary>
    private Dictionary<ulong, Dictionary<Type, object>> datas { get; set; }
    /// <summary>
    /// BehaviorInfo 类型 → Component 类型 映射
    /// </summary>
    private Dictionary<Type, Type> infotocomp { get; set; }
    /// <summary>
    /// Component 类型 → ApplyTo 静态委托
    /// </summary>
    private Dictionary<Type, Action<object, ulong, object[]>> applymap { get; set; }
    /// <summary>
    /// 事件去重缓存
    /// </summary>
    private HashSet<string> eventframecache { get; set; }

    public Mirror()
    {
        datas = ObjectPool.Ensure<Dictionary<ulong, Dictionary<Type, object>>>();
        infotocomp = ObjectPool.Ensure<Dictionary<Type, Type>>();
        applymap = ObjectPool.Ensure<Dictionary<Type, Action<object, ulong, object[]>>>();
        eventframecache = ObjectPool.Ensure<HashSet<string>>();
    }

    /// <summary>
    /// 注册 BehaviorInfo → Component 映射
    /// </summary>
    public void Register<TInfo, TComp>()
        where TComp : Component, new()
    {
        var infotype = typeof(TInfo);
        var comptype = typeof(TComp);
        infotocomp[infotype] = comptype;

        // 通过反射拿 ApplyTo 静态方法
        var method = comptype.GetMethod("ApplyTo",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
        if (null != method)
        {
            applymap[comptype] = (Action<object, ulong, object[]>)
                Delegate.CreateDelegate(typeof(Action<object, ulong, object[]>), method);
        }
    }

    /// <summary>
    /// 获取指定 Actor 的组件
    /// </summary>
    public T GetComp<T>(ulong actor) where T : Component
    {
        if (datas.TryGetValue(actor, out var compdict)
            && compdict.TryGetValue(typeof(T), out var comp))
            return comp as T;
        return null;
    }

    /// <summary>
    /// 应用单条投影数据
    /// </summary>
    private void Apply(ulong actor, Type infotype, ulong fieldmask, object[] values)
    {
        if (false == infotocomp.TryGetValue(infotype, out var comptype)) return;

        if (false == datas.TryGetValue(actor, out var compdict))
        {
            compdict = ObjectPool.Ensure<Dictionary<Type, object>>();
            datas[actor] = compdict;
        }

        if (false == compdict.TryGetValue(comptype, out var comp))
        {
            comp = Activator.CreateInstance(comptype);
            compdict[comptype] = comp;
        }

        if (applymap.TryGetValue(comptype, out var apply))
            apply(comp, fieldmask, values);
    }

    /// <summary>
    /// 批量应用 ObserverPacket
    /// </summary>
    public void ApplyPackets(ObserverPacket[] packets)
    {
        if (null == packets) return;
        eventframecache.Clear();
        foreach (var p in packets)
        {
            var key = $"{p.actor}_{p.behaviorinfotype.Name}";
            if (false == eventframecache.Add(key)) continue;
            Apply(p.actor, p.behaviorinfotype, p.fieldmask, p.values);
        }
    }

    /// <summary>
    /// 移除 Actor 的所有数据
    /// </summary>
    public void RmvActor(ulong actor)
    {
        if (datas.TryGetValue(actor, out var compdict))
        {
            compdict.Clear();
            ObjectPool.Set(compdict);
            datas.Remove(actor);
        }
    }
}
